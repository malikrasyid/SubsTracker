using SubsTracker.Views;

namespace SubsTracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for pages that are NOT in the bottom tabs
        Routing.RegisterRoute(nameof(AddSubscriptionPage), typeof(AddSubscriptionPage));
        Routing.RegisterRoute(nameof(SubscriptionDetailPage), typeof(SubscriptionDetailPage));
    }
}