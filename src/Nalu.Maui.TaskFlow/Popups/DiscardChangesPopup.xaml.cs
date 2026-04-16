using Nalu.Maui.TaskFlow.PopupModels;

namespace Nalu.Maui.TaskFlow.Popups;

public partial class DiscardChangesPopup
{
    public DiscardChangesPopup(DiscardChangesPopupModel model)
    {
        BindingContext = model;
        InitializeComponent();
    }
}
