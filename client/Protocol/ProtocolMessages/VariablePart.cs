namespace Protocol;

using System;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Signer;

/**
 * Component of a channel state update that is contant between all updates.
 * As such it only needs to be passed in once per challenge and fields can be ommitted 
 * in the state updates and replaced with the ChannelId instead
 */
public record VariablePart
{
    [Parameter("tuple[]", "outcome", 1)]
    public List<SingleAssetExit> Outcome { get; set; }

    [Parameter("bytes", "appData", 2)]
    public byte[] AppData { get; set; }

    [Parameter("uint48", "turnNum", 3)]
    public ulong TurnNum { get; set; }

    [Parameter("bool", "isFinal", 4)]
    public bool IsFinal { get; set; }

    public SignedVariablePart Sign(FixedPart fixedPart, EthECKey signer) {
        Signature sig = new StateUpdate(fixedPart, this).Sign(signer);
        return new SignedVariablePart(this, sig);
    }

    public Signature GetChallengeSignature(FixedPart fixedPart, EthECKey signer) {
        return new StateUpdate(fixedPart, this).GetChallengeSignature(signer);
    }
}
