using System.Numerics;
using System.Collections;
using Spectre.Console.Cli;
using Spectre.Console;
using Nethereum.Util;
using Nethereum.Signer;

using Protocol;
using Protocol.ToshimonStateTransition.Service;
using ToshimonDeployment;

// [Description("Respond to a game proposal by constructing a message which initializes a new state channel")]
public sealed class PlayCommand : Command<PlayCommand.Settings>
{
    public sealed class Settings : SharedSettings
    {
        [CommandOption("-c|--channelPath")]
        public string? ChannelPath { get; init; }

        [CommandOption("-k|--keystore")]
        public string? KeyStore { get; init; }

        [CommandOption("-p|--password")]
        public string? KeyStorePassword { get; init; }

        public Settings(string? deploymentPath, string? rpcUrl, string? channelPath, string? keystore, string? keystorePassword) : base(deploymentPath, rpcUrl)
        {
            ChannelPath = channelPath ?? Environment.GetEnvironmentVariable("CHANNEL_PATH");
            KeyStore = keystore ?? Environment.GetEnvironmentVariable("KEYSTORE");
            KeyStorePassword = keystorePassword ?? Environment.GetEnvironmentVariable("KEYSTORE_PASSWORD");
        }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(new Rule("Respond to a state update"));

        ToshimonDeployment.ToshimonDeployment deployment = new ToshimonDeployment.ToshimonDeployment(settings.DeploymentPath);

        // load the keyfile
        EthECKey key = Utils.loadKey(
            settings.KeyStore,
            settings.KeyStorePassword
        );

        var channelDir = Path.GetFullPath(settings.ChannelPath);

        // load the fixedPart/channel spec
        byte[] encodedFixedPart = File.ReadAllBytes(Path.Combine(channelDir, "channelSpec"));
        var fixedPart = FixedPart.AbiDecode(encodedFixedPart);        

        Utils.renderChannelDef(fixedPart);

        // figure which player index we are
        int whoami = fixedPart.Participants.FindIndex(x => x == key.GetPublicAddress());
        if (whoami < 0) {
            AnsiConsole.WriteLine("The keypair loaded is not a participant in this channel. Did you load the wrong key? Exiting..");
            return 0;
        }

        // assume we are playing off the highest existing state message
        var highestFile = Directory.EnumerateFiles(channelDir, "*.state")
                                .OrderBy(f => int.Parse(Path.GetFileNameWithoutExtension(f)))
                                .Last();

        // load the state update
        byte[] encodedSignedState = File.ReadAllBytes(Path.Combine(channelDir, highestFile));
        var signedStateUpdate = SignedStateUpdate.AbiDecode(encodedSignedState);

        var variablePart = signedStateUpdate.VariablePart;
        ulong thisTurnNum = variablePart.TurnNum+1;

        AnsiConsole.WriteLine(String.Format("Turn Number: {0}", thisTurnNum));

        if (thisTurnNum % 2 != (ulong) whoami) {
            AnsiConsole.WriteLine("It is not this players turn to sign a new state update. Exiting.."); 
            return 0;           
        }

        var newStateOutputPath = Path.Combine(channelDir, String.Format("{0}.state", thisTurnNum));

        // extract the game state and render
        var appData = AppData.AbiDecode(variablePart.AppData);
        var gameState = GameState.AbiDecode(appData.GameState);
        Utils.renderState(gameState, whoami);

        if (thisTurnNum == 1) {
            // If this is in the pre-fund phase then just increment the TurnNum, sign the update and return
            AnsiConsole.WriteLine("Channel is in the pre-fund phase. This message must be signed and sent before the opponent can deposit funds");            
            if(AnsiConsole.Confirm("Would you like to sign this update and confirm participation in the channel?")) {
                var nextVariablePart = variablePart with { TurnNum = thisTurnNum };
                Utils.signAndWriteUpdate(fixedPart, nextVariablePart, key, newStateOutputPath);
            }
            return 0;
        } else if (thisTurnNum >= 2 && thisTurnNum <= 3) {
            // Post-fund phase. Player must check that the expected funds have been deposited
            // Can add an automatic check down the track
            AnsiConsole.WriteLine("Channel is in the post-fund phase. Only sign this message if you have observed that the channel is fully funded and the chain is finalized.");            
            if(AnsiConsole.Confirm("Would you like to sign this update and confirm initialization of the channel?")) {
                var nextVariablePart = variablePart with { TurnNum = thisTurnNum };
                Utils.signAndWriteUpdate(fixedPart, nextVariablePart, key, newStateOutputPath);
            }
        } else {
            // the actual gameplay phase
            Reveal reveal;
            switch (thisTurnNum % 4) {
                case 0: // Player A commit phase            
                    AnsiConsole.WriteLine("Commit phase for Player A.");
                    reveal = getReveal(deployment, gameState[0]);
                    storeReveal(reveal, channelDir, "A.reveal");
                    appData.PreCommitA = reveal.CommitHash;
                break;
                case 1: // Player B commit phase
                    AnsiConsole.WriteLine("Commit phase for Player B.");     
                    reveal = getReveal(deployment, gameState[1]); 
                    storeReveal(reveal, channelDir, "B.reveal");      
                    appData.PreCommitB = reveal.CommitHash;
                break;
                case 2: // Player A reveal phase
                    AnsiConsole.WriteLine("Reveal phase for Player A.");  
                    appData.RevealA = loadReveal(channelDir, "A.reveal"); 
                    AnsiConsole.WriteLine("Loaded commit from file. No further action required.");  
                break;
                case 3: // Player B reveal and game state update phase
                    AnsiConsole.WriteLine("Reveal phase for Player B. Attempts to load commit from file.");            
                    appData.RevealB = loadReveal(channelDir, "B.reveal");
                    AnsiConsole.WriteLine("Loaded commit from file. No further action required.");  
                    // state transition!!
                    (GameState newState, _, _) = advanceState(deployment, gameState, variablePart.Outcome.ToArray(), new byte[]{ appData.RevealA.Move, appData.RevealB.Move }, 0);
                    appData.GameState = newState.AbiEncode();
                break;
            }

            var nextVariablePart = variablePart with { TurnNum = thisTurnNum, AppData = appData.AbiEncode() };
            Utils.signAndWriteUpdate(fixedPart, nextVariablePart, key, newStateOutputPath);
        }
        
        return 0;
    }


    private static Reveal getReveal(ToshimonDeployment.ToshimonDeployment deployment, PlayerState playerState) {
        GameAction action = promptForAction(deployment, playerState);
        BigInteger randomSeed = (BigInteger) new Random().Next();
        return new Reveal() {
            Move = (byte) action,
            Seed = randomSeed,
        };
    }

    private static void storeReveal(Reveal reveal, string channelDir, string filename) {
        using (Stream s = File.Create(Path.Combine(channelDir, filename)) ) {
            byte[] bytes = reveal.AbiEncode();
            s.Write(bytes, 0, bytes.Length);
        }
    }

    private static Reveal loadReveal(string channelDir, string filename) {
        byte[] revealBytes = File.ReadAllBytes(Path.Combine(channelDir, filename));
        return Reveal.AbiDecode(revealBytes);
    }

    /**
     * Actually make a call to an EVM RPC node in order to make the state transition.
     * This ensures that the new state will be identical to that calculated by the adjudicator
     */
    private static (GameState, SingleAssetExit[], bool) advanceState(
        ToshimonDeployment.ToshimonDeployment deployment,
        GameState gameState,
        SingleAssetExit[] outcome,
        byte[] actions,
        uint randomSeed) {

        var web3 = new Nethereum.Web3.Web3(Environment.GetEnvironmentVariable("ETH_RPC"));
        var service = new ToshimonStateTransitionService(web3, deployment.StateTransitionContractAddress);
        
        var result = service.AdvanceStateTypedQueryAsync(
            gameState,
            new List<SingleAssetExit>(outcome),
            (byte) actions[0],
            (byte) actions[1],
            Enumerable.Repeat((byte) 0, 32).ToArray()
        ).Result; // .Result blocks on async function. Can modify to be async if desired

        return (
            result.ReturnValue1,
            result.ReturnValue2.ToArray(),
            result.ReturnValue3
        );
    }


    // prompt the user and get a Toshimon move
    private static GameAction promptForAction(ToshimonDeployment.ToshimonDeployment deployment, PlayerState playerState) {
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
                var moveNames = playerState.Monsters[playerState.ActiveMonsterIndex].Moves.Select(addr => deployment.getMoveByAddress(addr)?.Name ?? "-").ToArray();
                return AnsiConsole.Prompt(
                    new SelectionPrompt<GameAction>()
                    .Title("Select Attack")
                    .AddChoices(new[] {
                        GameAction.Move1, GameAction.Move2, GameAction.Move3, GameAction.MoveTMM,
                    })
                    .UseConverter(x => moveNames[(int) x])
                );
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