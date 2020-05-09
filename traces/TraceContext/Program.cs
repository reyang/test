using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        // how to continue a trace from a W3C traceparent header
        var foo = new Activity("foo");
        foo.SetParentId("00-0123456789abcdef0123456789abcdef-0123456789abcdef-01");
        foo.TraceStateString = "a=1,b=2";
        foo.Start();
        Console.WriteLine($"TraceId:      {foo.TraceId}");
        Console.WriteLine($"ParentSpanId: {foo.ParentSpanId}");
        Console.WriteLine($"SpanId:       {foo.SpanId}");
        Console.WriteLine($"TraceFlags:   {foo.ActivityTraceFlags}");
        Console.WriteLine($"TraceState:   {foo.TraceStateString}");
        foo.Stop();

        // how to start a new trace
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        var bar = new Activity("bar");
        bar.TraceStateString = "a=1,b=2";
        bar.Start();
        Console.WriteLine($"TraceId:      {bar.TraceId}");
        Console.WriteLine($"ParentSpanId: {bar.ParentSpanId}");
        Console.WriteLine($"SpanId:       {bar.SpanId}");
        Console.WriteLine($"TraceFlags:   {bar.ActivityTraceFlags}");
        Console.WriteLine($"TraceState:   {bar.TraceStateString}");
        bar.Stop();
    }
}
