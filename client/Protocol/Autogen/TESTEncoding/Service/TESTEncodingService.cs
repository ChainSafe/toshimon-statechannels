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
using Protocol.TESTEncoding.ContractDefinition;

namespace Protocol.TESTEncoding.Service
{
    public partial class TESTEncodingService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, TESTEncodingDeployment tESTEncodingDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<TESTEncodingDeployment>().SendRequestAndWaitForReceiptAsync(tESTEncodingDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, TESTEncodingDeployment tESTEncodingDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<TESTEncodingDeployment>().SendRequestAsync(tESTEncodingDeployment);
        }

        public static async Task<TESTEncodingService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, TESTEncodingDeployment tESTEncodingDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, tESTEncodingDeployment, cancellationTokenSource);
            return new TESTEncodingService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public TESTEncodingService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<byte[]> EncodeAppDataQueryAsync(EncodeAppDataFunction encodeAppDataFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<EncodeAppDataFunction, byte[]>(encodeAppDataFunction, blockParameter);
        }

        
        public Task<byte[]> EncodeAppDataQueryAsync(AppData payload, BlockParameter blockParameter = null)
        {
            var encodeAppDataFunction = new EncodeAppDataFunction();
                encodeAppDataFunction.Payload = payload;
            
            return ContractHandler.QueryAsync<EncodeAppDataFunction, byte[]>(encodeAppDataFunction, blockParameter);
        }

        public Task<byte[]> EncodeItemCardQueryAsync(EncodeItemCardFunction encodeItemCardFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<EncodeItemCardFunction, byte[]>(encodeItemCardFunction, blockParameter);
        }

        
        public Task<byte[]> EncodeItemCardQueryAsync(ItemCard payload, BlockParameter blockParameter = null)
        {
            var encodeItemCardFunction = new EncodeItemCardFunction();
                encodeItemCardFunction.Payload = payload;
            
            return ContractHandler.QueryAsync<EncodeItemCardFunction, byte[]>(encodeItemCardFunction, blockParameter);
        }

        public Task<byte[]> EncodeMonsterCardQueryAsync(EncodeMonsterCardFunction encodeMonsterCardFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<EncodeMonsterCardFunction, byte[]>(encodeMonsterCardFunction, blockParameter);
        }

        
        public Task<byte[]> EncodeMonsterCardQueryAsync(MonsterCard payload, BlockParameter blockParameter = null)
        {
            var encodeMonsterCardFunction = new EncodeMonsterCardFunction();
                encodeMonsterCardFunction.Payload = payload;
            
            return ContractHandler.QueryAsync<EncodeMonsterCardFunction, byte[]>(encodeMonsterCardFunction, blockParameter);
        }

        public Task<byte[]> EncodePlayerStateQueryAsync(EncodePlayerStateFunction encodePlayerStateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<EncodePlayerStateFunction, byte[]>(encodePlayerStateFunction, blockParameter);
        }

        
        public Task<byte[]> EncodePlayerStateQueryAsync(PlayerState payload, BlockParameter blockParameter = null)
        {
            var encodePlayerStateFunction = new EncodePlayerStateFunction();
                encodePlayerStateFunction.Payload = payload;
            
            return ContractHandler.QueryAsync<EncodePlayerStateFunction, byte[]>(encodePlayerStateFunction, blockParameter);
        }

        public Task<byte[]> EncodeStateQueryAsync(EncodeStateFunction encodeStateFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<EncodeStateFunction, byte[]>(encodeStateFunction, blockParameter);
        }

        
        public Task<byte[]> EncodeStateQueryAsync(GameState payload, BlockParameter blockParameter = null)
        {
            var encodeStateFunction = new EncodeStateFunction();
                encodeStateFunction.Payload = payload;
            
            return ContractHandler.QueryAsync<EncodeStateFunction, byte[]>(encodeStateFunction, blockParameter);
        }

        public Task<byte[]> EncodeStatsQueryAsync(EncodeStatsFunction encodeStatsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<EncodeStatsFunction, byte[]>(encodeStatsFunction, blockParameter);
        }

        
        public Task<byte[]> EncodeStatsQueryAsync(Stats payload, BlockParameter blockParameter = null)
        {
            var encodeStatsFunction = new EncodeStatsFunction();
                encodeStatsFunction.Payload = payload;
            
            return ContractHandler.QueryAsync<EncodeStatsFunction, byte[]>(encodeStatsFunction, blockParameter);
        }
    }
}
