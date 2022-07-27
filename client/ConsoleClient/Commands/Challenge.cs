using System.Numerics;
using System.Collections;
using Spectre.Console.Cli;
using Spectre.Console;
using Nethereum.Util;
using Nethereum.Signer;
using Nethereum.Web3.Accounts;

using Protocol;
using Protocol.Adjudicator.Service;
using Protocol.Adjudicator.ContractDefinition;
using ToshimonDeployment;

// [Description("Submit a tx to put the channel in a challenge state and force the other player to move")]
public sealed class ChallengeCommand : Command<ChallengeCommand.Settings>
{
    public sealed class Settings : SharedSettings
    {
        [CommandOption("-c|--channelPath")]
        public string? ChannelPath { get; init; }

        public Settings(string? deploymentPath, string? rpcUrl, string? channelPath) : base(deploymentPath, rpcUrl)
        {
            ChannelPath = channelPath ?? Environment.GetEnvironmentVariable("CHANNEL_PATH");
        }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        AnsiConsole.Write(new Rule("Submit a Challenge"));

        ToshimonDeployment.ToshimonDeployment deployment = new ToshimonDeployment.ToshimonDeployment(settings.DeploymentPath);
        var channelDir = Path.GetFullPath(settings.ChannelPath);

        // load the fixedPart/channel spec
        byte[] encodedFixedPart = File.ReadAllBytes(Path.Combine(channelDir, "channelSpec"));
        var fixedPart = FixedPart.AbiDecode(encodedFixedPart);        

        Utils.renderChannelDef(fixedPart);

        // load the channel signers key
        AnsiConsole.WriteLine("Load the key used for this channel (ephemeral key)");
        EthECKey signerKey = Utils.promptLoadKey();

        // load up a funded account to submit the transaction
        // and pay the gas. this need not be the same as the signer
        Account acc = Utils.promptAccountFromPrivateKey();

        // find the most recent state we have and check if it is by this player.
        // It is important for turn-taking channels that the other player can respond
        // by making their move. Otherwise the player is effectively forcing themselve to 
        // move which is pointless
        //
        // If the last state is not by this player then prompt them to sign a new state update first

        // figure which player index we are
        int whoami = fixedPart.Participants.FindIndex(x => x == signerKey.GetPublicAddress());
        if (whoami < 0) {
            AnsiConsole.WriteLine("The keypair loaded is not a participant in this channel. Did you load the wrong key? Exiting..");
            return 0;
        }

        var lastSignedState = Utils.loadHighestStateInDirectory(channelDir);
        ulong lastTurnNum = lastSignedState.VariablePart.TurnNum;

        if (lastTurnNum % 2 != (ulong) whoami) {
            AnsiConsole.WriteLine("The most recent state was not made by this player. Make a move first then propose a challenge. Exiting.."); 
            return 0;           
        }
        
        // // HACK!!
        // // re-encode the GameState using the chain-side call
        // lastSignedState.VariablePart.AppData = 

        // Create a supported state proof by combining this most recent state with the prior one signed
        // by the other player
        var supportProof = new List<SignedVariablePart>() { Utils.loadHighestStateInDirectory(channelDir, 1), lastSignedState };

        // sign a challenge message (state hash with special string appended) to prove ownership of the challengers key
       var challengeSig = lastSignedState.VariablePart.GetChallengeSignature(fixedPart, signerKey);

        // submit with a call to challenge!
        var web3 = new Nethereum.Web3.Web3(acc, Environment.GetEnvironmentVariable("ETH_RPC"));
        web3.TransactionManager.UseLegacyAsDefault = true;
        var service = new AdjudicatorService(web3, deployment.AdjudicatorContractAddress);

        var result = service.ChallengeRequestAsync(
            fixedPart,
            supportProof,
            challengeSig
        ).Result;

        AnsiConsole.WriteLine("Challenge Success. Tx: {0}", result);

        StatusCommand.GetAndRenderStatus(deployment.AdjudicatorContractAddress, fixedPart.ChannelId, lastSignedState.VariablePart);

        return 0;
    }

}