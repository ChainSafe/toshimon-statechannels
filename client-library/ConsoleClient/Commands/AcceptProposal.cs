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
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(new Rule("Respond to game proposal"));
        
        // load the game proposal
        byte[] encodedProposal = File.ReadAllBytes(settings.InputPath);
        GameProposal proposal = GameProposal.AbiDecode(encodedProposal);

        Console.WriteLine(proposal);

        EthECKey key = Utils.createOrLoadKey();

        // initial game state
        // select toshimon
        // MonsterCard[] monsters = Utils.selectToshimonParty();
        // PlayerState playerState = new PlayerState(monsters.ToArray(), new ItemCard[0]);

        // // encode the proposal and write to output
        // Byte[] proposalBytes = proposal.AbiEncode();

        // // write to std out or a file if output path provided
        // using (Stream s = settings.OutputPath is null ? Console.OpenStandardOutput() : File.Create(settings.OutputPath) ) {
        //     s.Write(proposalBytes, 0, proposalBytes.Length);
        // }

        // AnsiConsole.Write(new Panel("Successfully created GameProposal message and written to output. Publish this message somewhere to allow other players to propose a game."));

        return 0;
    }
}