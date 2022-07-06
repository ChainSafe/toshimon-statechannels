namespace Protocol;

using System;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * Encoding of a signature supported by ForceMove
 */
public record Signature
{
    [Parameter("uint8", "v", 1)]
    public byte V { get; set; }
    [Parameter("bytes32", "r", 2)]
    public byte[] R { get; set; }
    [Parameter("bytes32", "s", 3)]
    public byte[] S { get; set; }
}
