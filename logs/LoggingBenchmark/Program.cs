using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;

class Program
{
    static void Main()
    {
        var summary = BenchmarkRunner.Run<LoggingBenchmark>();
    }
}

[MemoryDiagnoser]
public class LoggingBenchmark
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger logger;
    private bool disposed;

    public LoggingBenchmark()
    {
        this.loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(options =>
            {
                options.AddDummyLogExporter();
            });
        });

        this.logger = this.loggerFactory.CreateLogger<Program>();
    }

    [Benchmark]
    public void LogSingleString()
    {
        this.logger.LogInformation("Hello, World!");
    }

    [Benchmark]
    public void LogStringTemplateWithThreeParams()
    {
        this.logger.LogInformation("Hello, {a} {b} {c}.", 1, 2, 3);
    }

    [Benchmark]
    public void LogWithCompileTimeSourceGeneration()
    {
        this.logger.FoodRecallNotice(
            logLevel: LogLevel.Critical,
            brandName: "Contoso",
            productDescription: "Salads",
            productType: "Food & Beverages",
            productCode: 123,
            recallReasonDescription: "due to a possible health risk from Listeria monocytogenes",
            companyName: "Contoso Fresh Vegetables, Inc.");
    }

    [Benchmark]
    public void LogPayloadClass()
    {
        this.logger.Log(
            logLevel: LogLevel.Information,
            eventId: 1,
            state: new PayloadClass { },
            exception: null,
            formatter: (state, ex) => "Example formatted message."
        );
    }

    [Benchmark]
    public void LogPayloadStruct()
    {
        this.logger.Log(
            logLevel: LogLevel.Information,
            eventId: 2,
            state: new PayloadStruct { },
            exception: null,
            formatter: (state, ex) => "Example formatted message."
        );
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        if (disposing)
        {
            this.loggerFactory?.Dispose();
        }

        this.disposed = true;
    }
}
