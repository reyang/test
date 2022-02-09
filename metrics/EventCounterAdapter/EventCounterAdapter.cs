using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

public class EventCounterAdapter : EventListener
{
    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (!eventSource.Name.Equals("System.Runtime"))
        {
            return;
        }

        this.EnableEvents(eventSource, EventLevel.Verbose, EventKeywords.All, new Dictionary<string, string>()
        {
            ["EventCounterIntervalSec"] = "1",
        });
    }

    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        if (!eventData.EventName.Equals("EventCounters"))
        {
            return;
        }

        for (int i = 0; i < eventData.Payload.Count; ++i)
        {
            if (eventData.Payload[i] is IDictionary<string, object> eventPayload)
            {
                var (counterName, counterValue) = GetRelevantMetric(eventPayload);
                Console.WriteLine($"{counterName} : {counterValue}");
            }
        }
    }

    private static (string CounterName, string CounterValue) GetRelevantMetric(
        IDictionary<string, object> eventPayload)
    {
        var counterName = string.Empty;
        var counterValue = string.Empty;

        if (eventPayload.TryGetValue("DisplayName", out object displayValue))
        {
            counterName = displayValue.ToString();
        }

        if (eventPayload.TryGetValue("Mean", out object value) ||
            eventPayload.TryGetValue("Increment", out value))
        {
            counterValue = value.ToString();
        }

        return (counterName, counterValue);
    }
}
