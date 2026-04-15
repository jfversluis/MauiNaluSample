using CommunityToolkit.Mvvm.ComponentModel;

namespace Nalu.Maui.TaskFlow.PopupModels;

public class DiscardChangesPopupModel : ObservableObject
{
    public string Title { get; set; } = "Unsaved Changes";
    public string Message { get; set; } = "You have unsaved changes. Do you want to discard them?";
}
