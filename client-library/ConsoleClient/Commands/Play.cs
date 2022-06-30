using Spectre.Console.Cli;
using Spectre.Console;
using Protocol;

public sealed class PlayCommand : Command<PlayCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [CommandOption("-i|--input")]
        // [Description("Path to input file containg an encoded state object")]
        public string? InputPath { get; init; }

        [CommandOption("-o|--output")]
        // [Description("Path to write next state file to")]
        public string? OutputPath { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        renderState(build1v1(testMonster1(), testMonster1()), 0);
        promptForAction();
        return 0;
    }

    // Render the state to the console from the perspective of the player whoami
    public static void renderState(GameState state, int whoami) {
        // Create a table
        var table = new Table();

        PlayerState me = state[whoami];
        MonsterCard myMonster = getActiveMonster(me);
        PlayerState other = state[not(whoami)];
        MonsterCard otherMonster = getActiveMonster(other);

        var opponentHp = new BarChart()
        .AddItem("HP", otherMonster.Stats.Hp, Color.Red)
        .WithMaxValue(otherMonster.BaseStats.Hp);

        var ownHp = new BarChart()
        .AddItem("HP", myMonster.Stats.Hp, Color.Green)
        .WithMaxValue(myMonster.BaseStats.Hp);

        // Add some columns
        table.AddColumn("");
        table.AddColumn("");

        // Add some rows
        table.AddRow(String.Format("Enemy {0}", otherMonster.CardId), "");
        table.AddRow(new Panel(opponentHp), new Panel(""));
        table.AddRow("","");
        table.AddRow("", String.Format("Your {0}", myMonster.CardId));
        table.AddRow(new Panel(""), new Panel(ownHp));

        table.Border(TableBorder.Rounded);
        table.HideHeaders();
        table.Expand();

        // Render the table to the console
        AnsiConsole.Write(table);
    }

    // prompt the user and get a Toshimon move
    public static GameAction promptForAction() {
        var rule = new Rule("Select an action");
        rule.LeftAligned();
        AnsiConsole.Write(rule);

        // Top level action to take
        var topAction = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
            .AddChoices(new[] {
                "Attack", "Switch", "Item", "Run"
                }));


        switch (topAction) {
            case "Attack":
            return AnsiConsole.Prompt(
                new SelectionPrompt<GameAction>()
                .Title("Select Attack")
                .AddChoices(new[] {
                    GameAction.Move1, GameAction.Move2, GameAction.Move3, GameAction.MoveTMM,
                    }));
            case "Switch":
            return AnsiConsole.Prompt(
                new SelectionPrompt<GameAction>()
                .Title("Who would you like to swap?")
                .AddChoices(new[] {
                    GameAction.Swap1, GameAction.Swap2, GameAction.Swap3, GameAction.Swap4, GameAction.Swap5,
                    }));    
            case "Item":
            return AnsiConsole.Prompt(
                new SelectionPrompt<GameAction>()
                .Title("Select Item")
                .AddChoices(new[] {
                    GameAction.Item1, GameAction.Item2, GameAction.Item3, GameAction.Item4, GameAction.Item5,
                    }));            
            default:
            return GameAction.Noop;
        }
    }

    public static MonsterCard testMonster1() {
        Stats stats = new Stats {
            Hp = 70,
            Attack = 70,
            Defense = 70,
            SpAttack = 90,
            SpDefense = 90,
            Speed = 90,
            PP = new List<uint>(new uint[]{10, 0, 0, 0}),
        };

        return new MonsterCard(
            stats,
            stats,
            new string[]{ }
            );
    }

    // build a game state quickly where each player only has one monster
    public static GameState build1v1(MonsterCard one, MonsterCard two) {
        return new GameState(
            new PlayerState(new MonsterCard[]{one}, new ItemCard[0]),
            new PlayerState(new MonsterCard[]{two}, new ItemCard[0])
            );
    }

    private static int not(int i) {
        return i switch {
            0 => 1,
            1 => 0,
            _ => throw new IndexOutOfRangeException(),
        };
    }

    private static MonsterCard getActiveMonster(PlayerState player) {
        return player.Monsters[player.ActiveMonsterIndex];
    }
}