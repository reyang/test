# Logging Challenges

## Current State

### DiagnosticSource, EventSource, ILogger

* `EventSource` is a fast and strongly typed interface for OS logging systems. It was originally developed for ETW, later added support for LTTng. More details can be found from [.NET Core logging and tracing](https://docs.microsoft.com/dotnet/core/diagnostics/logging-tracing).

* [`DiagnosticSource`](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.diagnosticsource) is similar to `EventSource`. More details can be found from the [DiagnosticSource User's Guide](https://github.com/dotnet/runtime/blob/master/src/libraries/System.Diagnostics.DiagnosticSource/src/DiagnosticSourceUsersGuide.md).

* `ILogger` is an extensible logging API supporting custom plugins. ILogger requires [.NET Standard 2.0](https://dotnet.microsoft.com/platform/dotnet-standard), which means it **does not** work with .NET `4.5.2` and .NET `4.6`. ILogger is integrated with `Activity`, which means the distributed tracing information can be associated with log entries.

    ```csharp
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.Extensions.Logging;

    class Program
    {
        static int Main(string[] args)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            var source = new ActivitySource("Demo");
            ActivitySource.AddActivityListener(new ActivityListener
            {
                ActivityStarted = null,
                ActivityStopped = null,
                ShouldListenTo = source => true,
                GetRequestedDataUsingParentId = (ref ActivityCreationOptions<string> options) => ActivityDataRequest.AllData,
                GetRequestedDataUsingContext = (ref ActivityCreationOptions<ActivityContext> options) => ActivityDataRequest.AllData,
            });

            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddAwesome();
                builder.AddConsole(options => { options.IncludeScopes = true; });
                builder.Configure(options => options.ActivityTrackingOptions =
                    ActivityTrackingOptions.TraceId |
                    ActivityTrackingOptions.SpanId |
                    ActivityTrackingOptions.ParentId
                );
            });
            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("Hello, world!");

            using (var activity = source.StartActivity("Foo"))
            {
                logger.LogInformation("Hello with in an ActivityContext!");
            }

            return 0;
            // as loggerFactory gets disposed, this will flush the logs
        }
    }
    ````

## Challenges and Questions

* For customers who are using `EventSource` today, should they continue to use it or the path forward will be `ILogger`? Under which scenario should the customer use a particular interface?

* `EventSource` on Linux depends on LTTng, LTTng requires additional driver to be installed on the Linux machine/VM which could be a concern (maintenance, permission, security). Is there a plan to support something else?

    [Noah] Event Pipe (Unix Domain Socket), or the Event Listener.
    https://devblogs.microsoft.com/dotnet/introducing-dotnet-monitor/

* Customers who have existing instrumentation using `EventSource` would want to have an option to automatically associate logs with `TraceId` and `SpanId` (similar like what ILogger has), would we consider adding such support?

    [Noah] Need to explore, probably we can improve the tooling (on Geneva side) so the customer doesn't have to make code changes and get it for free. Potential way is to update the ETW correlation GUID during Activity creation/destruction.

* How do we allow people to write their own strong type logs with low cost / good perf?

    ILogger has strong type extensibility in the following way, I wonder if we could do something to remove step 2 or make it easier.

    1. Define your own strong type:

        ```csharp
        public struct Food
        {
            public string Name { get; set; }
            public double Price { get; set; }
        }
        ```

    2. Write the boilerplate ILogger extension:

        ```csharp
        using System;
        using Microsoft.Extensions.Logging;

        public static class LoggerExtensions
        {
            private static readonly Action<ILogger, Food, Exception> _eat = LoggerMessage.Define<Food>(
                LogLevel.Information,
                new EventId(1, nameof(Eat)),
                "Eat (Food = {food})."); // message template, not interpolated string

            public static void Eat(this ILogger logger, Food food)
            {
                _eat(logger, food, null);
            }
        }
        ```

    3. Write logs:

        ```csharp
        logger.Eat(new Food { Name = "artichoke", Price = 3.99 });
        ```

    4. The ILogger implementation would need a way to access the custom data type, what would be the efficient way rather than using reflection?

        ```csharp
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (state is IReadOnlyCollection<KeyValuePair<string, object>> dict)
            {
                Console.WriteLine("TState:");
                foreach (var entry in dict)
                {
                    Console.WriteLine($"    {entry.Key}: {entry.Value}"); // how to access strong type value instead of stick to a serializer?
                }
            }
        }
        ```

* How to enable multiple log streams, sending to different places (e.g. Audit Logs)? Having multiple factories?

    [Noah] use single factory, register multiple handlers, implement routing use filter rules (at factory level, or individual logger level).

    ```javascript
    {
        "Logging": {      // Default, all providers.
            "LogLevel": {
                "Microsoft": "Warning"
            },
            "Console": { // Console provider.
                "LogLevel": {
                    "Microsoft": "Information"
                }
            }
        }
    }
    ```

* How to discover and subscribe to logs that are provided by another lib which you don't own?

    [Sourabh] There is a "default" filter - as long as the library has access to the default static logger factory instance.

    [Noah] ILogger has a different philosophy - nothing should be global by default. A piece of code could create its own of **secret** stream of logs without others knowing.

    [Sourabh] Suggest to take a look at Azure SDKs as they have covered both the DI and explicit factory approaches.

* How to allow arbitrary custom fields? Should we use logging scope? What's the guidance for logging scope?

    ```csharp
    using (logger.BeginScope(new { Tips = 0.5 }))
    {
        logger.Eat(new Food { Name = "artichoke", Price = 3.99 });
    }
    ```

    * option 1: use logger scope (hacky)
        * [Sourabh] Do not abuse scope.
        * [Noah] Perf might hurt.
    * option 2: put a key-value pair in the strong type
    * option 3: put another parameter (key-value pair), or variadic argument style.

Follow up:

1. post the issue to dotnet runtime repo.
2. break it into smaller pieces / multiple issues (e.g. strong type worth its own issue?).

## Case Study

### Geneva IFx SDK

Strong type definition, using [bond](https://github.com/microsoft/bond).

```cpp
import "Ifx.bond"

namespace CustomSchemaDemo

struct DogBreed : Ifx.PartASchema
{
    10: required wstring Name;
    20: required double AverageWeightInLbs;
    30: required uint32 Popularity;
    40: required wstring Temperament;
}
```

Compile the bond definition and consume it from C#:

```csharp
using Microsoft.Cloud.InstrumentationFramework;

class Program
{
    static void Main(string[] args)
    {
        IfxInitializer.IfxInitialize("session name");

        DogBreed lab = new DogBreed
        {
            Name = @"Labrador Retriever",
            AverageWeightInLbs = 71.5,
            Popularity = 1,
            Temperament = @"Kind, Outgoing, Agile, Gentle, Intelligent, Event Tempered, Trusting"
        };

        IfxEvent.Log(lab);
    }
}
```

Downsides:

* There is dependency on 3rd party library and toolchain, ideally we want the instrumentation to be provided purely by .NET.
* Vendor neutral libraries cannot take this approach since different backend services could use different serialiation formats. Logs can only be emitted to Geneva.
* Adding per event key-value pair is impossible, one has to change the type definition.

### OpenCensus Python SDK

The Python built-in logging API is used, there is no OpenCensus logging API. Instrumentation can be done without depending on any 3rd party library.

Log handlers can be added to any logger, or all the loggers.

It is easy to have multiple log streams routed to different destination, for example:

* Audit logs sent to Windows Security Log or syslog.
* Debug logs sent to ETW.
* Critical error / exceptions sent to a cloud service.

```python
import logging

from opencensus.ext.azure.log_exporter import AzureLogHandler

logger = logging.getLogger(__name__)
logger.addHandler(AzureLogHandler(connection_string="InstrumentationKey=your-key"))
logger.warning('Hello, World!')
```

Logs can be [correlated](https://github.com/census-instrumentation/opencensus-python/tree/master/contrib/opencensus-ext-logging) with trace id and span id with a single line of code.

```python
import logging
from opencensus.trace import config_integration

config_integration.trace_integrations(['logging'])

logger = logging.getLogger(__name__)
tracer = Tracer()

logger.warning('Before the span')
with tracer.span(name='test'):
    logger.warning('In the span')
logger.warning('After the span')
```

Exception logging is integrated, the `logger.exception` invocation will automatically capture the exception and callstack.

```python
import logging

logger = logging.getLogger(__name__)

try:
    return 1 / 0  # generate a ZeroDivisionError
except Exception:
    logger.exception('Captured an exception.')
```
