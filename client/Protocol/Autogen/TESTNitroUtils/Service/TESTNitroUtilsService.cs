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
using Protocol.TESTNitroUtils.ContractDefinition;

namespace Protocol.TESTNitroUtils.Service
{
    public partial class TESTNitroUtilsService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, TESTNitroUtilsDeployment tESTNitroUtilsDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<TESTNitroUtilsDeployment>().SendRequestAndWaitForReceiptAsync(tESTNitroUtilsDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, TESTNitroUtilsDeployment tESTNitroUtilsDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<TESTNitroUtilsDeployment>().SendRequestAsync(tESTNitroUtilsDeployment);
        }

        public static async Task<TESTNitroUtilsService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, TESTNitroUtilsDeployment tESTNitroUtilsDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, tESTNitroUtilsDeployment, cancellationTokenSource);
            return new TESTNitroUtilsService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public TESTNitroUtilsService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<byte[]> GetChannelIdQueryAsync(GetChannelIdFunction getChannelIdFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetChannelIdFunction, byte[]>(getChannelIdFunction, blockParameter);
        }

        
        public Task<byte[]> GetChannelIdQueryAsync(FixedPart fixedPart, BlockParameter blockParameter = null)
        {
            var getChannelIdFunction = new GetChannelIdFunction();
                getChannelIdFunction.FixedPart = fixedPart;
            
            return ContractHandler.QueryAsync<GetChannelIdFunction, byte[]>(getChannelIdFunction, blockParameter);
        }

        public Task<List<byte>> GetClaimedSignersIndicesQueryAsync(GetClaimedSignersIndicesFunction getClaimedSignersIndicesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetClaimedSignersIndicesFunction, List<byte>>(getClaimedSignersIndicesFunction, blockParameter);
        }

        
        public Task<List<byte>> GetClaimedSignersIndicesQueryAsync(BigInteger signedBy, BlockParameter blockParameter = null)
        {
            var getClaimedSignersIndicesFunction = new GetClaimedSignersIndicesFunction();
                getClaimedSignersIndicesFunction.SignedBy = signedBy;
            
            return ContractHandler.QueryAsync<GetClaimedSignersIndicesFunction, List<byte>>(getClaimedSignersIndicesFunction, blockParameter);
        }

        public Task<byte> GetClaimedSignersNumQueryAsync(GetClaimedSignersNumFunction getClaimedSignersNumFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetClaimedSignersNumFunction, byte>(getClaimedSignersNumFunction, blockParameter);
        }

        
        public Task<byte> GetClaimedSignersNumQueryAsync(BigInteger signedBy, BlockParameter blockParameter = null)
        {
            var getClaimedSignersNumFunction = new GetClaimedSignersNumFunction();
                getClaimedSignersNumFunction.SignedBy = signedBy;
            
            return ContractHandler.QueryAsync<GetClaimedSignersNumFunction, byte>(getClaimedSignersNumFunction, blockParameter);
        }

        public Task<byte[]> HashStateQueryAsync(HashStateFunction hashStateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<HashStateFunction, byte[]>(hashStateFunction, blockParameter);
        }

        
        public Task<byte[]> HashStateQueryAsync(byte[] channelId, byte[] appData, List<SingleAssetExit> outcome, ulong turnNum, bool isFinal, BlockParameter blockParameter = null)
        {
            var hashStateFunction = new HashStateFunction();
                hashStateFunction.ChannelId = channelId;
                hashStateFunction.AppData = appData;
                hashStateFunction.Outcome = outcome;
                hashStateFunction.TurnNum = turnNum;
                hashStateFunction.IsFinal = isFinal;
            
            return ContractHandler.QueryAsync<HashStateFunction, byte[]>(hashStateFunction, blockParameter);
        }

        public Task<bool> IsClaimedSignedByQueryAsync(IsClaimedSignedByFunction isClaimedSignedByFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsClaimedSignedByFunction, bool>(isClaimedSignedByFunction, blockParameter);
        }

        
        public Task<bool> IsClaimedSignedByQueryAsync(BigInteger signedBy, byte participantIndex, BlockParameter blockParameter = null)
        {
            var isClaimedSignedByFunction = new IsClaimedSignedByFunction();
                isClaimedSignedByFunction.SignedBy = signedBy;
                isClaimedSignedByFunction.ParticipantIndex = participantIndex;
            
            return ContractHandler.QueryAsync<IsClaimedSignedByFunction, bool>(isClaimedSignedByFunction, blockParameter);
        }

        public Task<bool> IsClaimedSignedOnlyByQueryAsync(IsClaimedSignedOnlyByFunction isClaimedSignedOnlyByFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsClaimedSignedOnlyByFunction, bool>(isClaimedSignedOnlyByFunction, blockParameter);
        }

        
        public Task<bool> IsClaimedSignedOnlyByQueryAsync(BigInteger signedBy, byte participantIndex, BlockParameter blockParameter = null)
        {
            var isClaimedSignedOnlyByFunction = new IsClaimedSignedOnlyByFunction();
                isClaimedSignedOnlyByFunction.SignedBy = signedBy;
                isClaimedSignedOnlyByFunction.ParticipantIndex = participantIndex;
            
            return ContractHandler.QueryAsync<IsClaimedSignedOnlyByFunction, bool>(isClaimedSignedOnlyByFunction, blockParameter);
        }

        public Task<string> RecoverSignerQueryAsync(RecoverSignerFunction recoverSignerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RecoverSignerFunction, string>(recoverSignerFunction, blockParameter);
        }

        
        public Task<string> RecoverSignerQueryAsync(byte[] d, Signature sig, BlockParameter blockParameter = null)
        {
            var recoverSignerFunction = new RecoverSignerFunction();
                recoverSignerFunction.D = d;
                recoverSignerFunction.Sig = sig;
            
            return ContractHandler.QueryAsync<RecoverSignerFunction, string>(recoverSignerFunction, blockParameter);
        }
    }
}
