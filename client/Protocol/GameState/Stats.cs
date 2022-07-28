namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * Stats belonging to a monster card when in play.
 * These can either represent the base stats which are immutable during a game
 * or the current stats which can be buffed/debuffed
 */
[Struct("Stats")]
public record Stats {
    [Parameter("uint8", "hp", 1)]
    public byte Hp { get; set; }

    [Parameter("uint8", "attack", 2)]
    public byte Attack { get; set; }

    [Parameter("uint8", "defense", 3)]
    public byte Defense { get; set; }

    [Parameter("uint8", "spAttack", 4)]
    public byte SpAttack { get; set; }
    
    [Parameter("uint8", "spDefense", 5)]
    public byte SpDefense { get; set; }
    
    [Parameter("uint8", "speed", 6)]
    public byte Speed { get; set; }

    [Parameter("uint8[4]", "pp", 7)]
    public List<uint> PP { get; set; }

    public static readonly Stats Default = new Stats() {
        PP = new List<uint>{0, 0, 0, 0}
    };

    public byte[] AbiEncode() {
        ABIEncode abiEncode = new ABIEncode();
        return abiEncode.GetABIParamsEncoded(this);
    }
}
