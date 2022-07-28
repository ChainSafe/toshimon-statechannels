/**
 * Adds some extension methods on existing types to 
 * help make the tests more readable and expressive
 */

namespace test;

using System.Collections.Immutable;

public static class GameStateExtension {
    public static GameState SetPlayer(this GameState self, PlayerState player, int index) {
    	return index switch {
    		0 => self with { PlayerA = player },
       		1 => self with { PlayerB = player },
       		_ => throw new IndexOutOfRangeException(index.ToString()),
    	};
    }
}

public static class MonsterCardExtensions {
	public static MonsterCardDiff diff(this MonsterCard before, MonsterCard after) {
		return new MonsterCardDiff(before, after);
	}

	public static bool isAlive(this MonsterCard self) {
		return self.Stats.Hp > 0;
	}

	public static bool isFasterThan(this MonsterCard self, MonsterCard opponent) {
		return self.Stats.Speed > opponent.Stats.Speed;
	}

	public static MonsterCard TakeDamage(this MonsterCard self, byte damage) {
		Stats stats = self.Stats with { Hp = (byte)SafeMath.subtract(self.Stats.Hp, damage) };
		return self with { Stats = stats };
	}

	public static MonsterCard HealHp(this MonsterCard self, byte amount) {
		Stats stats = self.Stats with { Hp = (byte)Math.Min(self.Stats.Hp + amount, self.BaseStats.Hp) };
		return self with { Stats = stats };
	}	

	public static MonsterCard DecrementPP(this MonsterCard self, int moveIndex) {
		Stats stats = self.Stats with { PP = ImmutableArray.Create<uint>(self.Stats.PP.ToArray()).SetItem(moveIndex, SafeMath.subtract(self.Stats.PP[moveIndex], 1)).ToList() };
		return self with { Stats = stats };
	}
}

public static class PlayerStateExtensions {
	public static PlayerStateDiff diff(this PlayerState before, PlayerState after) {
		return new PlayerStateDiff(before, after);
	}

	public static PlayerState SetMonster(this PlayerState self, MonsterCard monster, int index) {
		return self with { Monsters = ImmutableArray.Create<MonsterCard>(self.Monsters.ToArray()).SetItem(index, monster).ToList() };
	}

	public static MonsterCard GetActiveMonster(this PlayerState self) {
		return self.Monsters[self.ActiveMonsterIndex];
	}

	public static PlayerState SetActiveMonster(this PlayerState self, MonsterCard monster) {
		return self.SetMonster(monster, self.ActiveMonsterIndex);
	}

	public static PlayerState SetItemUsed(this PlayerState self, int itemIndex) {
		ItemCard item = self.Items[itemIndex] with { Used = true };
		return self with { Items = ImmutableArray.Create<ItemCard>(self.Items.ToArray()).SetItem(itemIndex, item).ToList() };
	}

	public static bool isUnconcious(this PlayerState self) {
		return self.Monsters.All(monster => monster.Stats.Hp == 0);
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

static class SafeMath {
	public static uint subtract(uint x, uint y) {
		if (y > x) return 0;
		else return x - y;
	}
}
