namespace test;

using System.Collections;

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
public class StateEngine<ST> where ST: IStateTransition, new() {
	public List<StateSnapshot> States { get; } = new List<StateSnapshot>();
	ST stateTransition;

	public StateEngine() {
		this.stateTransition = new ST();
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
