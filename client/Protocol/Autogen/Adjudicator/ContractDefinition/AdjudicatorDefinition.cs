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

namespace Protocol.Adjudicator.ContractDefinition
{


    public partial class AdjudicatorDeployment : AdjudicatorDeploymentBase
    {
        public AdjudicatorDeployment() : base(BYTECODE) { }
        public AdjudicatorDeployment(string byteCode) : base(byteCode) { }
    }

    public class AdjudicatorDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "";
        public AdjudicatorDeploymentBase() : base(BYTECODE) { }
        public AdjudicatorDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class ChallengeFunction : ChallengeFunctionBase { }

    [Function("challenge")]
    public class ChallengeFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "fixedPart", 1)]
        public virtual FixedPart FixedPart { get; set; }
        [Parameter("tuple[]", "variableParts", 2)]
        public virtual List<VariablePart> VariableParts { get; set; }
        [Parameter("tuple[]", "sigs", 3)]
        public virtual List<Signature> Sigs { get; set; }
        [Parameter("uint8[]", "whoSignedWhat", 4)]
        public virtual List<byte> WhoSignedWhat { get; set; }
        [Parameter("tuple", "challengerSig", 5)]
        public virtual Signature ChallengerSig { get; set; }
    }

    public partial class CheckpointFunction : CheckpointFunctionBase { }

    [Function("checkpoint")]
    public class CheckpointFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "fixedPart", 1)]
        public virtual FixedPart FixedPart { get; set; }
        [Parameter("tuple[]", "variableParts", 2)]
        public virtual List<VariablePart> VariableParts { get; set; }
        [Parameter("tuple[]", "sigs", 3)]
        public virtual List<Signature> Sigs { get; set; }
        [Parameter("uint8[]", "whoSignedWhat", 4)]
        public virtual List<byte> WhoSignedWhat { get; set; }
    }

    public partial class Compute_transfer_effects_and_interactionsFunction : Compute_transfer_effects_and_interactionsFunctionBase { }

    [Function("compute_transfer_effects_and_interactions", typeof(Compute_transfer_effects_and_interactionsOutputDTO))]
    public class Compute_transfer_effects_and_interactionsFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "initialHoldings", 1)]
        public virtual BigInteger InitialHoldings { get; set; }
        [Parameter("tuple[]", "allocations", 2)]
        public virtual List<Allocation> Allocations { get; set; }
        [Parameter("uint256[]", "indices", 3)]
        public virtual List<BigInteger> Indices { get; set; }
    }

    public partial class ConcludeFunction : ConcludeFunctionBase { }

    [Function("conclude")]
    public class ConcludeFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "fixedPart", 1)]
        public virtual FixedPart FixedPart { get; set; }
        [Parameter("tuple", "latestVariablePart", 2)]
        public virtual VariablePart LatestVariablePart { get; set; }
        [Parameter("uint8", "numStates", 3)]
        public virtual byte NumStates { get; set; }
        [Parameter("uint8[]", "whoSignedWhat", 4)]
        public virtual List<byte> WhoSignedWhat { get; set; }
        [Parameter("tuple[]", "sigs", 5)]
        public virtual List<Signature> Sigs { get; set; }
    }

    public partial class ConcludeAndTransferAllAssetsFunction : ConcludeAndTransferAllAssetsFunctionBase { }

    [Function("concludeAndTransferAllAssets")]
    public class ConcludeAndTransferAllAssetsFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "fixedPart", 1)]
        public virtual FixedPart FixedPart { get; set; }
        [Parameter("tuple", "latestVariablePart", 2)]
        public virtual VariablePart LatestVariablePart { get; set; }
        [Parameter("uint8", "numStates", 3)]
        public virtual byte NumStates { get; set; }
        [Parameter("uint8[]", "whoSignedWhat", 4)]
        public virtual List<byte> WhoSignedWhat { get; set; }
        [Parameter("tuple[]", "sigs", 5)]
        public virtual List<Signature> Sigs { get; set; }
    }

    public partial class DepositFunction : DepositFunctionBase { }

    [Function("deposit")]
    public class DepositFunctionBase : FunctionMessage
    {
        [Parameter("address", "asset", 1)]
        public virtual string Asset { get; set; }
        [Parameter("bytes32", "channelId", 2)]
        public virtual byte[] ChannelId { get; set; }
        [Parameter("uint256", "expectedHeld", 3)]
        public virtual BigInteger ExpectedHeld { get; set; }
        [Parameter("uint256", "amount", 4)]
        public virtual BigInteger Amount { get; set; }
    }

    public partial class GetChainIDFunction : GetChainIDFunctionBase { }

    [Function("getChainID", "uint256")]
    public class GetChainIDFunctionBase : FunctionMessage
    {

    }

    public partial class HoldingsFunction : HoldingsFunctionBase { }

    [Function("holdings", "uint256")]
    public class HoldingsFunctionBase : FunctionMessage
    {
        [Parameter("address", "", 1)]
        public virtual string ReturnValue1 { get; set; }
        [Parameter("bytes32", "", 2)]
        public virtual byte[] ReturnValue2 { get; set; }
    }

    public partial class RequireValidInputFunction : RequireValidInputFunctionBase { }

    [Function("requireValidInput", "bool")]
    public class RequireValidInputFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "numParticipants", 1)]
        public virtual BigInteger NumParticipants { get; set; }
        [Parameter("uint256", "numStates", 2)]
        public virtual BigInteger NumStates { get; set; }
        [Parameter("uint256", "numSigs", 3)]
        public virtual BigInteger NumSigs { get; set; }
        [Parameter("uint256", "numWhoSignedWhats", 4)]
        public virtual BigInteger NumWhoSignedWhats { get; set; }
    }

    public partial class RespondFunction : RespondFunctionBase { }

    [Function("respond")]
    public class RespondFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "fixedPart", 1)]
        public virtual FixedPart FixedPart { get; set; }
        [Parameter("tuple[2]", "variablePartAB", 2)]
        public virtual List<VariablePart> VariablePartAB { get; set; }
        [Parameter("tuple", "sig", 3)]
        public virtual Signature Sig { get; set; }
    }

    public partial class StatusOfFunction : StatusOfFunctionBase { }

    [Function("statusOf", "bytes32")]
    public class StatusOfFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class TransferFunction : TransferFunctionBase { }

    [Function("transfer")]
    public class TransferFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "assetIndex", 1)]
        public virtual BigInteger AssetIndex { get; set; }
        [Parameter("bytes32", "fromChannelId", 2)]
        public virtual byte[] FromChannelId { get; set; }
        [Parameter("bytes", "outcomeBytes", 3)]
        public virtual byte[] OutcomeBytes { get; set; }
        [Parameter("bytes32", "stateHash", 4)]
        public virtual byte[] StateHash { get; set; }
        [Parameter("uint256[]", "indices", 5)]
        public virtual List<BigInteger> Indices { get; set; }
    }

    public partial class TransferAllAssetsFunction : TransferAllAssetsFunctionBase { }

    [Function("transferAllAssets")]
    public class TransferAllAssetsFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "channelId", 1)]
        public virtual byte[] ChannelId { get; set; }
        [Parameter("tuple[]", "outcome", 2)]
        public virtual List<SingleAssetExit> Outcome { get; set; }
        [Parameter("bytes32", "stateHash", 3)]
        public virtual byte[] StateHash { get; set; }
    }

    public partial class UnpackStatusFunction : UnpackStatusFunctionBase { }

    [Function("unpackStatus", typeof(UnpackStatusOutputDTO))]
    public class UnpackStatusFunctionBase : FunctionMessage
    {
        [Parameter("bytes32", "channelId", 1)]
        public virtual byte[] ChannelId { get; set; }
    }

    public partial class ValidTransitionFunction : ValidTransitionFunctionBase { }

    [Function("validTransition", "bool")]
    public class ValidTransitionFunctionBase : FunctionMessage
    {
        [Parameter("uint256", "nParticipants", 1)]
        public virtual BigInteger NParticipants { get; set; }
        [Parameter("tuple[2]", "ab", 2)]
        public virtual List<VariablePart> Ab { get; set; }
        [Parameter("address", "appDefinition", 3)]
        public virtual string AppDefinition { get; set; }
    }

    public partial class AllocationUpdatedEventDTO : AllocationUpdatedEventDTOBase { }

    [Event("AllocationUpdated")]
    public class AllocationUpdatedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "channelId", 1, true )]
        public virtual byte[] ChannelId { get; set; }
        [Parameter("uint256", "assetIndex", 2, false )]
        public virtual BigInteger AssetIndex { get; set; }
        [Parameter("uint256", "initialHoldings", 3, false )]
        public virtual BigInteger InitialHoldings { get; set; }
    }

    public partial class ChallengeClearedEventDTO : ChallengeClearedEventDTOBase { }

    [Event("ChallengeCleared")]
    public class ChallengeClearedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "channelId", 1, true )]
        public virtual byte[] ChannelId { get; set; }
        [Parameter("uint48", "newTurnNumRecord", 2, false )]
        public virtual ulong NewTurnNumRecord { get; set; }
    }

    public partial class ChallengeRegisteredEventDTO : ChallengeRegisteredEventDTOBase { }

    [Event("ChallengeRegistered")]
    public class ChallengeRegisteredEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "channelId", 1, true )]
        public virtual byte[] ChannelId { get; set; }
        [Parameter("uint48", "turnNumRecord", 2, false )]
        public virtual ulong TurnNumRecord { get; set; }
        [Parameter("uint48", "finalizesAt", 3, false )]
        public virtual ulong FinalizesAt { get; set; }
        [Parameter("bool", "isFinal", 4, false )]
        public virtual bool IsFinal { get; set; }
        [Parameter("tuple", "fixedPart", 5, false )]
        public virtual FixedPart FixedPart { get; set; }
        [Parameter("tuple[]", "variableParts", 6, false )]
        public virtual List<VariablePart> VariableParts { get; set; }
        [Parameter("tuple[]", "sigs", 7, false )]
        public virtual List<Signature> Sigs { get; set; }
        [Parameter("uint8[]", "whoSignedWhat", 8, false )]
        public virtual List<byte> WhoSignedWhat { get; set; }
    }

    public partial class ConcludedEventDTO : ConcludedEventDTOBase { }

    [Event("Concluded")]
    public class ConcludedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "channelId", 1, true )]
        public virtual byte[] ChannelId { get; set; }
        [Parameter("uint48", "finalizesAt", 2, false )]
        public virtual ulong FinalizesAt { get; set; }
    }

    public partial class DepositedEventDTO : DepositedEventDTOBase { }

    [Event("Deposited")]
    public class DepositedEventDTOBase : IEventDTO
    {
        [Parameter("bytes32", "destination", 1, true )]
        public virtual byte[] Destination { get; set; }
        [Parameter("address", "asset", 2, false )]
        public virtual string Asset { get; set; }
        [Parameter("uint256", "amountDeposited", 3, false )]
        public virtual BigInteger AmountDeposited { get; set; }
        [Parameter("uint256", "destinationHoldings", 4, false )]
        public virtual BigInteger DestinationHoldings { get; set; }
    }





    public partial class Compute_transfer_effects_and_interactionsOutputDTO : Compute_transfer_effects_and_interactionsOutputDTOBase { }

    [FunctionOutput]
    public class Compute_transfer_effects_and_interactionsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("tuple[]", "newAllocations", 1)]
        public virtual List<Allocation> NewAllocations { get; set; }
        [Parameter("bool", "allocatesOnlyZeros", 2)]
        public virtual bool AllocatesOnlyZeros { get; set; }
        [Parameter("tuple[]", "exitAllocations", 3)]
        public virtual List<Allocation> ExitAllocations { get; set; }
        [Parameter("uint256", "totalPayouts", 4)]
        public virtual BigInteger TotalPayouts { get; set; }
    }







    public partial class GetChainIDOutputDTO : GetChainIDOutputDTOBase { }

    [FunctionOutput]
    public class GetChainIDOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class HoldingsOutputDTO : HoldingsOutputDTOBase { }

    [FunctionOutput]
    public class HoldingsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint256", "", 1)]
        public virtual BigInteger ReturnValue1 { get; set; }
    }

    public partial class RequireValidInputOutputDTO : RequireValidInputOutputDTOBase { }

    [FunctionOutput]
    public class RequireValidInputOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }



    public partial class StatusOfOutputDTO : StatusOfOutputDTOBase { }

    [FunctionOutput]
    public class StatusOfOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes32", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }





    public partial class UnpackStatusOutputDTO : UnpackStatusOutputDTOBase { }

    [FunctionOutput]
    public class UnpackStatusOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("uint48", "turnNumRecord", 1)]
        public virtual ulong TurnNumRecord { get; set; }
        [Parameter("uint48", "finalizesAt", 2)]
        public virtual ulong FinalizesAt { get; set; }
        [Parameter("uint160", "fingerprint", 3)]
        public virtual BigInteger Fingerprint { get; set; }
    }

    public partial class ValidTransitionOutputDTO : ValidTransitionOutputDTOBase { }

    [FunctionOutput]
    public class ValidTransitionOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bool", "", 1)]
        public virtual bool ReturnValue1 { get; set; }
    }
}
