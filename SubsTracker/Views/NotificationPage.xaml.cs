using SubsTracker.ViewModels;

namespace SubsTracker.Views;

public partial class NotificationPage : ContentPage
{
    private readonly NotificationsViewModel _viewModel;
    public NotificationPage(NotificationsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }
}