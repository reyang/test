// dotnet restore DotNet5Demo.csproj -s .
// dotnet run DotNet5Demo.csproj

using System.Diagnostics;

class Program
{
    static int Main(string[] args)
    {
        Activity.DefaultIdFormat = ActivityIdFormat.W3C;

        var source = new ActivitySource("DemoSource");
        source.RegisterProcessor(new DemoProcessor());
        Activity activity;

        using (var foo = source.StartActivity("foo"))
        {
            using (var bar = source.StartActivity("bar", ActivityKind.Client))
            {
                // bar?.DisplayName = "baz";
                bar?.AddTag("tag1", "value1");
                bar?.SetCustomProperty("alias", "reyang");
                System.Console.WriteLine($"{bar.TraceStateString}");
                activity = bar;
                System.Console.WriteLine($"{activity.TraceId}");
            }
        }

        System.Console.WriteLine($"{activity.Context}");

        return 0;
    }
}
