using System.Numerics;
using System.Collections;
using Spectre.Console.Cli;
using Spectre.Console;
using Nethereum.Util;
using Nethereum.Signer;

using Protocol;
using Protocol.Adjudicator.Service;
using Protocol.Adjudicator.ContractDefinition;
using ToshimonDeployment;

// [Description("Display the status for a given channel")]
public sealed class StatusCommand : Command<StatusCommand.Settings>
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
        AnsiConsole.Write(new Rule("Channel Status"));

        ToshimonDeployment.ToshimonDeployment deployment = new ToshimonDeployment.ToshimonDeployment(settings.DeploymentPath);
        var channelDir = Path.GetFullPath(settings.ChannelPath);

        // load the fixedPart/channel spec
        byte[] encodedFixedPart = File.ReadAllBytes(Path.Combine(channelDir, "channelSpec"));
        var fixedPart = FixedPart.AbiDecode(encodedFixedPart);        

        Utils.renderChannelDef(fixedPart);

        var signedStateUpdate = Utils.loadHighestStateInDirectory(channelDir);

        var variablePart = signedStateUpdate.VariablePart;
        ulong thisTurnNum = variablePart.TurnNum+1;

        // query the chain to see if any challenges are lodged
        GetAndRenderStatus(deployment.AdjudicatorContractAddress, fixedPart.ChannelId, variablePart);

        var appData = AppData.AbiDecode(variablePart.AppData);
        var gameState = GameState.AbiDecode(appData.GameState);
        Utils.renderState(gameState, 0, deployment);

        return 0;
    }

    public static void GetAndRenderStatus(string contractAddress, byte[] channelId, VariablePart vp) {
        // query the chain to see if any challenges are lodged
        var (finalizedAt, turnNumRecord) = GetChannelStatus(contractAddress, channelId);
        renderChannelStatus(vp, finalizedAt, turnNumRecord);
    }

    public static (ulong, ulong) GetChannelStatus(string contractAddress, byte[] channelId) {
        var web3 = new Nethereum.Web3.Web3(Environment.GetEnvironmentVariable("ETH_RPC"));
        var service = new AdjudicatorService(web3, contractAddress);
        var result = service.UnpackStatusQueryAsync(
            channelId
        ).Result;
        return (result.FinalizesAt, result.TurnNumRecord);
    }

    public static void renderChannelStatus(VariablePart vp, ulong finalizedAt, ulong turnNumRecord) {

        // TODO add finalized status as well
        string status = finalizedAt > 0 ? "Challenge" : "Open";

        var table = new Table();
        table.HideHeaders();
        table.AddColumn("");
        table.AddColumn("");
        table.AddRow("TurnNum", vp.TurnNum.ToString());
        table.AddRow("Chain recorded TurnNum", turnNumRecord.ToString());
        table.AddRow("Channel Status", status);
        table.AddRow("Challenge Expires At", finalizedAt.ToString());

        AnsiConsole.Write(table);
    }

}