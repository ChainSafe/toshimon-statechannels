namespace test;

using Nethereum.Signer;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexConvertors;
using Nethereum.Hex.HexConvertors.Extensions;

using Protocol.Adjudicator.Service;
using Protocol.Adjudicator.ContractDefinition;
using Protocol.TESTNitroUtils.Service;
using Protocol.ToshimonStateTransition.Service;
using Protocol.ToshimonStateTransition.ContractDefinition;

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

    public VariablePart TestVariablePart(ulong turnNum) {
        return new VariablePart() {
            Outcome = new List<SingleAssetExit>() { SingleAssetExit.NewSimpleNative("0x0000000000000000000000000000000000000000", 10) },
            AppData = new byte[]{ 0x00 },
            TurnNum = turnNum,
            IsFinal = false,
        };
    }

    private bool ChannelIsOpen(byte[] channelId) {
        var result = aService.UnpackStatusQueryAsync(
            channelId
        ).Result;
        return result.FinalizesAt == 0;
    }

    // [Fact]
    // public void GeneratesCorrectChannelId() {
    //     var fixedPart = TestFixedPart();
    //     var service = new TESTNitroUtilsService(web3, deployment.NitroTestContractAddress);

    //     var result = service.GetChannelIdQueryAsync(
    //         fixedPart
    //     ).Result;

    //     Assert.Equal(result, fixedPart.ChannelId);

    // }

    // [Fact]
    // public void GeneratesCorrectStateHash() {
    //     var fixedPart = TestFixedPart();
    //     var variablePart0 = TestVariablePart(00);

    //     var service = new TESTNitroUtilsService(web3, deployment.NitroTestContractAddress);

    //     var result = service.HashStateQueryAsync(
    //         fixedPart.ChannelId,
    //         variablePart0.AppData,
    //         variablePart0.Outcome,
    //         variablePart0.TurnNum,
    //         variablePart0.IsFinal
    //     ).Result;

    //     Assert.Equal(result, new StateUpdate(fixedPart, variablePart0).StateHash);

    // }

    // [Fact]
    // public void CanRecoverSignerOfStateHash() {
    //     var fixedPart = TestFixedPart();
    //     var variablePart0 = TestVariablePart(0);

    //     var stateUpdate = new StateUpdate(TestFixedPart(), TestVariablePart(0));

    //     Signature signature = stateUpdate.Sign(aKey);

    //     var service = new TESTNitroUtilsService(web3, deployment.NitroTestContractAddress);
        
    //     var result = service.RecoverSignerQueryAsync(
    //         stateUpdate.StateHash,
    //         signature
    //     ).Result;

    //     Assert.Equal(result, aKey.GetPublicAddress());

    // }

    // [Fact]
    // public void ChallengeChannel() {

    //     /*
    //      * A channel can be put into Challenge mode by calling the challenge function on the 
    //      * adjudicator contract. If a challenge is valid this will put the channel into Challenge
    //      * mode and start the countdown to expiry. If the expiry is reached before the other party 
    //      * responds then the channel will finalize in the last published state
    //      *
    //      * Only participants in the channel can submit a challenge
    //      */

    //     // Create a new channel fixed part
    //     var fixedPart = TestFixedPart();

    //     var variablePart0 = TestVariablePart(0);
    //     var signedVariablePart0 = variablePart0.ToSigned(fixedPart, 0, aKey);

    //     // create a state update signed by B on their turn
    //     var variablePart1 = TestVariablePart(1);
    //     var signedVariablePart1 = variablePart1.ToSigned(fixedPart, 1, bKey);

    //     // As B, challenge the channel with state you just signed
    //     var _ = aService.ChallengeRequestAsync(
    //         fixedPart,
    //         new List<SignedVariablePart>() { signedVariablePart0, signedVariablePart1 },
    //         variablePart1.GetChallengeSignature(fixedPart, bKey)
    //     ).Result;

    //     // check the status of the channel 
    //     // If finalizesAt == 0 then it is open
    //     // If finalizesAt <= block.timestamp if is finalized
    //     // else it is in Challenge mode
    //     var result = aService.UnpackStatusQueryAsync(
    //         fixedPart.ChannelId
    //     ).Result;

    //     // this signifigies the Challenged status
    //     Assert.True(!ChannelIsOpen(fixedPart.ChannelId));
    // }

    // [Fact]
    // public void CheckpointChannel() {

    //     /*
    //         A call to checkpoint accepts a FixedPart and a collection of SignedVariableParts much like a call to challenge.
    //         It does not require a special signature and it is possible for anyone to make a checkpoint, 
    //         not just participants in the channel.
    //         Calling checkpoint will update the data stored agains the channel and will clear any open challenges if conditions are met
    //      */

    //     // Create a new channel fixed part
    //     var fixedPart = TestFixedPart();

    //     var variablePart0 = TestVariablePart(0);
    //     var signedVariablePart0 = variablePart0.ToSigned(fixedPart, 0, aKey);

    //     // create a state update signed by B on their turn
    //     var variablePart1 = TestVariablePart(1);
    //     var signedVariablePart1 = variablePart1.ToSigned(fixedPart, 1, bKey);

    //     // Checkpoint the channel using the pair of states
    //     var _ = aService.CheckpointRequestAsync(
    //         fixedPart,
    //         new List<SignedVariablePart>() { signedVariablePart0, signedVariablePart1 }
    //     ).Result;

    //     // check the status of the channel 
    //     // If finalizesAt == 0 then it is open
    //     // If finalizesAt <= block.timestamp if is finalized
    //     // else it is in Challenge mode
    //     var result = aService.UnpackStatusQueryAsync(
    //         fixedPart.ChannelId
    //     ).Result;

    //     Assert.Equal(result.TurnNumRecord, (ulong) 1); // registers latest turn number
    // }    

    // [Fact]
    // public void CheckpointClearsChallenge() {

    //     /*
    //         A channel in Challenge mode can be returned to Open mode by the other party calling the 
    //         conclude function on the adjudicator. This will check for support on the provided signed states
    //         and update the status stored against the channel if support is met
    //     */

    //     // Create a new channel fixed part
    //     var fixedPart = TestFixedPart();

    //     var variablePart0 = TestVariablePart(0);
    //     var signedVariablePart0 = variablePart0.ToSigned(fixedPart, 0, aKey);

    //     // create a state update signed by B on their turn
    //     var variablePart1 = TestVariablePart(1);
    //     var signedVariablePart1 = variablePart1.ToSigned(fixedPart, 1, bKey);

    //     Assert.True(ChannelIsOpen(fixedPart.ChannelId));

    //     // As B, challenge the channel with state you just signed
    //     var _ = aService.ChallengeRequestAsync(
    //         fixedPart,
    //         new List<SignedVariablePart>() { signedVariablePart0, signedVariablePart1 },
    //         variablePart1.GetChallengeSignature(fixedPart, bKey)
    //     ).Result;

    //     Assert.True(!ChannelIsOpen(fixedPart.ChannelId));

    //     // as A create a new state to checkpoint and update the channel
    //     var variablePart2 = TestVariablePart(2);
    //     var signedVariablePart2 = variablePart2.ToSigned(fixedPart, 0, aKey);

    //     // call checkpoint with the last two pair of states
    //     var __ = aService.CheckpointRequestAsync(
    //         fixedPart,
    //         new List<SignedVariablePart>() { signedVariablePart1, signedVariablePart2 }
    //     ).Result;

    //     // Channel should be open again
    //     Assert.True(ChannelIsOpen(fixedPart.ChannelId));
    // }    

    // [Fact]
    // public void CanDecodeGameStateOnChainIntegration() {
    //     GameState gs0 = TestHelpers.build1v1 (
    //         TestHelpers.testMonster1(deployment.Moves[102].Address),
    //         TestHelpers.testMonster1(deployment.Moves[102].Address)
    //     );
        
    //     var web3 = new Nethereum.Web3.Web3(Environment.GetEnvironmentVariable("ETH_RPC"));
    //     var service = new ToshimonStateTransitionService(web3, deployment.StateTransitionContractAddress);

    //     var fixedPart = TestFixedPart();
        
    //     var variablePart0 = TestVariablePart(6);
    //     variablePart0.AppData = new AppData(gs0.AbiEncode()).AbiEncode(); // <- This is totally wrong... Should be a rollingRandom appData
    //     var signedVariablePart0 = variablePart0.ToSigned(fixedPart, 0, aKey);


    //     var variablePart1 = TestVariablePart(7); // turnNum must exceed the pre/post fund stages or else decoding is not performed
    //     variablePart1.AppData = new AppData(gs0.AbiEncode()).AbiEncode();
    //     var signedVariablePart1 = variablePart1.ToSigned(fixedPart, 1, bKey);

    //     var result = service.LatestSupportedStateQueryAsync(
    //         fixedPart,
    //         new List<SignedVariablePart>() { signedVariablePart0, signedVariablePart1 }
    //     ).Result;

    //     Console.WriteLine("{0}", result.ReturnValue1);
    // }

    // [Fact]
    // public void CanDecodeGameStateOnChain() {
    //     GameState gs0 = TestHelpers.build1v1 (
    //         TestHelpers.testMonster1(deployment.Moves[102].Address),
    //         TestHelpers.testMonster1(deployment.Moves[102].Address)
    //     );
        
    //     var web3 = new Nethereum.Web3.Web3(Environment.GetEnvironmentVariable("ETH_RPC"));
    //     var service = new ToshimonStateTransitionService(web3, deployment.StateTransitionContractAddress);

        

    //     var result = service.AdvanceStateQueryAsync(
    //         gs0.AbiEncode(),
    //         TestVariablePart(0).Outcome,
    //         0,
    //         0,
    //         Enumerable.Repeat((byte) 0, 32).ToArray()
    //     ).Result;

    //     Console.WriteLine("{0}", result.ReturnValue1);
    // } 

    [Fact]
    public void ChainEncodingSameAsLocal() {

        var web3 = new Nethereum.Web3.Web3(Environment.GetEnvironmentVariable("ETH_RPC"));
        var service = new ToshimonStateTransitionService(web3, deployment.StateTransitionContractAddress);

        string addr0 = "0x0000000000000000000000000000000000000000";

        var itemCard = new ItemCard() {
            CardId = 0,
            Definition = addr0,
            Used = false,
        };
        Assert.Equal(service.EncodeItemCardQueryAsync(itemCard).Result, itemCard.AbiEncode());


        var stats = new Stats() {
            PP = new List<uint>(){ 0, 0, 0, 0 }
        };
        Assert.Equal(service.EncodeStatsQueryAsync(stats).Result, stats.AbiEncode());

        var monsterCard = new MonsterCard() {
            BaseStats = stats,
            Stats = stats,
            Moves = new List<string>() {addr0, addr0, addr0, addr0},
            StatusCondition = addr0,
            SpecialStatusCondition = addr0
        };
        Assert.Equal(service.EncodeMonsterCardQueryAsync(monsterCard).Result, monsterCard.AbiEncode());

        var playerState = new PlayerState() {
            Monsters = new List<MonsterCard>() {monsterCard, monsterCard, monsterCard, monsterCard, monsterCard},
            Items = new List<ItemCard>() {itemCard, itemCard, itemCard, itemCard, itemCard},
            ActiveMonsterIndex = 255,
        };
        Assert.Equal(service.EncodePlayerStateQueryAsync(playerState).Result, playerState.AbiEncode());

        var gameState = new GameState() {
            Players = new List<PlayerState>{ playerState, playerState }
        };

        Assert.Equal(service.EncodeStateQueryAsync(gameState).Result, gameState.AbiEncode());

    } 

    static string FormatSlots(string str)
    {
        var chunks = Enumerable.Range(0, str.Length / 64)
            .Select(i => str.Substring(i * 64, 64));
        return String.Join("\n", chunks);
    }

}