using System.Numerics;
using System.Collections.Immutable;
using Spectre.Console.Cli;
using Spectre.Console;
using Protocol;
using Nethereum.Util;
using Nethereum.Signer;

// [Description("Respond to a game proposal by constructing a message which initializes a new state channel")]
public sealed class AcceptProposalCommand : Command<AcceptProposalCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-i|--input")]
        public string? InputPath { get; init; }

        [CommandOption("-o|--output")]
        public string? OutputPath { get; init; }

        [CommandOption("-d|--def-output")]
        public string? DefinitionOutputPath { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(new Rule("Respond to game proposal"));
        
        // load the game proposal
        byte[] encodedProposal = File.ReadAllBytes(settings.InputPath);
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
        MonsterCard[] monsters = Utils.selectToshimonParty();
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

        GameState appData = new GameState(playerState, proposal.PlayerState);

        var variablePart = new VariablePart() {
            AppData = appData.AbiEncode(),
            Outcome = new List<SingleAssetExit>(),
            TurnNum = 0,
            IsFinal = false,
        };

        // sign this state update with this players key
        var signature = new StateUpdate(fixedPart, variablePart).Sign(key);    

        // combine into an acceptance message
        var stateUpdate = new SignedStateUpdate() {
            VariablePart = variablePart,
            Signature = signature,
        };

        // write to std out or a file if output path provided
        using (Stream s = settings.OutputPath is null ? Console.OpenStandardOutput() : File.Create(settings.OutputPath) ) {
            byte[] stateUpdateBytes = stateUpdate.AbiEncode();
            s.Write(stateUpdateBytes, 0, stateUpdateBytes.Length);
        }

        // write to std out or a file if output path provided
        using (Stream s = settings.DefinitionOutputPath is null ? Console.OpenStandardOutput() : File.Create(settings.DefinitionOutputPath) ) {
            byte[] fixedPartBytes = fixedPart.AbiEncode();
            s.Write(fixedPartBytes, 0, fixedPartBytes.Length);
        }

        AnsiConsole.Write(new Panel("Successfully created the initial channel state. If the other party responds with the next update then the channel can be funded and a game can commence."));

        return 0;
    }
}