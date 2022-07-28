namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * State associated with a monster card that is part of a players hand in battle
 */
[Struct("MonsterCard")]
public record MonsterCard {

    [Parameter("uint256", "cardId", 1)]
    public BigInteger CardId { get; set; }
    
    [Parameter("uint8", "mainType", 2)]
    public ToshimonType Type { get; set; } // ??

    [Parameter("uint8", "secondaryType", 3)]
    public ToshimonType SecondaryType { get; set; } //??
    
    [Parameter("tuple", "baseStats", 4)]
    public Stats BaseStats { get; set; }
    
    [Parameter("tuple", "stats", 5)]
    public Stats Stats { get; set; }
    
    [Parameter("address[4]", "moves", 6)]
    public List<string> Moves { get; set; }

    // inflicted status conditions
    [Parameter("address", "statusCondition", 7)]
    public string StatusCondition { get; set; }

    [Parameter("uint8", "statusConditionCounter", 8)]
    public byte StatusConditionCounter { get; set; }

    // Special status conditions. These can be in play at the same time (e.g. flying while poisoned)
    [Parameter("address", "specialStatusCondition", 9)]
    public string SpecialStatusCondition { get; set; }

    [Parameter("uint8", "specialStatusConditionCounter", 10)]
    public byte SpecialStatusConditionCounter { get; set; }

    // Multi-turn move index and counter (could be combined to compress state)
    [Parameter("uint8", "activeMoveIndex", 11)]
    public byte ActiveMoveIndex { get; set; }

    [Parameter("uint8", "activeMoveCounter", 12)]
    public byte ActiveMoveCounter { get; set; } 

    public MonsterCard() { }

    // This is the same as no monster card and will be ignored by any client and state transition code
    // Zeros as many fields as possible to save calldata gas
    public static MonsterCard Default() {
        return new MonsterCard() {
            CardId = 0,
            BaseStats = Stats.Default,
            Stats = Stats.Default,
            Moves = Enumerable.Repeat("0x0000000000000000000000000000000000000000", 4).ToList(),
            StatusCondition = "0x0000000000000000000000000000000000000000",
            StatusConditionCounter = 0,
            SpecialStatusCondition = "0x0000000000000000000000000000000000000000",
            SpecialStatusConditionCounter = 0,
            ActiveMoveIndex = 0,
            ActiveMoveCounter = 0,           
        };
    }

    public byte[] AbiEncode() {
        ABIEncode abiEncode = new ABIEncode();
        return abiEncode.GetABIParamsEncoded(this);
    }
}
