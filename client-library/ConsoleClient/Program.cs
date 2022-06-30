using Spectre.Console.Cli;

public static class Program
{
    public static int Main(string[] args)
    {
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
