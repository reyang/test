using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

internal class ReileyScopeProvider : LoggerExternalScopeProvider, IExternalScopeProvider
{
    public new void ForEachScope<TState>(Action<object, TState> callback, TState state)
    {
        callback("[global]", state);
        callback("[regional]", state);
        base.ForEachScope(callback, state);
    }
}

class Program
{
    static int Main(string[] args)
    {
        var services = new ServiceCollection()
            .AddLogging(builder => {
                builder.AddConsole(options => { options.IncludeScopes = true; });
            })
            .AddSingleton<ISupportExternalScope, ConsoleLoggerProvider>()
            .BuildServiceProvider();
        var loggerProvider = services.GetRequiredService<ISupportExternalScope>();
        loggerProvider.SetScopeProvider(new ReileyScopeProvider());
        
        var logger = ((ConsoleLoggerProvider)loggerProvider).CreateLogger("Program");
        
        using (logger.BeginScope("[operation]"))
        using (logger.BeginScope("[hardware]"))
        {
            logger.LogInformation("Hello, world!");
        }
        (services as IDisposable)?.Dispose(); // this will flush the logs
        return 0;
    }
}
