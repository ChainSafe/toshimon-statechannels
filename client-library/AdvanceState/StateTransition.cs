namespace toshimon_state_machine;

using Protocol;
using Protocol.ToshimonStateTransition.Service;

/**
 * This is the primary way a game client will integrate with the rules. 
 * An implementor of IStateTransition must provide a function to calculate 
 * a new toshimon game state given two actions and a random seed
 *
 * It is a pure function in that it should not mutate any of its inputs or make use
 * of any external data. Given the same data in it must always produce the same result!
 *
 */
 public interface IStateTransition {
	// Apply moves to transition to a new game state
	// The passed GameState and Outcome objects will not be
	// modified and a copy will be returned with the changes applied
 	public abstract (GameState, SingleAssetExit[], bool) advanceState(
 		GameState gameState,
 		SingleAssetExit[] outcome,
 		GameAction[] actions,
 		uint randomSeed
 		);
 }

public class EvmStateTransition : IStateTransition {
	
	protected Nethereum.Web3.Web3 Web3 { get; }
	protected string ContractAddress { get; }

	public EvmStateTransition(Nethereum.Web3.Web3 web3, string contractAddress) {
		Web3 = web3;
		ContractAddress = contractAddress;
	}

	public (GameState, SingleAssetExit[], bool) advanceState(
 		GameState gameState,
 		SingleAssetExit[] outcome,
 		GameAction[] actions,
 		uint randomSeed
 		) {	
		var service = new ToshimonStateTransitionService(Web3, ContractAddress);
		// serialize params and call function
		var result = service.AdvanceStateQueryAsync(
			gameState.AbiEncode(),
			new List<SingleAssetExit>(outcome),
			(byte) actions[0],
			(byte) actions[1],
			BitConverter.GetBytes(randomSeed)
		).Result; // block for results on async function. Can modify to be async if desired
		// deserialize and return result
		return (
			GameState.AbiDecode(result.ReturnValue1),
			result.ReturnValue2.ToArray(),
			result.ReturnValue3
		);
	}
}

 public class LocalStateTransition : IStateTransition {

 	public (GameState, SingleAssetExit[], bool) advanceState(
 		GameState gameState,
 		SingleAssetExit[] outcome,
 		GameAction[] actions,
 		uint randomSeed
 		) {	

 		// if either player is unconcious then no more moves can be made
 		// and the game is over. No further state updates possible.
 		if (gameState[0].isUnconcious() || gameState[1].isUnconcious()) {
 			return (gameState, outcome, true);
 		}

		// first up resolve any switch monster actions or items. 
		// These occur first and order between players doesn't matter
		for (int i = 0; i < 2; i++) {
			PlayerState player = gameState[i];
			MonsterCard monster = player.GetActiveMonster();

			if ( isSwapAction(actions[i]) ) {
				player = player with { ActiveMonsterIndex = (int) actions[i] - 4 };
			} else if ( isItemAction(actions[i]) ) {
				int itemIndex = (int) actions[i] - 9;
				Item item = Lookup.Items[player.Items[itemIndex].Definition];
				gameState = item.applyItem(gameState, i, player.ActiveMonsterIndex, randomSeed);

				player = gameState[i].SetItemUsed(itemIndex);
				// TODO mark any item to destroy in the outcome as well
			} 
			gameState = gameState.SetPlayer(player, i);
		}

		// if just one player is attacking this can be resolved right away
		for (int i = 0; i < 2; i++) {
			if ( isAttackAction(actions[i]) && !isAttackAction(actions[not(i)]) ) {
				gameState = resolveTurn(gameState, (int) actions[i],  i, randomSeed);
			}
		}

		// If both players are attacking it is a bit tricker
		// Need to use speed and/or properties of the move to resolve

		MonsterCard monsterA = gameState.PlayerA.GetActiveMonster();
		MonsterCard monsterB = gameState.PlayerB.GetActiveMonster();

		if ( isAttackAction(actions[0]) && isAttackAction(actions[1]) ) {
			Logger.log("Both players are attacking");

			if ( monsterA.isFasterThan(monsterB) ) {
				Logger.log("Player A is attacking first");
				gameState = resolveTurn(gameState, (int) actions[0],  0, randomSeed);
				gameState = resolveTurn(gameState, (int) actions[1],  1, randomSeed);
			} else {
				Logger.log("Player B is attacking first");
				gameState = resolveTurn(gameState, (int) actions[1],  1, randomSeed);
				gameState = resolveTurn(gameState, (int) actions[0],  0, randomSeed);

			} // TODO consider case where speeds are equal. What is the tiebreaker?
		}		

		return (gameState, outcome, false);
	}

	private bool isAttackAction(GameAction action) {
		return ( (int) action >= 0 && (int) action <=3);
	}

	private bool isSwapAction(GameAction action) {
		return ( (int) action >= 4 && (int) action <=8);
	}

	private bool isItemAction(GameAction action) {
		return ( (int) action >= 9 && (int) action <=13);
	}

	// helper for flipping the player index
	private static int not(int i) {
		return i switch {
			0 => 1,
			1 => 0,
			_ => throw new IndexOutOfRangeException(),
		};
	}

	/**
	 * Applies status conditions and moves if the status allows it
	 */
	private GameState resolveTurn(GameState gameState, int moveIndex, int mover, uint randomSeed) {
		(gameState, bool canMove) = resolveStatusConditionBeforeMove(gameState, mover, randomSeed);
		if (canMove) {
			gameState = makeMove(gameState, moveIndex,  mover, randomSeed);
		} else {
			Logger.log("Cannot move due to status condition");
		}
		return resolveStatusConditionAfterTurn(gameState, mover, randomSeed);
	}

	/**
	 * Applies status condition effects and returns a new state and if it is possible to move
	 * due the the status
	 */
	private (GameState, bool) resolveStatusConditionBeforeMove(GameState gameState, int mover, uint randomSeed) {
		MonsterCard attacker = gameState[mover].GetActiveMonster();
		if (attacker is StatusCondition status) {
			return status.onBeforeMove(gameState, mover, gameState[mover].ActiveMonsterIndex, randomSeed);
		} else {
			// no status condition
			return (gameState, true);
		}
	}

	/**
	 * Applies post-turn status condition effects and returns a new state
	 */
	private GameState resolveStatusConditionAfterTurn(GameState gameState, int mover, uint randomSeed) {
		MonsterCard attacker = gameState[mover].GetActiveMonster();
		if (attacker is StatusCondition status) {
			return status.onAfterTurn(gameState, mover, gameState[mover].ActiveMonsterIndex, randomSeed);
		} else {
			// no status condition
			return gameState;
		}
	}

	/**
	 * Apply a move to get a new game state
	 * This checks if the mover is unconcious or has insuffient PP for the move
	 * and will not apply it in those cases
	 * @return GameState - A new state object with the move applied
	 */
	private GameState makeMove(GameState gameState, int moveIndex, int mover, uint randomSeed) {
		MonsterCard attacker = gameState[mover].GetActiveMonster();
		MonsterCard defender = gameState[not(mover)].GetActiveMonster();

		Move move = Lookup.Moves[attacker.Moves[moveIndex]];

		// bail if attacker is unconcious or no PP available
		if (attacker.Stats.Hp == 0) {
			Logger.log(String.Format("Cannot attack due to no HP"));
			return gameState;
		}
		if (attacker.Stats.PP[moveIndex] == 0) {
			Logger.log(String.Format("Cannot attack due to no PP for chosen move"));
			return gameState;
		}		

		// reduce the PP of the attacker on that move
		attacker = attacker.DecrementPP(moveIndex);
		gameState = gameState.SetPlayer(gameState[mover].SetActiveMonster(attacker), mover);

		// apply move
		gameState = move.applyMove(gameState, mover, randomSeed);
		
		return gameState; 
	}
}
