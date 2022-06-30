using System.Numerics;
using System.Collections;
using Spectre.Console.Cli;
using Spectre.Console;
using Protocol;
using Nethereum.Util;
using Nethereum.Signer;

// [Description("Respond to a game proposal by constructing a message which initializes a new state channel")]
public sealed class PlayCommand : Command<PlayCommand.Settings>
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
        AnsiConsole.Write(new Rule("Respond to a state update"));

        // load the keyfile specified in KEYSTORE the env vars
        EthECKey key = Utils.loadKey(
            Environment.GetEnvironmentVariable("KEYSTORE"),
            Environment.GetEnvironmentVariable("KEYSTORE_PASSWORD")
        );

        // load the fixedPart/channel spec
        byte[] encodedFixedPart = File.ReadAllBytes(Environment.GetEnvironmentVariable("CHANNEL_DEF"));
        var channelDef = FixedPart.AbiDecode(encodedFixedPart);        

        Utils.renderChannelDef(channelDef);

        // figure which player index we are
        int whoami = channelDef.Participants.FindIndex(x => x == key.GetPublicAddress());
        if (whoami < 0) {
            AnsiConsole.WriteLine("The keypair loaded is not a participant in this channel. Did you load the wrong key? Exiting..");
            return 0;
        }

        // load the state update
        byte[] encodedSignedState = File.ReadAllBytes(settings.InputPath);
        var signedStateUpdate = SignedStateUpdate.AbiDecode(encodedSignedState);

        var variablePart = signedStateUpdate.VariablePart;
        ulong thisTurnNum = variablePart.TurnNum+1;

        AnsiConsole.WriteLine(String.Format("Turn Number: {0}", thisTurnNum));

        if (thisTurnNum % 2 != (ulong) whoami) {
            AnsiConsole.WriteLine("It is not this players turn to sign a new state update. Exiting.."); 
            return 0;           
        }

        // extract the game state and render
        var gameState = GameState.AbiDecode(variablePart.AppData);
        Utils.renderState(gameState, whoami);

        if (thisTurnNum == 1) {
            // If this is in the pre-fund phase then just increment the TurnNum, sign the update and return
            AnsiConsole.WriteLine("Channel is in the pre-fund phase. This message must be signed and sent before the opponent can deposit funds");            
            if(AnsiConsole.Confirm("Would you like to sign this update and confirm initialization of the channel?")) {
                thisTurnNum += 1;
                // sign and write to disk
            }
            return 0;
        } else if (thisTurnNum >= 2 && thisTurnNum <= 3) {
            // Post-fund phase. Player must check that the expected funds have been deposited
            // Can add an automatic check down the track
            AnsiConsole.WriteLine("Channel is in the post-fund phase. Only sign this message if you have observed that the channel is fully funded and the chain is finalized.");            
            if(AnsiConsole.Confirm("Would you like to sign this update and confirm initialization of the channel?")) {
                thisTurnNum += 1;
                // sign and write to disk
            }
        } else {
            // the actual gameplay phase
            GameAction action;
            switch (thisTurnNum % 4) {
                case 0: // Player A commit phase
                    action = promptForAction();
                    break;
                case 1: // Player B commit phase
                    action = promptForAction();
                    break;
                case 2: // Player A reveal phase
                    break;
                case 3: // Player B reveal phase
                    break;
            }
        }
        

        return 0;
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
}