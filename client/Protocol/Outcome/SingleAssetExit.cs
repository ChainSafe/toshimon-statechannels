namespace Protocol;

using System;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * A single asset exit tells a Nitro state channel how
 * to distribute a single asset across multiple destinations.
 * Each destination can have a different amount
 */
public record SingleAssetExit
{
    [Parameter("address", "asset", 1)]
    public string Asset { get; set; }

    [Parameter("bytes", "metadata", 2)]
    public byte[] Metadata { get; set; }

    [Parameter("tuple[]", "allocations", 3)]
    public List<Allocation> Allocations { get; set; }

    public static SingleAssetExit NewSimpleNative(string destination, BigInteger amount) {
        return new SingleAssetExit {
            Asset = "0x0000000000000000000000000000000000000000",
            Metadata = new byte[] { 0x00 },
            Allocations = new List<Allocation>() { Allocation.NewSimple(destination, amount) },
        };
    }
}