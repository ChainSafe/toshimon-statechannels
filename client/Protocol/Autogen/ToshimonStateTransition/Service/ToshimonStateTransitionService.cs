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
using Protocol.ToshimonStateTransition.ContractDefinition;

namespace Protocol.ToshimonStateTransition.Service
{
    public partial class ToshimonStateTransitionService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, ToshimonStateTransitionDeployment toshimonStateTransitionDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<ToshimonStateTransitionDeployment>().SendRequestAndWaitForReceiptAsync(toshimonStateTransitionDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, ToshimonStateTransitionDeployment toshimonStateTransitionDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<ToshimonStateTransitionDeployment>().SendRequestAsync(toshimonStateTransitionDeployment);
        }

        public static async Task<ToshimonStateTransitionService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, ToshimonStateTransitionDeployment toshimonStateTransitionDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, toshimonStateTransitionDeployment, cancellationTokenSource);
            return new ToshimonStateTransitionService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public ToshimonStateTransitionService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<AdvanceStateOutputDTO> AdvanceStateQueryAsync(AdvanceStateFunction advanceStateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<AdvanceStateFunction, AdvanceStateOutputDTO>(advanceStateFunction, blockParameter);
        }

        public Task<AdvanceStateOutputDTO> AdvanceStateQueryAsync(byte[] gameState_, List<SingleAssetExit> outcome, byte moveA, byte moveB, byte[] randomSeed, BlockParameter blockParameter = null)
        {
            var advanceStateFunction = new AdvanceStateFunction();
                advanceStateFunction.GameState_ = gameState_;
                advanceStateFunction.Outcome = outcome;
                advanceStateFunction.MoveA = moveA;
                advanceStateFunction.MoveB = moveB;
                advanceStateFunction.RandomSeed = randomSeed;
            
            return ContractHandler.QueryDeserializingToObjectAsync<AdvanceStateFunction, AdvanceStateOutputDTO>(advanceStateFunction, blockParameter);
        }

        public Task<AdvanceStateTypedOutputDTO> AdvanceStateTypedQueryAsync(AdvanceStateTypedFunction advanceStateTypedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<AdvanceStateTypedFunction, AdvanceStateTypedOutputDTO>(advanceStateTypedFunction, blockParameter);
        }

        public Task<AdvanceStateTypedOutputDTO> AdvanceStateTypedQueryAsync(GameState gameState, List<SingleAssetExit> outcome, byte moveA, byte moveB, byte[] randomSeed, BlockParameter blockParameter = null)
        {
            var advanceStateTypedFunction = new AdvanceStateTypedFunction();
                advanceStateTypedFunction.GameState = gameState;
                advanceStateTypedFunction.Outcome = outcome;
                advanceStateTypedFunction.MoveA = moveA;
                advanceStateTypedFunction.MoveB = moveB;
                advanceStateTypedFunction.RandomSeed = randomSeed;
            
            return ContractHandler.QueryDeserializingToObjectAsync<AdvanceStateTypedFunction, AdvanceStateTypedOutputDTO>(advanceStateTypedFunction, blockParameter);
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

        public Task<UpdateOutcomeFavourPlayerOutputDTO> UpdateOutcomeFavourPlayerQueryAsync(UpdateOutcomeFavourPlayerFunction updateOutcomeFavourPlayerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<UpdateOutcomeFavourPlayerFunction, UpdateOutcomeFavourPlayerOutputDTO>(updateOutcomeFavourPlayerFunction, blockParameter);
        }

        public Task<UpdateOutcomeFavourPlayerOutputDTO> UpdateOutcomeFavourPlayerQueryAsync(List<SingleAssetExit> outcome, byte playerIndex, BlockParameter blockParameter = null)
        {
            var updateOutcomeFavourPlayerFunction = new UpdateOutcomeFavourPlayerFunction();
                updateOutcomeFavourPlayerFunction.Outcome = outcome;
                updateOutcomeFavourPlayerFunction.PlayerIndex = playerIndex;
            
            return ContractHandler.QueryDeserializingToObjectAsync<UpdateOutcomeFavourPlayerFunction, UpdateOutcomeFavourPlayerOutputDTO>(updateOutcomeFavourPlayerFunction, blockParameter);
        }
    }
}
