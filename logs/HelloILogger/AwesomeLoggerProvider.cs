using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

[ProviderAlias("Awesome")]
public class AwesomeLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    ConcurrentDictionary<string, AwesomeLogger> _loggers = new ConcurrentDictionary<string, AwesomeLogger>();
    IExternalScopeProvider _scopeProvider;
 
    void ISupportExternalScope.SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }
 
    public ILogger CreateLogger(string name)
    {
        return _loggers.GetOrAdd(name, name => new AwesomeLogger(name));
    }

    public void Dispose()
    {
    }

    internal IExternalScopeProvider ScopeProvider
    {
        get
        {
            if (_scopeProvider == null)
            {
                _scopeProvider = new LoggerExternalScopeProvider();
            }
            return _scopeProvider;
        }
    }
}
