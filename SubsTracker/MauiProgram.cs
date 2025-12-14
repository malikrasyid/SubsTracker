using Microsoft.Extensions.Logging;
using SubsTracker.Services;
using Plugin.LocalNotification;
using SubsTracker.ViewModels;
using Microcharts.Maui;
using Microsoft.Maui.Platform;

namespace SubsTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMicrocharts()
                //.UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<NotificationService>();
            builder.Services.AddSingleton<AdviceService>();

            builder.Services.AddSingleton<DashboardViewModel>();
            builder.Services.AddTransient<AddSubscriptionViewModel>();
            builder.Services.AddTransient<SubscriptionDetailViewModel>();
            builder.Services.AddTransient<InsightsViewModel>();
            builder.Services.AddSingleton<NotificationsViewModel>();

            builder.Services.AddSingleton<Views.DashboardPage>();
            builder.Services.AddTransient<Views.AddSubscriptionPage>();
            builder.Services.AddTransient<Views.SubscriptionDetailPage>();
            builder.Services.AddTransient<Views.NotificationPage>();
            builder.Services.AddTransient<Views.InsightsPage>();
            builder.Services.AddTransient<Views.SettingsPage>();

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
            #if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);            
            #elif IOS
                handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
                handler.PlatformView.Layer.BorderWidth = 0;
                handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
            #endif
            });

            Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });

            Microsoft.Maui.Handlers.DatePickerHandler.Mapper.AppendToMapping("Borderless", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
                handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
#endif
            });
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
