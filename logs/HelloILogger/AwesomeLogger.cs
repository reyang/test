using System;
using Microsoft.Extensions.Logging;

class AwesomeLogger : ILogger
{
    public AwesomeLogger(string name)
    {
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Console.WriteLine("This is awesome!");
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }
}
