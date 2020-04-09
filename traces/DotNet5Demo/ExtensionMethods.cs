// dotnet restore DotNet5Demo.csproj -s .
// dotnet run DotNet5Demo.csproj

using System.Diagnostics;

public static class ExtensionMethods
{
    public static void RegisterProcessor(this ActivitySource source, IActivityProcessor processor)
    {
        ActivitySource.AddActivityListener(
            activitySource => source == activitySource,
            (activitySource, name, kind, context, tags, links) => ActivityDataRequest.AllData,
            (activitySource, name, kind, parentId, tags, links) => ActivityDataRequest.AllData,
            processor.OnActivityStarted,
            processor.OnActivityStopped
        );
    }
}
