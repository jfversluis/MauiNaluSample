using Nalu.Maui.TaskFlow.PageModels;

namespace Nalu.Maui.TaskFlow.Pages;

public partial class DashboardPage
{
    public DashboardPage(DashboardPageModel model)
    {
        BindingContext = model;
        InitializeComponent();
    }
}
