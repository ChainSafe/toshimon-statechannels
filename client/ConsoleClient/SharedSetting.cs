using Spectre.Console.Cli;
using Spectre.Console;

public class SharedSettings : CommandSettings
{
    [CommandOption("-d|--deployment-path")]
    public string? DeploymentPath { get; }

    [CommandOption("-r|--eth-rpc")]
    public string? RpcUrl { get; }


    public SharedSettings(string? deploymentPath, string? rpcUrl)
    {
        DeploymentPath = deploymentPath ?? Environment.GetEnvironmentVariable("DEPLOYMENT");
        RpcUrl = rpcUrl ?? Environment.GetEnvironmentVariable("ETH_RPC");
    }
}
