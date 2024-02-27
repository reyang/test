using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.Extensions.Logging;

[MemoryDiagnoser]
public class Program
{
    private static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => { builder.ClearProviders(); });
    private static readonly ILogger MyLogger = MyLoggerFactory.CreateLogger<Program>();
    private static readonly string FoodName = "tomato";
    private const double FoodPrice = 2.99;

    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Program>();
    }

    [Benchmark]
    public void LogWithStringInterpolation()
    {
        MyLogger.LogInformation($"Hello from {FoodName} {FoodPrice}.");
    }

    [Benchmark]
    public void LogWithMessageTemplate()
    {
        MyLogger.LogInformation("Hello from {FoodName} {FoodPrice}.", FoodName, FoodPrice);
    }

    [Benchmark]
    public void LogWithCompileTimeCodeGeneration()
    {
        MyLogger.SayHello(FoodName, FoodPrice);
    }

    [Benchmark]
    public void LogWithNewlyCreatedLogger()
    {
        var logger = MyLoggerFactory.CreateLogger<Program>();
        logger.SayHello(FoodName, FoodPrice);
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        MyLoggerFactory.Dispose();
    }
}

internal static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Hello from {food} {price}.")]
    public static partial void SayHello(this ILogger logger, string food, double price);
}

