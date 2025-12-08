using Android.App;
using Android.Content.PM;
using Android.OS;

namespace SubsTracker
{
    [Activity(
        Theme = "@style/Maui.SplashTheme", 
        MainLauncher = true, 
        LaunchMode = LaunchMode.SingleTop,
        ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    
    public class MainActivity : MauiAppCompatActivity
    {
    }
}
