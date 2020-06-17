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
                logger.LogInformation("This is a test");
            }
        }

        logger.Eat(new Food { Name = "burger from chick-fil-a", Price = 5.99 });
        logger.Sleep();

        return 0;
        // as loggerFactory gets disposed, this will flush the logs
    }
}
