namespace test;

using System.Collections;
using Protocol.ToshimonStateTransition.Service;
using Protocol.ToshimonStateTransition.ContractDefinition;

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

		var advanceStateFunction = new AdvanceStateTypedFunction();
            advanceStateFunction.GameState = gameState;
            advanceStateFunction.Outcome = outcome.ToList();
            advanceStateFunction.MoveA = (byte) actions[0];
            advanceStateFunction.MoveB = (byte) actions[1];
            advanceStateFunction.RandomSeed = Enumerable.Repeat((byte) 0, 32).ToArray();

        // gas estimation doesn't work for advanceState due to the outside contract calls
        // Just make this something large..
        advanceStateFunction.Gas = 100000000;

		var result = service.AdvanceStateTypedQueryAsync(advanceStateFunction).Result; // block for results on async function. Can modify to be async if desired

		// deserialize and return result
		return (
			result.ReturnValue1,
			result.ReturnValue2.ToArray(),
			result.ReturnValue3
		);
	}
}

public struct StateSnapshot {
	public GameState before;
	public GameState after;
	public GameAction moveA;
	public GameAction moveB;
	public uint randomSeed;

	public PlayerStateDiff Player(int index) {
		return new PlayerStateDiff(before[index], after[index]);
	}
}

// Useful class to keep track of the historical game states and helpful tools to make new ones
// It stores snapshots of before and after state transitions at each stage
public class StateEngine {
	public List<StateSnapshot> States { get; } = new List<StateSnapshot>();
	IStateTransition stateTransition;

	public StateEngine(IStateTransition st) {
		this.stateTransition = st;
	}

	public void Init(GameState initialState) {
		this.States.Add(new StateSnapshot {
			after = initialState,
		});
	}

	public StateSnapshot Last() {
		return this.States.Last();
	}

	// evolve the game state and return it (but also add it to the internal list)
	public GameState next(GameAction move1, GameAction move2, uint randomSeed) {
		GameState before = this.Last().after;
		(GameState after, _, _) = this.stateTransition.advanceState(before, new SingleAssetExit[]{}, new GameAction[]{move1, move2}, randomSeed);
		this.States.Add(new StateSnapshot {
			before = before,
			after = after,
			moveA = move1,
			moveB = move2,
			randomSeed = randomSeed,
		});
		return after;
	}

	// Indexer declaration.
    // If index is out of range, the temps array will throw the exception.
    public StateSnapshot this[int index] {
        get => States[index];
    }

	// quick check that the before/next fields are set correctly
	public bool isConsistent() {
		for (int i = 1; i < this.States.Count; i++) {
			if ( this.States[i-1].after != this.States[i].before ) {
				return false;
			}
		}
		return true;
	}

}
