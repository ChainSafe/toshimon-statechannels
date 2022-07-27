namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * State associated with an card that is part of a players hand in battle
 */
[Struct("ItemCard")]
public record ItemCard {
    [Parameter("uint256", "cardId", 1)]
    public BigInteger CardId { get; set; }

    [Parameter("address", "definition", 2)]
    public string Definition { get; set; }
    
    [Parameter("bool", "used", 3)]
    public bool Used { get; set; }

    public byte[] AbiEncode() {
        ABIEncode abiEncode = new ABIEncode();
        return abiEncode.GetABIParamsEncoded(this);
    }
}
