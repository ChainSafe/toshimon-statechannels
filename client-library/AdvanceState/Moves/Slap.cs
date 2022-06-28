namespace toshimon_state_machine;

using Protocol;

/**
 * A simple attack move
 */
public class Slap : BaseAttackMove {

	public static string Id { get { return "0x0000000000000000000000000000000000000002"; } }

	public override ToshimonType MoveType { get { return ToshimonType.None; } }
	public override uint ExtraCrit { get { return 0; } }

	public override uint Power(GameState state, int mover, uint randomSeed) {
		return 80;
	}

	public override uint Accuracy(GameState state, int mover, uint randomSeed) {
		return 70;
	}
}
