using Nalu.Maui.TaskFlow.Pages;

namespace Nalu.Maui.TaskFlow;

public partial class AppShell : NaluShell
{
    public AppShell(INavigationService navigationService)
        : base(navigationService, typeof(DashboardPage))
    {
        InitializeComponent();
    }
}
