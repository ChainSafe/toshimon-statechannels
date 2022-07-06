using System.Numerics;
using System.Collections.Immutable;
using Spectre.Console.Cli;
using Spectre.Console;
using Protocol;
using Nethereum.Util;
using Nethereum.Signer;


// [Description("Create a message which proposes a new game. Another player can respond to this proposal to start a new battle channel.")]
public sealed class CreateProposalCommand : Command<CreateProposalCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-o|--output")]
        public string? OutputPath { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(new Rule("Create new game proposal"));

        int chainId = AnsiConsole.Ask<int>("Chain ID (default Matic Mumbai)", 80001);
        ulong channelNonce = AnsiConsole.Ask<ulong>("Channel Nonce (random value, do not reuse)", (ulong) new Random().Next());
        string appDefinition = AnsiConsole.Ask<string>("Address of Toshimon rules definition contract", "0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2");
        
        EthECKey key = Utils.createOrLoadKey();

        string wagerAsset = AnsiConsole.Ask<string>("Address of Wager Asset (default native asset)", "0x0000000000000000000000000000000000000000");
        BigInteger wagerAmount = AnsiConsole.Ask<BigInteger>("How much to wager (in lowest denomination", 0);
        ulong challengeDuration = AnsiConsole.Ask<ulong>("Channel challenge duration seconds (default 1 day)", 86400);

        // initial game state
        // select toshimon
        MonsterCard[] monsters = Utils.selectToshimonParty();

        PlayerState playerState = new PlayerState(monsters.ToArray(), new ItemCard[0]);

        GameProposal proposal = new GameProposal() {
            ChainId = chainId,
            ChannelNonce = channelNonce,
            AppDefinition = appDefinition,
            SigningKey = key.GetPublicAddress(),
            RecoveryKey = key.GetPublicAddress(),
            Recipient = key.GetPublicAddress(),
            WagerAssetAddress = wagerAsset,
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