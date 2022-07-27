namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;
/**
 * State of one player at an instant in time
 */
[Struct("PlayerState")]
public record PlayerState {
    [Parameter("tuple[5]", "monsters", 1)]
    public List<MonsterCard> Monsters { get; set; }

    [Parameter("tuple[5]", "items", 2)]
    public List<ItemCard> Items { get; set; }

    [Parameter("uint8", "activeMonsterIndex", 3)]
    public byte ActiveMonsterIndex { get; set; }

    public PlayerState() {}

    public PlayerState(MonsterCard[] monsters, ItemCard[] items) {
        this.Monsters = new List<MonsterCard>(monsters);
        this.Items = new List<ItemCard>(items);
        this.ActiveMonsterIndex = 0;
    }

    public byte[] AbiEncode() {
        ABIEncode abiEncode = new ABIEncode();
        return abiEncode.GetABIParamsEncoded(this);
    }
}
