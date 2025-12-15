using SubsTracker.ViewModels;

namespace SubsTracker.Views;

public partial class NotificationPage : ContentPage
{
    public NotificationPage(NotificationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // This triggers the data loading every time the page opens
        if (BindingContext is NotificationsViewModel viewModel)
        {
            await viewModel.LoadNotificationsCommand.ExecuteAsync(null);
        }
    }
}