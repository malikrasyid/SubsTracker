using SubsTracker.ViewModels;

namespace SubsTracker.Views;

public partial class AddSubscriptionPage : ContentPage
{
    public AddSubscriptionPage(AddSubscriptionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}