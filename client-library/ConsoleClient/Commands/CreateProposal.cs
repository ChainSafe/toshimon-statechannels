using System.Numerics;
using Spectre.Console.Cli;
using Spectre.Console;
using Protocol;

public sealed class CreateProposalCommand : Command<CreateProposalCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-o|--output")]
        // [Description("Path to write the encoded proposal file to")]
        public string? OutputPath { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        // AnsiConsole.Write(new Rule("Setup State Channel"));

        // int chainId = AnsiConsole.Ask<int>("Chain ID (default Matic Mumbai)", 80001);
        // int channelNonce = AnsiConsole.Ask<int>("Channel Nonce (random value, do not reuse)", new Random().Next());
        // string appDefinition = AnsiConsole.Ask<string>("Address of Toshimon rules definition contract", "0xc02aaa39b223fe8d0a0e5c4f27ead9083c756cc2");
        
        // if (AnsiConsole.Confirm("Generate new ephemeral keypair for this game?")) {
        //     AnsiConsole.MarkupLine("Generating new keypair...");
        //     // generate a new keypair
        // } else {
        //     // ask for a path to an existing keypair
        // }

        // string wagerAsset = AnsiConsole.Ask<string>("Address of Wager Asset (default native asset)", "0x0000000000000000000000000000000000000000");
        // BigInteger wagerAmount = AnsiConsole.Ask<BigInteger>("How much to wager (in lowest denomination", 0);
        // int challengeDuration = AnsiConsole.Ask<int>("Channel challenge duration seconds (default 1 day)", 86400);

        // initial game state
        // select toshimon
        AnsiConsole.Write(new Rule("Select Toshimon Party"));

        // TODO make this an env var or something
        var db = new ToshimonDb("./toshimon.csv");

        for (int i = 0; i < 5; i++) {
            do {
                int toshimonNumber = AnsiConsole.Ask<int>(String.Format("Input Toshidex number for party member {0}:", i+1));
                renderMonster(db.findByToshidexNumber(toshimonNumber));
            } while (!AnsiConsole.Confirm(String.Format("Use this Toshimon?")));
        }

        return 0;
    }

    void renderMonster(MonsterRecord mon) {

        AnsiConsole.Write(new Rule(mon.Name));
        AnsiConsole.Write(new Panel(mon.Description));

        var typeTable = new Table();
        typeTable.AddColumn("Primary Type");
        typeTable.AddColumn("Secondary Type"); 
        typeTable.AddRow(((ToshimonType) mon.Type1).ToString(), ((ToshimonType) mon.Type2).ToString());
        AnsiConsole.Write(typeTable);

        var statTable = new Table();
        statTable.AddColumn("Stat");
        statTable.AddColumn("Value");
        // Add some rows
        statTable.AddRow("MaxHP", String.Format("{0}", mon.MaxHP));
        statTable.AddRow("Attack", String.Format("{0}", mon.Attack));
        statTable.AddRow("Defense", String.Format("{0}", mon.Defense));
        statTable.AddRow("Sp.Attack", String.Format("{0}", mon.SpAttack));
        statTable.AddRow("Sp.Defense", String.Format("{0}", mon.SpDefense));
        statTable.AddRow("Speed", String.Format("{0}", mon.Speed));

        statTable.Border(TableBorder.Rounded);

        // Render the statTable to the console
        AnsiConsole.Write(statTable);
    }
}