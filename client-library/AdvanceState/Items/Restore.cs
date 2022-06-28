namespace toshimon_state_machine;

using Protocol;

public class Restore: Item {
	
	public static string Id { get { return "0x0000000000000000000000000000000000000001"; } }

	public GameState applyItem(GameState state, int mover, int monster, uint randomSeed) {
		Logger.log(String.Format("Player is using a Restore"));

		MonsterCard reciever = state[mover].GetActiveMonster();
		reciever = reciever.HealHp(25);
		return state.SetPlayer(state[mover].SetActiveMonster(reciever), mover);
	}
}
