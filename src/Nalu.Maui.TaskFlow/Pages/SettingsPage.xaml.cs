using System.Globalization;
using Nalu.Maui.TaskFlow.PageModels;

namespace Nalu.Maui.TaskFlow.Pages;

public partial class SettingsPage
{
    public SettingsPage(SettingsPageModel model)
    {
        BindingContext = model;
        InitializeComponent();
    }
}

public class BoolToChipOpacityConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? 1.0 : 0.45;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
