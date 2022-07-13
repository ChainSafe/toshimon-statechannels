namespace Protocol;

using System;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
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

    // signedBy is a bitmask field. E.g for a 2 participant channel
    // 0b00 = 0 : No-one
    // 0b01 = 1 : participant 0
    // 0b10 = 2 : participant 1
    // 0b11 = 3 : participant 0 and participant 1
    [Parameter("uint256", "signedBy", 3)]
    public BigInteger SignedBy { get; set; }

    // create a new SignedVariable part from a variable part, a state update signature and the index of the signer (0 or 1)
    public SignedVariablePart(VariablePart vp, Signature sig) {
        VariablePart = vp;
        Sigs = new List<Signature>();
        Sigs.Add(sig);
        ulong signer = vp.TurnNum % 2; // always 2 players in this game
        SignedBy = signer == 0 ? 0b01 : 0b10;
    }

    public byte[] AbiEncode() {
        ABIEncode abiEncode = new ABIEncode();
        return abiEncode.GetABIParamsEncoded(this);
    }

    public static SignedVariablePart AbiDecode(byte[] encoded) {
        var decoder = new ParameterDecoder();
        return (SignedVariablePart) decoder.DecodeAttributes(encoded, typeof(SignedVariablePart));
    }
}
