using SubsTracker.ViewModels;

namespace SubsTracker.Views;

public partial class InsightsPage : ContentPage
{
    private readonly InsightsViewModel _viewModel;

    public InsightsPage(InsightsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    // Force data refresh every time the user taps this tab
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadInsightsCommand.Execute(null);
    }
}