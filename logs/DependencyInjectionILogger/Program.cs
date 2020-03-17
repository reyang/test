using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

class Program
{
    static int Main(string[] args)
    {
        var services = new ServiceCollection()
            .AddLogging(builder => {
                builder.AddConsole();
            })
            .BuildServiceProvider();
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Hello, world!");
        (services as IDisposable)?.Dispose(); // this will flush the logs
        return 0;
    }
}
