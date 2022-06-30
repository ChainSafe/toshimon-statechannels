namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * State associated with a monster card that is part of a players hand in battle
 */
public record MonsterCard {

    [Parameter("uint256", "cardId", 1)]
    public uint CardId { get; set; }
    
    [Parameter("uint8", "mainType", 1)]
    public ToshimonType Type { get; set; }

    [Parameter("uint8", "secondaryType", 2)]
    public ToshimonType SecondaryType { get; set; }
    
    [Parameter("tuple", "baseStats", 2)]
    public Stats BaseStats { get; set; }
    
    [Parameter("tuple", "stats", 3)]
    public Stats Stats { get; set; }
    
    [Parameter("address[4]", "moves", 4)]
    public List<string> Moves { get; set; }

    // inflicted status conditions
    [Parameter("address", "statusCondition", 5)]
    public string StatusCondition { get; set; }

    [Parameter("uint8", "statusConditionCounter", 6)]
    public uint StatusConditionCounter { get; set; }

    // Special status conditions. These can be in play at the same time (e.g. flying while poisoned)
    [Parameter("address", "specialStatusCondition", 7)]
    public string SpecialStatusCondition { get; set; }

    [Parameter("uint8", "specialStatusConditionCounter", 8)]
    public uint SpecialStatusConditionCounter { get; set; }

    // Multi-turn move index and counter (could be combined to compress state)
    [Parameter("uint8", "activeMoveIndex", 9)]
    public uint ActiveMoveIndex { get; set; }

    [Parameter("uint8", "activeMoveCounter", 10)]
    public uint ActiveMoveCounter { get; set; } 

    public MonsterCard() { }

    public MonsterCard(Stats baseStats, Stats stats, List<string> moves) {
        this.CardId = 0;
        this.BaseStats = baseStats with {};
        this.Stats = stats with {};
        this.Moves = moves;
        this.StatusCondition = "0x0000000000000000000000000000000000000000";
        this.StatusConditionCounter = 0;
        this.SpecialStatusCondition = "0x0000000000000000000000000000000000000000";
        this.SpecialStatusConditionCounter = 0;
        this.ActiveMoveIndex = 0;
        this.ActiveMoveCounter = 0;
    }

    public MonsterCard(Stats baseStats, Stats stats, string[] moves) : this(baseStats, stats, new List<string>(moves)) { }
}
