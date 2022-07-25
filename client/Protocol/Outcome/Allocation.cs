namespace Protocol;

using System;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Hex.HexConvertors.Extensions;

/**
 * An allocation tells a Nitro state channel how to distribute one part of one asset to a 
 * single destination
 */
public record Allocation
{
    [Parameter("bytes32", "destination", 1)]
    public byte[] Destination { get; set; }

    [Parameter("uint256", "amount", 2)]
    public BigInteger Amount { get; set; }

    [Parameter("uint8", "allocationType", 3)]
    public byte AllocationType { get; set; }

    [Parameter("bytes", "metadata", 4)]
    public byte[] Metadata { get; set; }

    public static Allocation NewSimple(string destination, BigInteger amount) {
        return new Allocation() {
            Destination = destination.HexToByteArray(),
            Amount = amount,
            AllocationType = 0, // AllocationType.Simple
            Metadata = new byte[]{ 0x00 }
        };
    }
}
