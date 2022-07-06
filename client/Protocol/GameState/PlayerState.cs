namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * State of one player at an instant in time
 */
public record PlayerState {
    [Parameter("tuple[]", "monsters", 1)]
    public List<MonsterCard> Monsters { get; set; }

    [Parameter("tuple[]", "items", 2)]
    public List<ItemCard> Items { get; set; }

    [Parameter("uint8", "activeMonsterIndex", 3)]
    public byte ActiveMonsterIndex { get; set; }

    public PlayerState() {}

    public PlayerState(MonsterCard[] monsters, ItemCard[] items) {
        this.Monsters = new List<MonsterCard>(monsters);
        this.Items = new List<ItemCard>(items);
        this.ActiveMonsterIndex = 0;
    }
}
