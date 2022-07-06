namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * 
 */
public record SignedStateUpdate
{
	[Parameter("tuple", "variablePart", 1)]
    public VariablePart VariablePart { get; set; }

    [Parameter("tuple", "signature", 2)]
    public Signature Signature { get; set; }

    public byte[] AbiEncode() {
        ABIEncode abiEncode = new ABIEncode();
        return abiEncode.GetABIParamsEncoded(this);
    }

    public static SignedStateUpdate AbiDecode(byte[] encoded) {
        var decoder = new ParameterDecoder();
        return (SignedStateUpdate) decoder.DecodeAttributes(encoded, typeof(SignedStateUpdate));
    }
}
