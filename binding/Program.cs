using System;
using System.Diagnostics;
using System.Reflection;

class Program
{
    static int Main(string[] args)
    {
        var assembly = Assembly.Load("System.Diagnostics.DiagnosticSource");
        var location = assembly.Location;
        var fvi = FileVersionInfo.GetVersionInfo(location);
#if NETFRAMEWORK
        Console.WriteLine($"Running on: {AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName}");
#else
        Console.WriteLine($"Running on: {Assembly.GetEntryAssembly()?.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>()?.FrameworkName}");
#endif
        Console.WriteLine($"Assembly FullName: {assembly.FullName}");
        Console.WriteLine($"Assembly Location: {assembly.Location}");
        Console.WriteLine($"PE File Version: {fvi.FileVersion}");
        return 0;
    }
}
