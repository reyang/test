using OpenTelemetry.Trace.Export;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class DemoExporter : SpanExporter
{
    public override Task<ExportResult> ExportAsync(IEnumerable<SpanData> data, CancellationToken cancellationToken)
    {
        foreach (var spandata in data)
        {
            Console.WriteLine($"{spandata.Name}");
        }
        return Task.FromResult(ExportResult.Success);
    }

    public override Task ShutdownAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
