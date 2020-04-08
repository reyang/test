// dotnet restore DotNet5Demo.csproj -s .
// dotnet run DotNet5Demo.csproj

using System.Diagnostics;

class Program
{
    static int Main(string[] args)
    {
        var source = new ActivitySource("DemoSource");
        var processor = new DemoProcessor();
        ActivitySource.AddActivityListener(
            activitySource => true, // subscribe all sources
            (activitySource, name, kind, context, tags, links) => ActivityDataRequest.AllData,
            (activitySource, name, kind, parentId, tags, links) => ActivityDataRequest.AllData,
            activity => processor.OnActivityStarted(activity),
            activity => processor.OnActivityStopped(activity)
        );

        using (var foo = source.StartActivity("foo"))
        {
            using (var bar = source.StartActivity("bar", ActivityKind.Internal))
            {
            }
        }
        return 0;
    }
}
