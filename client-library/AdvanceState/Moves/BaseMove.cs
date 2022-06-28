namespace toshimon_state_machine;

using Protocol;

public abstract class BaseMove : Move {

	public static string Id { get; }

	// helper for flipping the player index
	protected static int not(int i) {
		return i switch {
			0 => 1,
			1 => 0,
			_ => throw new IndexOutOfRangeException(),
		};
	}	

	public abstract uint Accuracy(GameState state, int mover, uint randomSeed);
	public abstract GameState applyMove(GameState state, int mover, uint randomSeed);
}
