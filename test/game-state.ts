// Test representation of the game state and experimental state transitions
// this is for getting an idea for patterns to implement the complexity of the
// state transition function

interface GameState {
	players: [PlayerState, PlayerState]
}

interface PlayerState {
	monsters: [MonsterCard, MonsterCard, MonsterCard, MonsterCard, MonsterCard],
	items: Array<ItemCard>,
	activeMonster: number,
}

interface MonsterCard {
	cardId: number,
	baseStats: Array<number>,
	stats: Array<number>,
	type: number,
	moves: [MoveFn?, MoveFn?, MoveFn?],
}

type MoveFn = (gameState: Readonly<GameState>, mover: number, randomSeed: number) => GameState;

const moves: Map<string, MoveFn> = new Map([
	["tackle", (gameState, mover, seed) => {

	}]
]);

interface ItemCard {
	cardId: number,
	attachedTo: number,
	move: MoveFn,
	used: boolean,
}

interface Outcome {

}

enum Move {
	// attack using the moves on the
	// current active monster
	MonsterMove1,
	MonsterMove2,
	MonsterMove3,
	// Switch the active monster
	Swap1,
	Swap2,
	Swap3,
	Swap4,
	Swap5,
	// Use an item in hand
	Item1,
	Item2,
	Item3,
	Item4,
	Item5,
}

function advanceState(
	gameState: Readonly<GameState>,
	outcome: Readonly<Outcome>,
	moveA: Move,
	moveB: Move,
	randomSeed: number
): [GameState, Outcome, boolean] {
	return [gameState, outcome, false]
}
