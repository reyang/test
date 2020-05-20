using Microsoft.Extensions.Logging;

class Program
{
    static int Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole(options => { options.IncludeScopes = true; });
        });
        var logger = loggerFactory.CreateLogger<Program>();

        logger.LogInformation("Hello, world!");

        // Logging using a manual scope
        using (logger.BeginScope(new { A = 1 }))
        {
            logger.LogInformation("This is a test");
        }

        return 0;
        // as loggerFactory gets disposed, this will flush the logs
    }
}
