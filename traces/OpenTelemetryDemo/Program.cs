using OpenTelemetry.Trace;
using OpenTelemetry.Trace.Configuration;
using OpenTelemetry.Trace.Export;

class Program
{
    static void Main(string[] args)
    {
        var spanExporter = new DemoExporter();
        var tracer = TracerFactory.Create(b => b
                .AddProcessorPipeline(p => p
                    .SetExporter(spanExporter)
                    .SetExportingProcessor(e => new SimpleSpanProcessor(e))))
            .GetTracer(null);

        var span = tracer.StartSpan("Hello", null, SpanKind.Internal);
        span.SetAttribute("custom-attribute", 55);
        span.End();
    }
}
