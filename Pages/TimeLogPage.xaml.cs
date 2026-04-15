using System.Globalization;
using Nalu.Maui.TaskFlow.PageModels;

namespace Nalu.Maui.TaskFlow.Pages;

public partial class TimeLogPage
{
    public TimeLogPage(TimeLogPageModel model)
    {
        BindingContext = model;
        InitializeComponent();
    }
}

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Color.FromArgb("#6C5CE7") : Color.FromArgb("#B2BEC3");

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class InvertedBoolToColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Color.FromArgb("#B2BEC3") : Color.FromArgb("#6C5CE7");

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
