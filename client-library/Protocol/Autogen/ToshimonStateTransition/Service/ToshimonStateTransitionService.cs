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

        public Task<bool> DummyQueryAsync(DummyFunction dummyFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<DummyFunction, bool>(dummyFunction, blockParameter);
        }

        
        public Task<bool> DummyQueryAsync(AppData gameState, BlockParameter blockParameter = null)
        {
            var dummyFunction = new DummyFunction();
                dummyFunction.GameState = gameState;
            
            return ContractHandler.QueryAsync<DummyFunction, bool>(dummyFunction, blockParameter);
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

        public Task<bool> ValidTransitionQueryAsync(ValidTransitionFunction validTransitionFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ValidTransitionFunction, bool>(validTransitionFunction, blockParameter);
        }

        
        public Task<bool> ValidTransitionQueryAsync(VariablePart prev, VariablePart next, BigInteger nParticipants, BlockParameter blockParameter = null)
        {
            var validTransitionFunction = new ValidTransitionFunction();
                validTransitionFunction.Prev = prev;
                validTransitionFunction.Next = next;
                validTransitionFunction.NParticipants = nParticipants;
            
            return ContractHandler.QueryAsync<ValidTransitionFunction, bool>(validTransitionFunction, blockParameter);
        }
    }
}
