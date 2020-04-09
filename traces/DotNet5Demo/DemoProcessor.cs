using System;
using System.Diagnostics;

public class DemoProcessor
{
    public void OnActivityStarted(Activity activity)
    {
        Console.WriteLine(
            "Activity started OperationName={0}, DisplayName={1}",
            activity.OperationName,
            activity.DisplayName
        );
    }

    public void OnActivityStopped(Activity activity)
    {
        Console.WriteLine(
            "Activity stopped OperationName={0}, DisplayName={1}",
            activity.OperationName,
            activity.DisplayName
        );
    }
}
