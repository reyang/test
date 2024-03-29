using System;
using OpenTelemetry;
using OpenTelemetry.Logs;

internal class DummyLogExporter : BaseExporter<LogRecord>
{
    public override ExportResult Export(in Batch<LogRecord> batch)
    {
        foreach (var record in batch)
        {
        }

        return ExportResult.Success;
    }
}

internal static class LoggerExtensions
{
    public static OpenTelemetryLoggerOptions AddDummyLogExporter(this OpenTelemetryLoggerOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return options.AddProcessor(new ConcurrentExportProcessor<LogRecord>(new DummyLogExporter()));
    }
}
