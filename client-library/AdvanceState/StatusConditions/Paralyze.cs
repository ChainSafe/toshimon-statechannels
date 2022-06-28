namespace toshimon_state_machine;

using Protocol;


/**
 * 1 in 5 chance to be unable to use attacks during the duration
 */
public class Paralyze : StatusCondition {

	public static string Id { get { return "0x0000000000000000000000000000000000000001"; } }

	public GameState onStart(GameState state, int mover, int monsterIndex, uint randomSeed) {
		MonsterCard monster = state[mover].Monsters[monsterIndex];
		monster.StatusCondition = Id;
		monster.StatusConditionCounter = DetRandom.uniform(3, 6, randomSeed, 456546456);
		return state.SetPlayer(state[mover].SetMonster(monster, monsterIndex), mover);
	}

	public (GameState, bool) onBeforeMove(GameState state, int mover, int monsterIndex, uint randomSeed) {
		if (DetRandom.uniform(1, 5, randomSeed, 2342323) == 1) {
			return (state, false);
		} else {
			return (state, true);
		}
	}

	public GameState onAfterTurn(GameState state, int mover, int monsterIndex, uint randomSeed) {
		MonsterCard monster = state[mover].Monsters[monsterIndex];
		monster.StatusConditionCounter -= 1;
		return state.SetPlayer(state[mover].SetMonster(monster, monsterIndex), mover);
	}

}
