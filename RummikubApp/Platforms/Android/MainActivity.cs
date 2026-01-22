using Android.App;
using Android.Content.PM;
using Android.OS;
using CommunityToolkit.Mvvm.Messaging;
using RummikubApp.Models;
using Android.Content;

namespace RummikubApp.Platforms.Android
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        MyTimer? mTimer;
        override protected void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RegisterTimerMessages();
            StartDeleteFbDocsService();
        }

        private void StartDeleteFbDocsService()
        {
            Intent intent = new(this, typeof(DeleteFbDocsService));
            StartService(intent);
        }

        private void RegisterTimerMessages()
        {
            WeakReferenceMessenger.Default.Register<AppMessage<TimerSettings>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
            WeakReferenceMessenger.Default.Register<AppMessage<bool>>(this, (r, m) =>
            {
                OnMessageReceived(m.Value);
            });
        }

        private void OnMessageReceived(bool value)
        {
            if (value)
            {
                mTimer?.Cancel();
                mTimer = null;
            }
        }

        private void OnMessageReceived(TimerSettings value)
        {
            mTimer = new MyTimer(value.TotalTimeInMilliseconds, value.IntervalInMilliseconds);
            mTimer.Start();
        }
    }
}
