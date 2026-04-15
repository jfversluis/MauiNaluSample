using System.Globalization;
using Nalu.Maui.TaskFlow.Models;
using Nalu.Maui.TaskFlow.PageModels;

namespace Nalu.Maui.TaskFlow.Pages;

public partial class TaskListPage
{
    public TaskListPage(TaskListPageModel model)
    {
        BindingContext = model;
        InitializeComponent();
    }
}

public class PriorityColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is TaskPriority priority)
        {
            return priority switch
            {
                TaskPriority.High => Color.FromArgb("#E17055"),
                TaskPriority.Medium => Color.FromArgb("#FDCB6E"),
                TaskPriority.Low => Color.FromArgb("#00B894"),
                _ => Colors.Gray
            };
        }
        return Colors.Gray;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToCheckIconConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? "\ue876" : "\ue836";

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class BoolToCheckColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Color.FromArgb("#00B894") : Color.FromArgb("#B2BEC3");

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
