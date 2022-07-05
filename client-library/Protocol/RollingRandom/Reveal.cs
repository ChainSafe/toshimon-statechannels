namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;

public record Reveal {
    [Parameter("uint8", "move", 1)]
    public byte Move { get; set; }

    [Parameter("uint256", "seed", 2)]
    public BigInteger Seed { get; set; }

    public Reveal() {
        Move = 0;
        Seed = 0;
    }

    public byte[] CommitHash {
        get {
            ABIEncode abiEncode = new ABIEncode();
            return abiEncode.GetSha3ABIEncoded(
                Move,
                Seed
            );
        }
    }

    public byte[] AbiEncode() {
        ABIEncode abiEncode = new ABIEncode();
        return abiEncode.GetABIParamsEncoded(this);
    }

    public static Reveal AbiDecode(byte[] encoded) {
        var decoder = new ParameterDecoder();
        return (Reveal) decoder.DecodeAttributes(encoded, typeof(Reveal));
    }
}
