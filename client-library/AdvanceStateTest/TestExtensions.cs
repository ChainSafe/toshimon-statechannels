// adds some extension methods on existing types to 
// help make the tests more readable and expressive

namespace test;

public static class MonsterCardExtensions {
	public static MonsterCardDiff diff(this MonsterCard before, MonsterCard after) {
		return new MonsterCardDiff(before, after);
	}
}

public static class PlayerStateExtensions {
	public static PlayerStateDiff diff(this PlayerState before, PlayerState after) {
		return new PlayerStateDiff(before, after);
	}
}


public class PlayerStateDiff {
	public PlayerState before;
	public PlayerState after;

	public PlayerStateDiff(PlayerState before, PlayerState after) {
		this.before = before;
		this.after = after;
	}

	public MonsterCardDiff Monster(int monsterIndex) {
		return new MonsterCardDiff(before.Monsters[monsterIndex], after.Monsters[monsterIndex]);
	}
}

public class MonsterCardDiff {
	public MonsterCard before;
	public MonsterCard after;

	public MonsterCardDiff(MonsterCard before, MonsterCard after) {
		this.before = before;
		this.after = after;
	}

	public bool HasTakenDamage(uint damage) {
		return (before.Stats.Hp - damage) == after.Stats.Hp;
	}

	public bool HasDecreasedPP(int moveIndex, uint decrease) {
		return (before.Stats.PP[moveIndex] - decrease) == after.Stats.PP[moveIndex];
	}
}
