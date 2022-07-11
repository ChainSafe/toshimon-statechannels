using System.Numerics;
using System.Collections.Immutable;
using Spectre.Console.Cli;
using Spectre.Console;
using Protocol;
using Nethereum.Util;
using Nethereum.Signer;
using ToshimonDeployment;

// [Description("Respond to a game proposal by constructing a message which initializes a new state channel")]
public sealed class AcceptProposalCommand : Command<AcceptProposalCommand.Settings>
{
    public sealed class Settings : SharedSettings
    {
        [CommandOption("-p|--proposal")]
        public string? ProposalPath { get; init; }

        [CommandOption("-o|--output")]
        public string? OutputPath { get; init; }

        public Settings(string? deploymentPath, string? rpcUrl) : base(deploymentPath, rpcUrl) {}
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(new Rule("Respond to game proposal"));
        
        ToshimonDeployment.ToshimonDeployment deployment = new ToshimonDeployment.ToshimonDeployment(settings.DeploymentPath);

        // load the game proposal
        byte[] encodedProposal = File.ReadAllBytes(settings.ProposalPath);
        GameProposal proposal = GameProposal.AbiDecode(encodedProposal);

        // display it so the player can see if they want to respond
        Utils.renderProposal();

        if(!AnsiConsole.Confirm(String.Format("Would you like to respond to this proposal?"))) {
            return 0;
        }

        EthECKey key = Utils.createOrLoadKey();
        var participants = new List<string>();

        participants.Add(key.GetPublicAddress());
        participants.Add(proposal.SigningKey);

        // initial game state
        // select toshimon
        MonsterCard[] monsters = Utils.selectToshimonParty(deployment);
        PlayerState playerState = new PlayerState(monsters.ToArray(), new ItemCard[0]);

        // create the inital state channel tuples (FixedPart, VariablePart, Signature)
        // using data from the proposal
        var fixedPart = new FixedPart() {
            ChainId = proposal.ChainId,
            Participants = participants,
            ChannelNonce = proposal.ChannelNonce,
            AppDefinition = proposal.AppDefinition,
            ChallengeDuration = proposal.ChallengeDuration,
        };

        // create a new game state from the proposal and wrap in a rolling random app data
        GameState gameState = new GameState(playerState, proposal.PlayerState);
        AppData appData = new AppData(gameState.AbiEncode());

        var variablePart = new VariablePart() {
            AppData = appData.AbiEncode(),
            Outcome = new List<SingleAssetExit>(),
            TurnNum = 0,
            IsFinal = false,
        };

        var channelDir = Path.Combine(Path.GetFullPath(settings.OutputPath ?? "."), Convert.ToBase64String(fixedPart.ChannelId));
        Directory.CreateDirectory(channelDir);


        // Write channel definition to output directory
        using (Stream s = File.Create(Path.Combine(channelDir, "channelSpec")) ) {
            byte[] fixedPartBytes = fixedPart.AbiEncode();
            s.Write(fixedPartBytes, 0, fixedPartBytes.Length);
        }

        Utils.signAndWriteUpdate(fixedPart, variablePart, key, Path.Combine(channelDir, "0.state"));

        AnsiConsole.Write(new Panel("Successfully created the initial channel state. If the other party responds with the next update then the channel can be funded and a game can commence."));

        return 0;
    }
}