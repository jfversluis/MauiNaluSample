using Nalu.Maui.TaskFlow.PageModels;

namespace Nalu.Maui.TaskFlow.Pages;

public partial class TaskEditorPage
{
    public TaskEditorPage(TaskEditorPageModel model)
    {
        BindingContext = model;
        InitializeComponent();
    }
}
