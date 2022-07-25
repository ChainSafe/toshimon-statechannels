using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts;
using System.Threading;

namespace Protocol.TESTNitroUtils.ContractDefinition
{


    public partial class TESTNitroUtilsDeployment : TESTNitroUtilsDeploymentBase
    {
        public TESTNitroUtilsDeployment() : base(BYTECODE) { }
        public TESTNitroUtilsDeployment(string byteCode) : base(byteCode) { }
    }

    public class TESTNitroUtilsDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "";
        public TESTNitroUtilsDeploymentBase() : base(BYTECODE) { }
        public TESTNitroUtilsDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class GetChannelIdFunction : GetChannelIdFunctionBase { }

    [Function("getChannelId", "bytes32")]
    public class GetChannelIdFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "fixedPart", 1)]
        public virtual FixedPart FixedPart { get; set; }
    }

    public partial class GetClaimedSignersIndicesFunction : GetClaimedSignersIndicesFunctionBase { }

    [Function("getClaimedSignersIndices", "uint8[]")]
    public class GetClaimedSignersIndicesFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "signedBy", 1)]
        public virtual BigInteger SignedBy { get; set; }
    }

    public partial class GetClaimedSignersNumFunction : GetClaimedSignersNumFunctionBase { }

    [Function("getClaimedSignersNum", "uint8")]
    public class GetClaimedSignersNumFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "signedBy", 1)]
        public virtual BigInteger SignedBy { get; set; }
    }

    public partial class HashStateFunction : HashStateFunctionBase { }

    [Function("hashState", "bytes32")]
    public class HashStateFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "channelId", 1)]
        public virtual byte[] ChannelId { get; set; }
        [Parameter("bytes", "appData", 2)]
        public virtual byte[] AppData { get; set; }
        [Parameter("tuple[]", "outcome", 3)]
        public virtual List<SingleAssetExit> Outcome { get; set; }
        [Parameter("uint48", "turnNum", 4)]
        public virtual ulong TurnNum { get; set; }
        [Parameter("bool", "isFinal", 5)]
        public virtual bool IsFinal { get; set; }
    }

    public partial class IsClaimedSignedByFunction : IsClaimedSignedByFunctionBase { }

    [Function("isClaimedSignedBy", "bool")]
    public class IsClaimedSignedByFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "signedBy", 1)]
        public virtual BigInteger SignedBy { get; set; }
        [Parameter("uint8", "participantIndex", 2)]
        public virtual byte ParticipantIndex { get; set; }
    }

    public partial class IsClaimedSignedOnlyByFunction : IsClaimedSignedOnlyByFunctionBase { }

    [Function("isClaimedSignedOnlyBy", "bool")]
    public class IsClaimedSignedOnlyByFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "signedBy", 1)]
        public virtual BigInteger SignedBy { get; set; }
        [Parameter("uint8", "participantIndex", 2)]
        public virtual byte ParticipantIndex { get; set; }
    }

    public partial class RecoverSignerFunction : RecoverSignerFunctionBase { }

    [Function("recoverSigner", "address")]
    public class RecoverSignerFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "_d", 1)]
        public virtual byte[] D { get; set; }
        [Parameter("tuple", "sig", 2)]
        public virtual Signature Sig { get; set; }
    }

    public partial class GetChannelIdOutputDTO : GetChannelIdOutputDTOBase { }

    [FunctionOutput]
    public class GetChannelIdOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "channelId", 1)]
        public virtual byte[] ChannelId { get; set; }
    }

    public partial class GetClaimedSignersIndicesOutputDTO : GetClaimedSignersIndicesOutputDTOBase { }

    [FunctionOutput]
    public class GetClaimedSignersIndicesOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint8[]", "", 1)]
        public virtual List<byte> ReturnValue1 { get; set; }
    }

    public partial class GetClaimedSignersNumOutputDTO : GetClaimedSignersNumOutputDTOBase { }

    [FunctionOutput]
    public class GetClaimedSignersNumOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint8", "", 1)]
        public virtual byte ReturnValue1 { get; set; }
    }

    public partial class HashStateOutputDTO : HashStateOutputDTOBase { }

    [FunctionOutput]
    public class HashStateOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class IsClaimedSignedByOutputDTO : IsClaimedSignedByOutputDTOBase { }

    [FunctionOutput]
    public class IsClaimedSignedByOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    public partial class IsClaimedSignedOnlyByOutputDTO : IsClaimedSignedOnlyByOutputDTOBase { }

    [FunctionOutput]
    public class IsClaimedSignedOnlyByOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }

    public partial class RecoverSignerOutputDTO : RecoverSignerOutputDTOBase { }

    [FunctionOutput]
    public class RecoverSignerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
    }
}
