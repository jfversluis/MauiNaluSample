using CommunityToolkit.Mvvm.Input;

namespace Nalu.Maui.TaskFlow.PopupModels;

public class DiscardIntent : AwaitableIntent<bool>;

public partial class DiscardChangesPopupModel(INavigationService navigationService) : PopupModelBase<DiscardIntent, bool>(navigationService)
{
    public string Title { get; set; } = "Unsaved Changes";
    public string Message { get; set; } = "You have unsaved changes. Do you want to discard them?";

    [RelayCommand]
    public Task DiscardChangesAsync()
    {
        return CloseAsync(true);
    }
    
    [RelayCommand]
    public Task KeepEditingAsync()
    {
        return CloseAsync(false);
    }
}
