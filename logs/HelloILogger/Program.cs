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
            // Logging using a manual scope
            using (logger.BeginScope(new { A = 1 }))
            {
                using (logger.BeginScope(new { B = 1 }))
                {
                    logger.LogInformation("This is a test");
                }
                logger.Eat(new Food { Name = "artichoke", Price = 3.99 });
                logger.LogEx(new Dictionary<string, object>{
                    ["foo"] = "1",
                    ["bar"] = "2",
                });
            }
        }

        logger.Sleep();

        return 0;
        // as loggerFactory gets disposed, this will flush the logs
    }
}

/*
Challenges:
* How do we allow people to write their own strong type logs with low cost / good perf?
* How to enable multiple log streams, sending to different places (e.g. Audit Logs)? Having multiple factories?
* How to discover and subscribe to logs that are provided by another lib which you don't own?
* How to allow arbitrary Part C fields?
* What's the guidance over EventSource vs. ILogger?
* What's the guidance for logging scope?
*/
