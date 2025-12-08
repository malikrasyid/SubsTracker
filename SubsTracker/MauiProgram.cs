using Microsoft.Extensions.Logging;
using SubsTracker.Services;
using Plugin.LocalNotification;
using SubsTracker.ViewModels;

namespace SubsTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                //.UseLocalNotification()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<Services.DatabaseService>();
            builder.Services.AddSingleton<Services.NotificationService>();

            builder.Services.AddSingleton<DashboardViewModel>();
            builder.Services.AddTransient<AddSubscriptionViewModel>();
            builder.Services.AddTransient<SubscriptionDetailViewModel>();
            builder.Services.AddTransient<InsightsViewModel>();

            builder.Services.AddSingleton<Views.DashboardPage>();
            builder.Services.AddTransient<Views.AddSubscriptionPage>();
            builder.Services.AddTransient<Views.SubscriptionDetailPage>();
            builder.Services.AddTransient<Views.InsightsPage>();
            builder.Services.AddTransient<Views.SettingsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
