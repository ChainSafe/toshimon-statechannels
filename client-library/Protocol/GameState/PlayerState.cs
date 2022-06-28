namespace Protocol;

using System;
using System.Numerics;
using System.Collections.Immutable;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * State of one player at an instant in time
 */
public record PlayerState {
    [Parameter("tuple[]", "monsters", 1)]
    public ImmutableArray<MonsterCard> Monsters { get; set; }

    [Parameter("tuple[]", "items", 2)]
    public ImmutableArray<ItemCard> Items { get; set; }

    [Parameter("uint8", "activeMonsterIndex", 3)]
    public int ActiveMonsterIndex { get; set; }


    public PlayerState(MonsterCard[] monsters, ItemCard[] items) {
        this.Monsters = ImmutableArray.Create<MonsterCard>(monsters);
        this.Items = ImmutableArray.Create<ItemCard>(items);
        this.ActiveMonsterIndex = 0;
    }
}
