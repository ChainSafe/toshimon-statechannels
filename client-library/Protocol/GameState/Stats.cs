namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * Stats belonging to a monster card when in play.
 * These can either represent the base stats which are immutable during a game
 * or the current stats which can be buffed/debuffed
 */
public record Stats {
    [Parameter("uint8", "hp", 3)]
    public uint Hp { get; set; }

    [Parameter("uint8", "attack", 4)]
    public uint Attack { get; set; }

    [Parameter("uint8", "defense", 5)]
    public uint Defense { get; set; }

    [Parameter("uint8", "spAttack", 6)]
    public uint SpAttack { get; set; }
    
    [Parameter("uint8", "spDefense", 7)]
    public uint SpDefense { get; set; }
    
    [Parameter("uint8", "speed", 8)]
    public uint Speed { get; set; }

    [Parameter("uint8[4]", "moves", 10)]
    public List<uint> PP { get; set; }
}
