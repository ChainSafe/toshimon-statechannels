namespace Protocol;

using System;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.ABI.FunctionEncoding.Attributes;

/**
 * 
 */
public record ClaimArgs
{
    [Parameter("bytes32", "sourceChannelId", 1)]
    public virtual byte[] SourceChannelId { get; set; }
    [Parameter("bytes32", "sourceStateHash", 2)]
    public virtual byte[] SourceStateHash { get; set; }
    [Parameter("bytes", "sourceOutcomeBytes", 3)]
    public virtual byte[] SourceOutcomeBytes { get; set; }
    [Parameter("uint256", "sourceAssetIndex", 4)]
    public virtual BigInteger SourceAssetIndex { get; set; }
    [Parameter("uint256", "indexOfTargetInSource", 5)]
    public virtual BigInteger IndexOfTargetInSource { get; set; }
    [Parameter("bytes32", "targetStateHash", 6)]
    public virtual byte[] TargetStateHash { get; set; }
    [Parameter("bytes", "targetOutcomeBytes", 7)]
    public virtual byte[] TargetOutcomeBytes { get; set; }
    [Parameter("uint256", "targetAssetIndex", 8)]
    public virtual BigInteger TargetAssetIndex { get; set; }
    [Parameter("uint256[]", "targetAllocationIndicesToPayout", 9)]
    public virtual List<BigInteger> TargetAllocationIndicesToPayout { get; set; }
}
