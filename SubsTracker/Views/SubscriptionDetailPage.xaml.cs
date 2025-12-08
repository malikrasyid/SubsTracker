using SubsTracker.ViewModels;

namespace SubsTracker.Views;

public partial class SubscriptionDetailPage : ContentPage
{
    public SubscriptionDetailPage(SubscriptionDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}