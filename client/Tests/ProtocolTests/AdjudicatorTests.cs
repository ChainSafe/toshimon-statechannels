namespace test;

using Nethereum.Signer;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexConvertors;
using Nethereum.Hex.HexConvertors.Extensions;

using Protocol.Adjudicator.Service;
using Protocol.Adjudicator.ContractDefinition;
using Protocol.TESTNitroUtils.Service;


public class AdjudicatorTests
{
    private ToshimonDeployment.ToshimonDeployment deployment;

    private EthECKey aKey;
    private EthECKey bKey;

    private Account aAccount;
    private Account bAccount;

    private AdjudicatorService aService;
    private AdjudicatorService bService;

    private Nethereum.Web3.Web3 web3;

    // setup - called before each test
    public AdjudicatorTests() {
        this.deployment = new ToshimonDeployment.ToshimonDeployment(Environment.GetEnvironmentVariable("DEPLOYMENT"));
        
        aKey = EthECKey.GenerateKey();
        bKey = EthECKey.GenerateKey();
       
        aAccount = new Account("0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80");
        bAccount = new Account("0x59c6995e998f97a5a0044966f0945389dc9e86dae88c7a8412f4603b6b78690d");

        web3 = new Nethereum.Web3.Web3(Environment.GetEnvironmentVariable("ETH_RPC"));

        var web3a = new Nethereum.Web3.Web3(aAccount, Environment.GetEnvironmentVariable("ETH_RPC"));
        web3a.TransactionManager.UseLegacyAsDefault = true;

        // set up the services that A or B can use to call functions
        aService = new AdjudicatorService(web3a, deployment.AdjudicatorContractAddress);
        bService = new AdjudicatorService(new Nethereum.Web3.Web3(bAccount, Environment.GetEnvironmentVariable("ETH_RPC")), deployment.AdjudicatorContractAddress);
    }

    public FixedPart TestFixedPart() {
        return new FixedPart() {
            ChainId = deployment.ChainId,
            Participants = new List<string>(){ aKey.GetPublicAddress(), bKey.GetPublicAddress() },
            ChannelNonce = 123,
            AppDefinition = deployment.StateTransitionContractAddress,
            ChallengeDuration = 10,
        };
    }

    public VariablePart TestVariablePart() {
        return new VariablePart() {
            Outcome = new List<SingleAssetExit>() { SingleAssetExit.NewSimpleNative("0x0000000000000000000000000000000000000000", 10) },
            AppData = new byte[]{ 0x00 },
            TurnNum = 0,
            IsFinal = false,
        };
    }

    [Fact]
    public void GeneratesCorrectChannelId() {
        var fixedPart = TestFixedPart();
        var service = new TESTNitroUtilsService(web3, deployment.NitroTestContractAddress);

        var result = service.GetChannelIdQueryAsync(
            fixedPart
        ).Result;

        Assert.Equal(result, fixedPart.ChannelId);

    }

    [Fact]
    public void GeneratesCorrectStateHash() {
        var fixedPart = TestFixedPart();
        var variablePart0 = TestVariablePart();

        var service = new TESTNitroUtilsService(web3, deployment.NitroTestContractAddress);

        var result = service.HashStateQueryAsync(
            fixedPart.ChannelId,
            variablePart0.AppData,
            variablePart0.Outcome,
            variablePart0.TurnNum,
            variablePart0.IsFinal
        ).Result;

        Assert.Equal(result, new StateUpdate(fixedPart, variablePart0).StateHash);

    }

    [Fact]
    public void CanRecoverSignerOfStateHash() {
        var fixedPart = TestFixedPart();
        var variablePart0 = TestVariablePart();

        var stateUpdate = new StateUpdate(TestFixedPart(), TestVariablePart());

        Signature signature = stateUpdate.Sign(aKey);

        var service = new TESTNitroUtilsService(web3, deployment.NitroTestContractAddress);
        
        var result = service.RecoverSignerQueryAsync(
            stateUpdate.StateHash,
            signature
        ).Result;

        Assert.Equal(result, aKey.GetPublicAddress());

    }

    [Fact]
    public void ChallengeChannel() {

        // Create a new channel fixed part
        var fixedPart = TestFixedPart();
        var variablePart0 = TestVariablePart();
        var signedVariablePart0 = variablePart0.Sign(fixedPart, aKey);

        // Challenge a channel with the zero state signed by A
        var _ = aService.ChallengeRequestAsync(
            fixedPart,
            new List<SignedVariablePart>() { signedVariablePart0 },
            variablePart0.GetChallengeSignature(fixedPart, aKey)
        ).Result;

        // check the status of the channel 
        // If finalizesAt == 0 then it is open
        // If finalizesAt <= block.timestamp if is finalized
        // else it is in Challenge mode
        var result = aService.UnpackStatusQueryAsync(
            fixedPart.ChannelId
        ).Result;

        // this signifigies the Challenged status
        Assert.True(result.FinalizesAt > 0);
    }

}