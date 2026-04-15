using Nalu.Maui.TaskFlow.PopupModels;

namespace Nalu.Maui.TaskFlow.Popups;

public partial class DiscardChangesPopup
{
    public DiscardChangesPopup(DiscardChangesPopupModel model)
    {
        BindingContext = model;
        InitializeComponent();
    }

    private async void KeepEditingClicked(object? sender, EventArgs e) => await CloseAsync(false);
    private async void DiscardClicked(object? sender, EventArgs e) => await CloseAsync(true);
}
