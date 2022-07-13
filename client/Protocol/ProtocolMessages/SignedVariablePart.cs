namespace Protocol;

using System;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * A signed variable part of a state
 */
public record SignedVariablePart
{
    [Parameter("tuple", "variablePart", 1)]
    public VariablePart VariablePart { get; set; }

    [Parameter("tuple[]", "sigs", 2)]
    public List<Signature> Sigs { get; set; }

    [Parameter("uint256", "signedBy", 3)]
    public BigInteger SignedBy { get; set; }
}
