namespace Protocol;

using System;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * Component of a channel state update that is contant between all updates.
 * As such it only needs to be passed in once per challenge and fields can be ommitted 
 * in the state updates and replaced with the ChannelId instead
 */
public record FixedPart
{
    [Parameter("uint256", "chainId", 1)]
    public BigInteger ChainId { get; set; }

    [Parameter("address[]", "participants", 2)]
    public List<string> Participants { get; set; }

    [Parameter("uint48", "channelNonce", 3)]
    public ulong ChannelNonce { get; set; }

    [Parameter("address", "appDefinition", 4)]
    public string AppDefinition { get; set; }

    [Parameter("uint48", "challengeDuration", 5)]
    public ulong ChallengeDuration { get; set; }

    // This is a derived field that compresses a FixedPart of a state channel into a unique ID
    // see https://github.com/statechannels/go-nitro/blob/7bf69da2e9df58b9fddf8d02158449667223c8d2/nitro-protocol/contracts/ForceMove.sol#L775
    public byte[] ChannelId { 
        get {
            ABIEncode abiEncode = new ABIEncode();
            return abiEncode.GetSha3ABIEncoded(
                new ABIValue("uint256", ChainId),
                new ABIValue("address[]", Participants),
                new ABIValue("uint48", ChannelNonce),
                new ABIValue("address", AppDefinition),
                new ABIValue("uint48", ChallengeDuration)
            );
        } 
    }

    public byte[] AbiEncode() {
        ABIEncode abiEncode = new ABIEncode();
        return abiEncode.GetABIParamsEncoded(this);
    }

    public static FixedPart AbiDecode(byte[] encoded) {
        var decoder = new ParameterDecoder();
        return (FixedPart) decoder.DecodeAttributes(encoded, typeof(FixedPart));
    }
}
