namespace toshimon_state_machine;

using System.Text.Json;
using System.Collections.Immutable;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.ABI;

using Protocol;

/**
 * A collection of methods added into the Protocol/GameState records
 * to make the state transition code more idiomatic and readable
 */

public static class GameStateExtension {
    public static GameState SetPlayer(this GameState self, PlayerState player, int index) {
    	return index switch {
    		0 => self with { PlayerA = player },
       		1 => self with { PlayerB = player },
       		_ => throw new IndexOutOfRangeException(index.ToString()),
    	};
    }
}

public static class PlayerStateExtensions {

	public static PlayerState SetMonster(this PlayerState self, MonsterCard monster, int index) {
		return self with { Monsters = self.Monsters.SetItem(index, monster) };
	}

	public static MonsterCard GetActiveMonster(this PlayerState self) {
		return self.Monsters[self.ActiveMonsterIndex];
	}

	public static PlayerState SetActiveMonster(this PlayerState self, MonsterCard monster) {
		return self.SetMonster(monster, self.ActiveMonsterIndex);
	}

	public static PlayerState AddItem(this PlayerState self, ItemCard item) {
		return self with { Items = self.Items.Add(item) };
	}

	public static PlayerState SetItemUsed(this PlayerState self, int itemIndex) {
		ItemCard item = self.Items[itemIndex] with { Used = true };
		return self with { Items = self.Items.SetItem(itemIndex, item) };
	}

	public static bool isUnconcious(this PlayerState self) {
		return self.Monsters.All(monster => monster.Stats.Hp == 0);
	}
}

public static class MonsterCardExtension {

	public static bool isAlive(this MonsterCard self) {
		return self.Stats.Hp > 0;
	}

	public static bool isFasterThan(this MonsterCard self, MonsterCard opponent) {
		return self.Stats.Speed > opponent.Stats.Speed;
	}

	public static MonsterCard TakeDamage(this MonsterCard self, uint damage) {
		Stats stats = self.Stats with { Hp = SafeMath.subtract(self.Stats.Hp, damage) };
		return self with { Stats = stats };
	}

	/**
	 * Recovers Hp but cannot go above the maximum Hp in the Base stats
	 * @param uint amount to increase the Hp by
	 */
	public static MonsterCard HealHp(this MonsterCard self, uint amount) {
		Stats stats = self.Stats with { Hp = Math.Min(self.Stats.Hp + amount, self.BaseStats.Hp) };
		return self with { Stats = stats };
	}	

	public static MonsterCard DecrementPP(this MonsterCard self, int moveIndex) {
		Stats stats = self.Stats with { PP = self.Stats.PP.SetItem(moveIndex, SafeMath.subtract(self.Stats.PP[moveIndex], 1)) };
		return new MonsterCard(self.BaseStats, stats, self.Moves);
	}
}
