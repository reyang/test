using System.Diagnostics;

public interface IActivityProcessor
{
    void OnActivityStarted(Activity activity);
    void OnActivityStopped(Activity activity);
}
