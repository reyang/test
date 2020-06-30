using System;
using System.Collections.Generic;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;
using BenchmarkDotNet.Running;

class Program
{
    static int Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<TraceExporterBenchmark>();
        return 0;
    }
}

[EtwProfiler]
[MemoryDiagnoser]
public class TraceExporterBenchmark
{
    private Random r = new Random();
    private Activity activity;
    private ActivitySource sourceBoring = new ActivitySource("OpenTelemetry.Exporter.Geneva.Benchmark.Boring");
    private ActivitySource sourceTedious = new ActivitySource("OpenTelemetry.Exporter.Geneva.Benchmark.Tedious");

    public TraceExporterBenchmark()
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        ActivitySource.AddActivityListener(new ActivityListener {
            ActivityStarted = null,
            ActivityStopped = null,
            ShouldListenTo = (activitySource) => activitySource.Name == sourceTedious.Name,
            GetRequestedDataUsingParentId = (ref ActivityCreationOptions<string> options) => ActivityDataRequest.AllData,
            GetRequestedDataUsingContext = (ref ActivityCreationOptions<ActivityContext> options) => ActivityDataRequest.AllData,
        });
        using (var tedious = sourceTedious.StartActivity("Benchmark"))
        {
            activity = tedious;
        }
    }

    [Benchmark]
    public void CreateBoringActivity()
    {
        using (var activity = sourceBoring.StartActivity("Benchmark"))
        {
            // this activity won't be created as there is no listener
        }
    }

    [Benchmark]
    public void CreateTediousActivity()
    {
        using (var activity = sourceTedious.StartActivity("Benchmark"))
        {
            // this activity will be created and feed into an ActivityListener that simply drops everything on the floor
        }
    }
}
