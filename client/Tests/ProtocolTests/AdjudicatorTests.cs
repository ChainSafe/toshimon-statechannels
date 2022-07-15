namespace test;

using Nethereum.Signer;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexConvertors;
using Nethereum.Hex.HexConvertors.Extensions;

using Protocol.Adjudicator.Service;
using Protocol.Adjudicator.ContractDefinition;

public class AdjudicatorTests
{
    private ToshimonDeployment.ToshimonDeployment deployment;

    private EthECKey aKey;
    private EthECKey bKey;

    private Account aAccount;
    private Account bAccount;

    private AdjudicatorService aService;
    private AdjudicatorService bService;

    // setup - called before each test
    public AdjudicatorTests() {
        this.deployment = new ToshimonDeployment.ToshimonDeployment(Environment.GetEnvironmentVariable("DEPLOYMENT"));
        
        aKey = EthECKey.GenerateKey();
        bKey = EthECKey.GenerateKey();
       
        aAccount = new Account("0xac0974bec39a17e36ba4a6b4d238ff944bacb478cbed5efcae784d7bf4f2ff80");
        bAccount = new Account("0x59c6995e998f97a5a0044966f0945389dc9e86dae88c7a8412f4603b6b78690d");

        var web3a = new Nethereum.Web3.Web3(aAccount, Environment.GetEnvironmentVariable("ETH_RPC"));
        web3a.TransactionManager.UseLegacyAsDefault = true;

        // set up the services that A or B can use to call functions
        aService = new AdjudicatorService(web3a, deployment.AdjudicatorContractAddress);
        bService = new AdjudicatorService(new Nethereum.Web3.Web3(bAccount, Environment.GetEnvironmentVariable("ETH_RPC")), deployment.AdjudicatorContractAddress);
    }

    public FixedPart TestFixedPart() {
        return new FixedPart() {
            ChainId = deployment.ChainId,
            Participants = new List<string>(){aAccount.Address, bAccount.Address},
            ChannelNonce = 123,
            AppDefinition = deployment.StateTransitionContractAddress,
            ChallengeDuration = 10,
        };
    }

    [Fact]
    public void ChallengeChannel() {

        // Create a new channel fixed part
        var fixedPart = TestFixedPart();
        
        var variablePart0 = new VariablePart() {
            Outcome = new List<SingleAssetExit>(),
            AppData = new byte[]{},
            TurnNum = 2,
            IsFinal = false,
        };

        var signedVariablePart0 = variablePart0.Sign(fixedPart, aKey);


        Console.WriteLine("ChannelId: 0x{0}", fixedPart.ChannelId.ToHex());

        Console.WriteLine("StateHash: 0x{0}", new StateUpdate(fixedPart, variablePart0).StateHash.ToHex());

        // var _result = aService.CheckpointRequestAsync(
        //     fixedPart,
        //     new List<SignedVariablePart>() { signedVariablePart0 }
        // ).Result;

        // var result = aService.ConcludeRequestAsync(
        //     fixedPart,
        //     new List<SignedVariablePart>() { signedVariablePart0 }
        // ).Result;

        // Challenge a channel with the zero state signed by A
        var result = aService.ChallengeRequestAsync(
            fixedPart,
            new List<SignedVariablePart>() { signedVariablePart0 },
            variablePart0.GetChallengeSignature(fixedPart, aKey)
        ).Result;

        Console.WriteLine(result);

        // check the status of the channel 
        // If finalizesAt == 0 then it is open
        // If finalizesAt <= block.timestamp if is finalized
        // else it is in Challenge mode
        // Need to unpack the response..
        // service.StatusOfQueryAsync(channelId).Result;
    }

}