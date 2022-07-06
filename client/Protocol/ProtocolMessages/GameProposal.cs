namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * A game proposal is a message published by a player advertising the fact they are looking for a game.
 * It contains all the information required for a second player to construct the fixed parameters and first state 
 * update in a state channel.
 *
 * This data structure is never submitted on-chain but still uses ABI encoding for consistency.
 */
public record GameProposal
{
	[Parameter("uint256", "chainId", 1)]
    public BigInteger ChainId { get; set; }

	[Parameter("uint48", "channelNonce", 2)]
    public ulong ChannelNonce { get; set; }

    [Parameter("address", "appDefinition", 3)]
    public string AppDefinition { get; set; }

    [Parameter("address", "signingKey", 4)]
    public string SigningKey { get; set; }

    [Parameter("address", "recoveryKey", 5)]
    public string RecoveryKey { get; set; }

    [Parameter("address", "recipient", 6)]
    public string Recipient { get; set; }

    [Parameter("address", "wagerAssetAddress", 7)]
    public string WagerAssetAddress { get; set; }

    [Parameter("uint256", "wagerAmount", 8)]
    public BigInteger WagerAmount { get; set; }

    [Parameter("uint48", "challengeDuration", 9)]
    public ulong ChallengeDuration { get; set; }

    [Parameter("tuple", "playerState", 10)]
    public PlayerState PlayerState { get; set; }

    public byte[] AbiEncode() {
        ABIEncode abiEncode = new ABIEncode();
        return abiEncode.GetABIParamsEncoded(this);
    }

    public static GameProposal AbiDecode(byte[] encoded) {
        var decoder = new ParameterDecoder();
        return (GameProposal) decoder.DecodeAttributes(encoded, typeof(GameProposal));
    }
}
