namespace toshimon_state_machine;

using Protocol;

/**
 * A test move that does a fixed 10 HP of damage regardless
 * Useful for testing
 */
public class DoTen : BaseMove {

	public static string Id { get { return "0x0000000000000000000000000000000000000001"; } }

	public override uint Accuracy(GameState state, int mover, uint randomSeed) {
		return 100;
	}

	public override GameState applyMove(GameState state, int mover, uint randomSeed) {

		MonsterCard attacker = state[mover].GetActiveMonster();
		MonsterCard defender = state[not(mover)].GetActiveMonster();

		defender = defender.TakeDamage(10);

		// update the monsters
		state = state.SetPlayer(state[mover].SetActiveMonster(attacker), mover);
		state = state.SetPlayer(state[not(mover)].SetActiveMonster(defender), not(mover));

		return state;
	}
}
