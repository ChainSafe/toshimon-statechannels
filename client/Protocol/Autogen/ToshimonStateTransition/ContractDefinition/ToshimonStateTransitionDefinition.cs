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

namespace Protocol.ToshimonStateTransition.ContractDefinition
{


    public partial class ToshimonStateTransitionDeployment : ToshimonStateTransitionDeploymentBase
    {
        public ToshimonStateTransitionDeployment() : base(BYTECODE) { }
        public ToshimonStateTransitionDeployment(string byteCode) : base(byteCode) { }
    }

    public class ToshimonStateTransitionDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "";
        public ToshimonStateTransitionDeploymentBase() : base(BYTECODE) { }
        public ToshimonStateTransitionDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class AdvanceStateFunction : AdvanceStateFunctionBase { }

    [Function("advanceState", typeof(AdvanceStateOutputDTO))]
    public class AdvanceStateFunctionBase : FunctionMessage
    {
        [Parameter("bytes", "_gameState_", 1)]
        public virtual byte[] GameState_ { get; set; }
        [Parameter("tuple[]", "outcome", 2)]
        public virtual List<SingleAssetExit> Outcome { get; set; }
        [Parameter("uint8", "moveA", 3)]
        public virtual byte MoveA { get; set; }
        [Parameter("uint8", "moveB", 4)]
        public virtual byte MoveB { get; set; }
        [Parameter("bytes32", "randomSeed", 5)]
        public virtual byte[] RandomSeed { get; set; }
    }

    public partial class AdvanceStateTypedFunction : AdvanceStateTypedFunctionBase { }

    [Function("advanceStateTyped", typeof(AdvanceStateTypedOutputDTO))]
    public class AdvanceStateTypedFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "gameState", 1)]
        public virtual GameState GameState { get; set; }
        [Parameter("tuple[]", "outcome", 2)]
        public virtual List<SingleAssetExit> Outcome { get; set; }
        [Parameter("uint8", "moveA", 3)]
        public virtual byte MoveA { get; set; }
        [Parameter("uint8", "moveB", 4)]
        public virtual byte MoveB { get; set; }
        [Parameter("bytes32", "randomSeed", 5)]
        public virtual byte[] RandomSeed { get; set; }
    }

    public partial class UpdateOutcomeFavourPlayerFunction : UpdateOutcomeFavourPlayerFunctionBase { }

    [Function("updateOutcomeFavourPlayer", typeof(UpdateOutcomeFavourPlayerOutputDTO))]
    public class UpdateOutcomeFavourPlayerFunctionBase : FunctionMessage
    {
        [Parameter("tuple[]", "outcome", 1)]
        public virtual List<SingleAssetExit> Outcome { get; set; }
        [Parameter("uint8", "playerIndex", 2)]
        public virtual byte PlayerIndex { get; set; }
    }

    public partial class ValidTransitionFunction : ValidTransitionFunctionBase { }

    [Function("validTransition", "bool")]
    public class ValidTransitionFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "prev", 1)]
        public virtual VariablePart Prev { get; set; }
        [Parameter("tuple", "next", 2)]
        public virtual VariablePart Next { get; set; }
        [Parameter("uint256", "nParticipants", 3)]
        public virtual BigInteger NParticipants { get; set; }
    }

    public partial class AdvanceStateOutputDTO : AdvanceStateOutputDTOBase { }

    [FunctionOutput]
    public class AdvanceStateOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
        [Parameter("tuple[]", "", 2)]
        public virtual List<SingleAssetExit> ReturnValue2 { get; set; }
        [Parameter("bool", "", 3)]
        public virtual bool ReturnValue3 { get; set; }
    }

    public partial class AdvanceStateTypedOutputDTO : AdvanceStateTypedOutputDTOBase { }

    [FunctionOutput]
    public class AdvanceStateTypedOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("tuple", "", 1)]
        public virtual GameState ReturnValue1 { get; set; }
        [Parameter("tuple[]", "", 2)]
        public virtual List<SingleAssetExit> ReturnValue2 { get; set; }
        [Parameter("bool", "", 3)]
        public virtual bool ReturnValue3 { get; set; }
    }

    public partial class UpdateOutcomeFavourPlayerOutputDTO : UpdateOutcomeFavourPlayerOutputDTOBase { }

    [FunctionOutput]
    public class UpdateOutcomeFavourPlayerOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("tuple[]", "", 1)]
        public virtual List<SingleAssetExit> ReturnValue1 { get; set; }
    }

    public partial class ValidTransitionOutputDTO : ValidTransitionOutputDTOBase { }

    [FunctionOutput]
    public class ValidTransitionOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }
}
