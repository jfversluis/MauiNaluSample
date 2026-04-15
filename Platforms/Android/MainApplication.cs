namespace Nalu.Maui.TaskFlow;

[Application]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, Android.Runtime.JniHandleOwnership ownership)
        : base(handle, ownership)
    {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
