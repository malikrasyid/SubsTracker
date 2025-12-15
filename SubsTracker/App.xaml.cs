using SubsTracker.Services;
using Plugin.LocalNotification;

namespace SubsTracker
{
    public partial class App : Application
    {
        private readonly NotificationService _notificationService;

        public App(NotificationService notificationService)
        {
            InitializeComponent();
            _notificationService = notificationService;

            LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationTapped;

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();

            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }

            try
            {
                await _notificationService.CheckAndGenerateNotifications();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error generating notifications on start: {ex.Message}");
            }
        }

        private async void OnNotificationTapped(Plugin.LocalNotification.EventArgs.NotificationActionEventArgs e)
        {
            if (e.IsTapped)
            {
                await Shell.Current.GoToAsync($"//{nameof(Views.DashboardPage)}/{nameof(Views.NotificationPage)}");
            }
        }
    }
}