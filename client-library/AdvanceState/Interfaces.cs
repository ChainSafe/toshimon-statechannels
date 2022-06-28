namespace toshimon_state_machine;

using Protocol;

public interface StatusCondition {
	GameState onStart(GameState state, int mover, int monster, uint randomSeed);
	(GameState, bool) onBeforeMove(GameState state, int mover, int monster, uint randomSeed);
	GameState onAfterTurn(GameState state, int mover, int monster, uint randomSeed);
}

public interface Move {
	GameState applyMove(GameState state, int mover, uint randomSeed);
}

public interface Item {
	GameState applyItem(GameState state, int mover, int monster, uint randomSeed);
}
