// dotnet restore DotNet5Demo.csproj -s .
// dotnet run DotNet5Demo.csproj

using System.Diagnostics;

class Program
{
    static int Main(string[] args)
    {
        var source = new ActivitySource("DemoSource");
        source.RegisterProcessor(new DemoProcessor());

        using (var foo = source.StartActivity("foo"))
        {
            using (var bar = source.StartActivity("bar", ActivityKind.Internal))
            {
                bar.DisplayName = "baz";
                // bar?.SetDisplayName("baz");
                bar?.SetCustomProperty("alias", "reyang");
            }
        }
        return 0;
    }
}
