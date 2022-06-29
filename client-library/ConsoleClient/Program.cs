using Spectre.Console.Cli;

public static class Program
{
    public static int Main(string[] args)
    {
		var app = new CommandApp();
        app.Configure(config =>
        {
            config.SetApplicationName("toshimon-console-client");
            config.AddCommand<PlayCommand>("play");
            config.AddCommand<CreateProposalCommand>("new");
        });		
		return app.Run(args);
    }
}
