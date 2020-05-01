﻿using System;

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

class FoobarChannel : ITelemetryChannel
{
    public FoobarChannel()
    {
    }

    public void Initialize(TelemetryConfiguration configuration)
    {
    }

    public bool? DeveloperMode { get; set; }

    public string EndpointAddress { get; set; }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public void Send(ITelemetry item)
    {
        Console.WriteLine($"Foobar.Send({item}) called");
    }

    public void Flush()
    {
        Console.WriteLine($"Foobar.Flush() called");
    }
}

class Program
{
    static void Main(string[] args)
    {
        var config = TelemetryConfiguration.CreateDefault();
        var sink = new TelemetrySink(config, new FoobarChannel()) { Name = "Foobar" };
        sink.Initialize(config);
        config.TelemetrySinks.Add(sink);
        // config.TelemetryChannel = new FoobarChannel();
        config.TelemetryProcessorChainBuilder.Build();
        // TelemetryClient is thread safe
        // you want to create only one instance and use it across the application
        // otherwise you will pay additional CPU/memory cost
        var client = new TelemetryClient(config);

        var evt = new EventTelemetry("Hello, world!");
        evt.Properties.Add("CustomName", "CustomValue");
        client.TrackEvent(evt);

        client.Flush();
        foreach (var s in config.TelemetrySinks)
        {
            Console.WriteLine($"Trying to flush TelemetrySink(Name={s.Name})");
            s.TelemetryChannel.Flush();
        }
    }
}
