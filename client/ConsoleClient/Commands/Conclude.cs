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

// [Description("Submit a tx to put the channel in the concluded state and allow withdrawal of funds")]
public sealed class ConcludeCommand : Command<ConcludeCommand.Settings>
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
        AnsiConsole.Write(new Rule("Conclude a channel"));

        ToshimonDeployment.ToshimonDeployment deployment = new ToshimonDeployment.ToshimonDeployment(settings.DeploymentPath);
        var channelDir = Path.GetFullPath(settings.ChannelPath);

        // load the fixedPart/channel spec
        byte[] encodedFixedPart = File.ReadAllBytes(Path.Combine(channelDir, "channelSpec"));
        var fixedPart = FixedPart.AbiDecode(encodedFixedPart);        

        Utils.renderChannelDef(fixedPart);

        // load up a funded account to submit the transaction
        // and pay the gas. this need not be the same as the signer
        Account acc = Utils.promptAccountFromPrivateKey();
        
        // Create a supported conclusion proof. This is the same as a regular state 
        // support proof except that the highest supported state must have isFinal=true
        var lastSignedState = Utils.loadHighestStateInDirectory(channelDir);

        if (!lastSignedState.VariablePart.IsFinal) {
            AnsiConsole.WriteLine("The channel has not reached a conclusion locally therefore a conclusion proof cannot be constructed.");
            return 0;
        }

        var supportedConclusionProof = new List<SignedVariablePart>() { Utils.loadHighestStateInDirectory(channelDir, 1), lastSignedState };

        // submit with a call to challenge!
        var web3 = new Nethereum.Web3.Web3(acc, Environment.GetEnvironmentVariable("ETH_RPC"));
        web3.TransactionManager.UseLegacyAsDefault = true;
        var service = new AdjudicatorService(web3, deployment.AdjudicatorContractAddress);

        if(AnsiConsole.Confirm(String.Format("Would you like to also transfer all assets stored in the channel?"))) {
            var result = service.ConcludeAndTransferAllAssetsRequestAsync(
                fixedPart,
                supportedConclusionProof
            ).Result;
            AnsiConsole.WriteLine("Conclude success. Tx: {0}", result);
        } else {
            var result = service.ConcludeRequestAsync(
                fixedPart,
                supportedConclusionProof
            ).Result;
            AnsiConsole.WriteLine("Conclude success. Tx: {0}", result);
        }

        StatusCommand.GetAndRenderStatus(deployment.AdjudicatorContractAddress, fixedPart.ChannelId, lastSignedState.VariablePart);

        return 0;
    }

}