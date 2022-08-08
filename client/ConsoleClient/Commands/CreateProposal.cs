using System.Numerics;
using System.Collections.Immutable;
using Spectre.Console.Cli;
using Spectre.Console;
using Protocol;
using Nethereum.Util;
using Nethereum.Signer;
using ToshimonDeployment;


// [Description("Create a message which proposes a new game. Another player can respond to this proposal to start a new battle channel.")]
public sealed class CreateProposalCommand : Command<CreateProposalCommand.Settings>
{
    public sealed class Settings : SharedSettings
    {
        [CommandOption("-o|--output")]
        public string? OutputPath { get; init; }

        public Settings(string? deploymentPath, string? rpcUrl) : base(deploymentPath, rpcUrl) {}
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(new Rule("Create new game proposal"));

        ToshimonDeployment.ToshimonDeployment deployment = new ToshimonDeployment.ToshimonDeployment(settings.DeploymentPath);
        
        ulong channelNonce = AnsiConsole.Ask<ulong>("Channel Nonce (random value, do not reuse)", (ulong) new Random().Next());
        
        EthECKey key = Utils.createOrLoadKey();

        BigInteger wagerAmount = AnsiConsole.Ask<BigInteger>("How much to wager (in lowest denomination", 0);
        string recipientAddress = AnsiConsole.Ask<string>("Input address to receive winnings");

        ulong challengeDuration = AnsiConsole.Ask<ulong>("Channel challenge duration seconds (default 1 day)", 86400);

        // initial game state
        // select toshimon
        MonsterCard[] monsters = Utils.selectToshimonParty(deployment);

        PlayerState playerState = new PlayerState();
        playerState.Monsters = monsters.ToList();

        GameProposal proposal = new GameProposal() {
            ChainId = deployment.ChainId,
            ChannelNonce = channelNonce,
            AppDefinition = deployment.StateTransitionContractAddress,
            SigningKey = key.GetPublicAddress(),
            RecoveryKey = key.GetPublicAddress(),
            Recipient = recipientAddress.ToHex(),
            WagerAssetAddress = "0x0000000000000000000000000000000000000000", // the native asset
            WagerAmount = wagerAmount,
            ChallengeDuration = challengeDuration,
            PlayerState = playerState,
        };

        // encode the proposal and write to output
        Byte[] proposalBytes = proposal.AbiEncode();

        // write to std out or a file if output path provided
        using (Stream s = settings.OutputPath is null ? Console.OpenStandardOutput() : File.Create(settings.OutputPath) ) {
            s.Write(proposalBytes, 0, proposalBytes.Length);
        }

        AnsiConsole.Write(new Panel("Successfully created GameProposal message and written to output. Publish this message somewhere to allow other players to propose a game."));

        return 0;
    }


}