namespace Nalu.Maui.TaskFlow;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    ConfigurationChanges = Android.Content.PM.ConfigChanges.ScreenSize |
                           Android.Content.PM.ConfigChanges.Orientation |
                           Android.Content.PM.ConfigChanges.UiMode |
                           Android.Content.PM.ConfigChanges.ScreenLayout |
                           Android.Content.PM.ConfigChanges.SmallestScreenSize |
                           Android.Content.PM.ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
}
