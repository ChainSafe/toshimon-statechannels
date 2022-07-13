using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using Protocol.Adjudicator.ContractDefinition;

namespace Protocol.Adjudicator.Service
{
    public partial class AdjudicatorService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, AdjudicatorDeployment adjudicatorDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<AdjudicatorDeployment>().SendRequestAndWaitForReceiptAsync(adjudicatorDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, AdjudicatorDeployment adjudicatorDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<AdjudicatorDeployment>().SendRequestAsync(adjudicatorDeployment);
        }

        public static async Task<AdjudicatorService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, AdjudicatorDeployment adjudicatorDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, adjudicatorDeployment, cancellationTokenSource);
            return new AdjudicatorService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public AdjudicatorService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> ChallengeRequestAsync(ChallengeFunction challengeFunction)
        {
             return ContractHandler.SendRequestAsync(challengeFunction);
        }

        public Task<TransactionReceipt> ChallengeRequestAndWaitForReceiptAsync(ChallengeFunction challengeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(challengeFunction, cancellationToken);
        }

        public Task<string> ChallengeRequestAsync(FixedPart fixedPart, List<SignedVariablePart> signedVariableParts, Signature challengerSig)
        {
            var challengeFunction = new ChallengeFunction();
                challengeFunction.FixedPart = fixedPart;
                challengeFunction.SignedVariableParts = signedVariableParts;
                challengeFunction.ChallengerSig = challengerSig;
            
             return ContractHandler.SendRequestAsync(challengeFunction);
        }

        public Task<TransactionReceipt> ChallengeRequestAndWaitForReceiptAsync(FixedPart fixedPart, List<SignedVariablePart> signedVariableParts, Signature challengerSig, CancellationTokenSource cancellationToken = null)
        {
            var challengeFunction = new ChallengeFunction();
                challengeFunction.FixedPart = fixedPart;
                challengeFunction.SignedVariableParts = signedVariableParts;
                challengeFunction.ChallengerSig = challengerSig;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(challengeFunction, cancellationToken);
        }

        public Task<string> CheckpointRequestAsync(CheckpointFunction checkpointFunction)
        {
             return ContractHandler.SendRequestAsync(checkpointFunction);
        }

        public Task<TransactionReceipt> CheckpointRequestAndWaitForReceiptAsync(CheckpointFunction checkpointFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(checkpointFunction, cancellationToken);
        }

        public Task<string> CheckpointRequestAsync(FixedPart fixedPart, List<SignedVariablePart> signedVariableParts)
        {
            var checkpointFunction = new CheckpointFunction();
                checkpointFunction.FixedPart = fixedPart;
                checkpointFunction.SignedVariableParts = signedVariableParts;
            
             return ContractHandler.SendRequestAsync(checkpointFunction);
        }

        public Task<TransactionReceipt> CheckpointRequestAndWaitForReceiptAsync(FixedPart fixedPart, List<SignedVariablePart> signedVariableParts, CancellationTokenSource cancellationToken = null)
        {
            var checkpointFunction = new CheckpointFunction();
                checkpointFunction.FixedPart = fixedPart;
                checkpointFunction.SignedVariableParts = signedVariableParts;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(checkpointFunction, cancellationToken);
        }

        public Task<string> ClaimRequestAsync(ClaimFunction claimFunction)
        {
             return ContractHandler.SendRequestAsync(claimFunction);
        }

        public Task<TransactionReceipt> ClaimRequestAndWaitForReceiptAsync(ClaimFunction claimFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(claimFunction, cancellationToken);
        }

        public Task<string> ClaimRequestAsync(ClaimArgs claimArgs)
        {
            var claimFunction = new ClaimFunction();
                claimFunction.ClaimArgs = claimArgs;
            
             return ContractHandler.SendRequestAsync(claimFunction);
        }

        public Task<TransactionReceipt> ClaimRequestAndWaitForReceiptAsync(ClaimArgs claimArgs, CancellationTokenSource cancellationToken = null)
        {
            var claimFunction = new ClaimFunction();
                claimFunction.ClaimArgs = claimArgs;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(claimFunction, cancellationToken);
        }

        public Task<Compute_claim_effects_and_interactionsOutputDTO> Compute_claim_effects_and_interactionsQueryAsync(Compute_claim_effects_and_interactionsFunction compute_claim_effects_and_interactionsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<Compute_claim_effects_and_interactionsFunction, Compute_claim_effects_and_interactionsOutputDTO>(compute_claim_effects_and_interactionsFunction, blockParameter);
        }

        public Task<Compute_claim_effects_and_interactionsOutputDTO> Compute_claim_effects_and_interactionsQueryAsync(BigInteger initialHoldings, List<Allocation> sourceAllocations, List<Allocation> targetAllocations, BigInteger indexOfTargetInSource, List<BigInteger> targetAllocationIndicesToPayout, BlockParameter blockParameter = null)
        {
            var compute_claim_effects_and_interactionsFunction = new Compute_claim_effects_and_interactionsFunction();
                compute_claim_effects_and_interactionsFunction.InitialHoldings = initialHoldings;
                compute_claim_effects_and_interactionsFunction.SourceAllocations = sourceAllocations;
                compute_claim_effects_and_interactionsFunction.TargetAllocations = targetAllocations;
                compute_claim_effects_and_interactionsFunction.IndexOfTargetInSource = indexOfTargetInSource;
                compute_claim_effects_and_interactionsFunction.TargetAllocationIndicesToPayout = targetAllocationIndicesToPayout;
            
            return ContractHandler.QueryDeserializingToObjectAsync<Compute_claim_effects_and_interactionsFunction, Compute_claim_effects_and_interactionsOutputDTO>(compute_claim_effects_and_interactionsFunction, blockParameter);
        }

        public Task<Compute_transfer_effects_and_interactionsOutputDTO> Compute_transfer_effects_and_interactionsQueryAsync(Compute_transfer_effects_and_interactionsFunction compute_transfer_effects_and_interactionsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<Compute_transfer_effects_and_interactionsFunction, Compute_transfer_effects_and_interactionsOutputDTO>(compute_transfer_effects_and_interactionsFunction, blockParameter);
        }

        public Task<Compute_transfer_effects_and_interactionsOutputDTO> Compute_transfer_effects_and_interactionsQueryAsync(BigInteger initialHoldings, List<Allocation> allocations, List<BigInteger> indices, BlockParameter blockParameter = null)
        {
            var compute_transfer_effects_and_interactionsFunction = new Compute_transfer_effects_and_interactionsFunction();
                compute_transfer_effects_and_interactionsFunction.InitialHoldings = initialHoldings;
                compute_transfer_effects_and_interactionsFunction.Allocations = allocations;
                compute_transfer_effects_and_interactionsFunction.Indices = indices;
            
            return ContractHandler.QueryDeserializingToObjectAsync<Compute_transfer_effects_and_interactionsFunction, Compute_transfer_effects_and_interactionsOutputDTO>(compute_transfer_effects_and_interactionsFunction, blockParameter);
        }

        public Task<string> ConcludeRequestAsync(ConcludeFunction concludeFunction)
        {
             return ContractHandler.SendRequestAsync(concludeFunction);
        }

        public Task<TransactionReceipt> ConcludeRequestAndWaitForReceiptAsync(ConcludeFunction concludeFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(concludeFunction, cancellationToken);
        }

        public Task<string> ConcludeRequestAsync(FixedPart fixedPart, List<SignedVariablePart> signedVariableParts)
        {
            var concludeFunction = new ConcludeFunction();
                concludeFunction.FixedPart = fixedPart;
                concludeFunction.SignedVariableParts = signedVariableParts;
            
             return ContractHandler.SendRequestAsync(concludeFunction);
        }

        public Task<TransactionReceipt> ConcludeRequestAndWaitForReceiptAsync(FixedPart fixedPart, List<SignedVariablePart> signedVariableParts, CancellationTokenSource cancellationToken = null)
        {
            var concludeFunction = new ConcludeFunction();
                concludeFunction.FixedPart = fixedPart;
                concludeFunction.SignedVariableParts = signedVariableParts;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(concludeFunction, cancellationToken);
        }

        public Task<string> ConcludeAndTransferAllAssetsRequestAsync(ConcludeAndTransferAllAssetsFunction concludeAndTransferAllAssetsFunction)
        {
             return ContractHandler.SendRequestAsync(concludeAndTransferAllAssetsFunction);
        }

        public Task<TransactionReceipt> ConcludeAndTransferAllAssetsRequestAndWaitForReceiptAsync(ConcludeAndTransferAllAssetsFunction concludeAndTransferAllAssetsFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(concludeAndTransferAllAssetsFunction, cancellationToken);
        }

        public Task<string> ConcludeAndTransferAllAssetsRequestAsync(FixedPart fixedPart, List<SignedVariablePart> signedVariableParts)
        {
            var concludeAndTransferAllAssetsFunction = new ConcludeAndTransferAllAssetsFunction();
                concludeAndTransferAllAssetsFunction.FixedPart = fixedPart;
                concludeAndTransferAllAssetsFunction.SignedVariableParts = signedVariableParts;
            
             return ContractHandler.SendRequestAsync(concludeAndTransferAllAssetsFunction);
        }

        public Task<TransactionReceipt> ConcludeAndTransferAllAssetsRequestAndWaitForReceiptAsync(FixedPart fixedPart, List<SignedVariablePart> signedVariableParts, CancellationTokenSource cancellationToken = null)
        {
            var concludeAndTransferAllAssetsFunction = new ConcludeAndTransferAllAssetsFunction();
                concludeAndTransferAllAssetsFunction.FixedPart = fixedPart;
                concludeAndTransferAllAssetsFunction.SignedVariableParts = signedVariableParts;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(concludeAndTransferAllAssetsFunction, cancellationToken);
        }

        public Task<string> DepositRequestAsync(DepositFunction depositFunction)
        {
             return ContractHandler.SendRequestAsync(depositFunction);
        }

        public Task<TransactionReceipt> DepositRequestAndWaitForReceiptAsync(DepositFunction depositFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(depositFunction, cancellationToken);
        }

        public Task<string> DepositRequestAsync(string asset, byte[] channelId, BigInteger expectedHeld, BigInteger amount)
        {
            var depositFunction = new DepositFunction();
                depositFunction.Asset = asset;
                depositFunction.ChannelId = channelId;
                depositFunction.ExpectedHeld = expectedHeld;
                depositFunction.Amount = amount;
            
             return ContractHandler.SendRequestAsync(depositFunction);
        }

        public Task<TransactionReceipt> DepositRequestAndWaitForReceiptAsync(string asset, byte[] channelId, BigInteger expectedHeld, BigInteger amount, CancellationTokenSource cancellationToken = null)
        {
            var depositFunction = new DepositFunction();
                depositFunction.Asset = asset;
                depositFunction.ChannelId = channelId;
                depositFunction.ExpectedHeld = expectedHeld;
                depositFunction.Amount = amount;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(depositFunction, cancellationToken);
        }

        public Task<BigInteger> GetChainIDQueryAsync(GetChainIDFunction getChainIDFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetChainIDFunction, BigInteger>(getChainIDFunction, blockParameter);
        }

        
        public Task<BigInteger> GetChainIDQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetChainIDFunction, BigInteger>(null, blockParameter);
        }

        public Task<BigInteger> HoldingsQueryAsync(HoldingsFunction holdingsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<HoldingsFunction, BigInteger>(holdingsFunction, blockParameter);
        }

        
        public Task<BigInteger> HoldingsQueryAsync(string returnValue1, byte[] returnValue2, BlockParameter blockParameter = null)
        {
            var holdingsFunction = new HoldingsFunction();
                holdingsFunction.ReturnValue1 = returnValue1;
                holdingsFunction.ReturnValue2 = returnValue2;
            
            return ContractHandler.QueryAsync<HoldingsFunction, BigInteger>(holdingsFunction, blockParameter);
        }

        public Task<LatestSupportedStateOutputDTO> LatestSupportedStateQueryAsync(LatestSupportedStateFunction latestSupportedStateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<LatestSupportedStateFunction, LatestSupportedStateOutputDTO>(latestSupportedStateFunction, blockParameter);
        }

        public Task<LatestSupportedStateOutputDTO> LatestSupportedStateQueryAsync(FixedPart fixedPart, List<SignedVariablePart> signedVariableParts, BlockParameter blockParameter = null)
        {
            var latestSupportedStateFunction = new LatestSupportedStateFunction();
                latestSupportedStateFunction.FixedPart = fixedPart;
                latestSupportedStateFunction.SignedVariableParts = signedVariableParts;
            
            return ContractHandler.QueryDeserializingToObjectAsync<LatestSupportedStateFunction, LatestSupportedStateOutputDTO>(latestSupportedStateFunction, blockParameter);
        }

        public Task<byte[]> StatusOfQueryAsync(StatusOfFunction statusOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<StatusOfFunction, byte[]>(statusOfFunction, blockParameter);
        }

        
        public Task<byte[]> StatusOfQueryAsync(byte[] returnValue1, BlockParameter blockParameter = null)
        {
            var statusOfFunction = new StatusOfFunction();
                statusOfFunction.ReturnValue1 = returnValue1;
            
            return ContractHandler.QueryAsync<StatusOfFunction, byte[]>(statusOfFunction, blockParameter);
        }

        public Task<string> TransferRequestAsync(TransferFunction transferFunction)
        {
             return ContractHandler.SendRequestAsync(transferFunction);
        }

        public Task<TransactionReceipt> TransferRequestAndWaitForReceiptAsync(TransferFunction transferFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFunction, cancellationToken);
        }

        public Task<string> TransferRequestAsync(BigInteger assetIndex, byte[] fromChannelId, byte[] outcomeBytes, byte[] stateHash, List<BigInteger> indices)
        {
            var transferFunction = new TransferFunction();
                transferFunction.AssetIndex = assetIndex;
                transferFunction.FromChannelId = fromChannelId;
                transferFunction.OutcomeBytes = outcomeBytes;
                transferFunction.StateHash = stateHash;
                transferFunction.Indices = indices;
            
             return ContractHandler.SendRequestAsync(transferFunction);
        }

        public Task<TransactionReceipt> TransferRequestAndWaitForReceiptAsync(BigInteger assetIndex, byte[] fromChannelId, byte[] outcomeBytes, byte[] stateHash, List<BigInteger> indices, CancellationTokenSource cancellationToken = null)
        {
            var transferFunction = new TransferFunction();
                transferFunction.AssetIndex = assetIndex;
                transferFunction.FromChannelId = fromChannelId;
                transferFunction.OutcomeBytes = outcomeBytes;
                transferFunction.StateHash = stateHash;
                transferFunction.Indices = indices;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferFunction, cancellationToken);
        }

        public Task<string> TransferAllAssetsRequestAsync(TransferAllAssetsFunction transferAllAssetsFunction)
        {
             return ContractHandler.SendRequestAsync(transferAllAssetsFunction);
        }

        public Task<TransactionReceipt> TransferAllAssetsRequestAndWaitForReceiptAsync(TransferAllAssetsFunction transferAllAssetsFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferAllAssetsFunction, cancellationToken);
        }

        public Task<string> TransferAllAssetsRequestAsync(byte[] channelId, List<SingleAssetExit> outcome, byte[] stateHash)
        {
            var transferAllAssetsFunction = new TransferAllAssetsFunction();
                transferAllAssetsFunction.ChannelId = channelId;
                transferAllAssetsFunction.Outcome = outcome;
                transferAllAssetsFunction.StateHash = stateHash;
            
             return ContractHandler.SendRequestAsync(transferAllAssetsFunction);
        }

        public Task<TransactionReceipt> TransferAllAssetsRequestAndWaitForReceiptAsync(byte[] channelId, List<SingleAssetExit> outcome, byte[] stateHash, CancellationTokenSource cancellationToken = null)
        {
            var transferAllAssetsFunction = new TransferAllAssetsFunction();
                transferAllAssetsFunction.ChannelId = channelId;
                transferAllAssetsFunction.Outcome = outcome;
                transferAllAssetsFunction.StateHash = stateHash;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferAllAssetsFunction, cancellationToken);
        }

        public Task<UnpackStatusOutputDTO> UnpackStatusQueryAsync(UnpackStatusFunction unpackStatusFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<UnpackStatusFunction, UnpackStatusOutputDTO>(unpackStatusFunction, blockParameter);
        }

        public Task<UnpackStatusOutputDTO> UnpackStatusQueryAsync(byte[] channelId, BlockParameter blockParameter = null)
        {
            var unpackStatusFunction = new UnpackStatusFunction();
                unpackStatusFunction.ChannelId = channelId;
            
            return ContractHandler.QueryDeserializingToObjectAsync<UnpackStatusFunction, UnpackStatusOutputDTO>(unpackStatusFunction, blockParameter);
        }
    }
}
