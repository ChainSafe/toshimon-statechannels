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

// [Description("Submit a tx to update the on-chain channel state. Can be used to respond to a challenge.")]
public sealed class CheckpointCommand : Command<CheckpointCommand.Settings>
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
        AnsiConsole.Write(new Rule("Submit a Checkpoint"));

        ToshimonDeployment.ToshimonDeployment deployment = new ToshimonDeployment.ToshimonDeployment(settings.DeploymentPath);
        var channelDir = Path.GetFullPath(settings.ChannelPath);

        // load the fixedPart/channel spec
        byte[] encodedFixedPart = File.ReadAllBytes(Path.Combine(channelDir, "channelSpec"));
        var fixedPart = FixedPart.AbiDecode(encodedFixedPart);        

        Utils.renderChannelDef(fixedPart);

        // load up a funded account to submit the transaction
        // and pay the gas. this need not be the same as the signer
        Account acc = Utils.promptAccountFromPrivateKey();

        var lastSignedState = Utils.loadHighestStateInDirectory(channelDir);
        
        // Create a supported state proof by combining this most recent state with the prior one signed
        // by the other player
        var supportProof = new List<SignedVariablePart>() { 
            Utils.loadHighestStateInDirectory(channelDir, 1), 
            lastSignedState
        };

        // submit with a call to challenge!
        var web3 = new Nethereum.Web3.Web3(acc, Environment.GetEnvironmentVariable("ETH_RPC"));
        web3.TransactionManager.UseLegacyAsDefault = true;
        var service = new AdjudicatorService(web3, deployment.AdjudicatorContractAddress);

        var result = service.CheckpointRequestAsync(
            fixedPart,
            supportProof
        ).Result;

        AnsiConsole.WriteLine("Checkpoint Success. Tx: {0}", result);

        StatusCommand.GetAndRenderStatus(deployment.AdjudicatorContractAddress, fixedPart.ChannelId, lastSignedState.VariablePart);

        return 0;
    }

}