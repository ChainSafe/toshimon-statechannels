namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * State associated with an card that is part of a players hand in battle
 */
public record ItemCard {
    [Parameter("uint8", "cardId", 1)]
    public uint CardId { get; set; }

    [Parameter("address", "definition", 2)]
    public string Definition { get; set; }
    
    [Parameter("bool", "used", 3)]
    public bool Used { get; set; }
}
