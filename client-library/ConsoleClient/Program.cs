using Spectre.Console.Cli;

public static class Program
{
    public static int Main(string[] args)
    {
        // grab env vars from .env
        var root = Directory.GetCurrentDirectory();
        var dotenv = Path.Combine(root, ".env");
        DotEnv.Load(dotenv);

		var app = new CommandApp();
        app.Configure(config =>
        {
            #if DEBUG
                config.PropagateExceptions();
                config.ValidateExamples();
            #endif
            config.SetApplicationName("toshimon-console-client");
            config.AddCommand<PlayCommand>("play");
            config.AddCommand<CreateProposalCommand>("propose");
            config.AddCommand<AcceptProposalCommand>("accept");
        });		

		return app.Run(args);
    }
}
