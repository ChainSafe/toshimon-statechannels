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

namespace Protocol.TESTEncoding.ContractDefinition
{


    public partial class TESTEncodingDeployment : TESTEncodingDeploymentBase
    {
        public TESTEncodingDeployment() : base(BYTECODE) { }
        public TESTEncodingDeployment(string byteCode) : base(byteCode) { }
    }

    public class TESTEncodingDeploymentBase : ContractDeploymentMessage
    {
        public static string BYTECODE = "";
        public TESTEncodingDeploymentBase() : base(BYTECODE) { }
        public TESTEncodingDeploymentBase(string byteCode) : base(byteCode) { }

    }

    public partial class EncodeAppDataFunction : EncodeAppDataFunctionBase { }

    [Function("encodeAppData", "bytes")]
    public class EncodeAppDataFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "payload", 1)]
        public virtual AppData Payload { get; set; }
    }

    public partial class EncodeItemCardFunction : EncodeItemCardFunctionBase { }

    [Function("encodeItemCard", "bytes")]
    public class EncodeItemCardFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "payload", 1)]
        public virtual ItemCard Payload { get; set; }
    }

    public partial class EncodeMonsterCardFunction : EncodeMonsterCardFunctionBase { }

    [Function("encodeMonsterCard", "bytes")]
    public class EncodeMonsterCardFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "payload", 1)]
        public virtual MonsterCard Payload { get; set; }
    }

    public partial class EncodePlayerStateFunction : EncodePlayerStateFunctionBase { }

    [Function("encodePlayerState", "bytes")]
    public class EncodePlayerStateFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "payload", 1)]
        public virtual PlayerState Payload { get; set; }
    }

    public partial class EncodeStateFunction : EncodeStateFunctionBase { }

    [Function("encodeState", "bytes")]
    public class EncodeStateFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "payload", 1)]
        public virtual GameState Payload { get; set; }
    }

    public partial class EncodeStatsFunction : EncodeStatsFunctionBase { }

    [Function("encodeStats", "bytes")]
    public class EncodeStatsFunctionBase : FunctionMessage
    {
        [Parameter("tuple", "payload", 1)]
        public virtual Stats Payload { get; set; }
    }

    public partial class EncodeAppDataOutputDTO : EncodeAppDataOutputDTOBase { }

    [FunctionOutput]
    public class EncodeAppDataOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class EncodeItemCardOutputDTO : EncodeItemCardOutputDTOBase { }

    [FunctionOutput]
    public class EncodeItemCardOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class EncodeMonsterCardOutputDTO : EncodeMonsterCardOutputDTOBase { }

    [FunctionOutput]
    public class EncodeMonsterCardOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class EncodePlayerStateOutputDTO : EncodePlayerStateOutputDTOBase { }

    [FunctionOutput]
    public class EncodePlayerStateOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class EncodeStateOutputDTO : EncodeStateOutputDTOBase { }

    [FunctionOutput]
    public class EncodeStateOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }

    public partial class EncodeStatsOutputDTO : EncodeStatsOutputDTOBase { }

    [FunctionOutput]
    public class EncodeStatsOutputDTOBase : IFunctionOutputDTO 
    {
        [Parameter("bytes", "", 1)]
        public virtual byte[] ReturnValue1 { get; set; }
    }
}
