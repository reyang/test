﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

class AwesomeLogger : ILogger
{
    public AwesomeLogger(string name)
    {
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Console.WriteLine("This is awesome!");
        if (state is IReadOnlyCollection<KeyValuePair<string, object>> dict)
        {
            foreach (var entry in dict)
            {
                Console.WriteLine($"{entry.Key}: {entry.Value}");
            }
        }
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
