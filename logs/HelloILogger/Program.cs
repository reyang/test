using Microsoft.Extensions.Logging;

class Program
{
    static int Main(string[] args)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogInformation("Hello, world!");
        loggerFactory.Dispose(); // this will flush the logs
        return 0;
    }
}
