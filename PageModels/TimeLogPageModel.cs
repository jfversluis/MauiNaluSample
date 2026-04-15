using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nalu.Maui.TaskFlow.Models;
using Nalu.Maui.TaskFlow.Services;

namespace Nalu.Maui.TaskFlow.PageModels;

public partial class TimeLogPageModel : ObservableObject, IAppearingAware
{
    private readonly TaskService _taskService;

    [ObservableProperty]
    private ObservableCollection<TaskItem> _availableTasks = [];

    [ObservableProperty]
    private TaskItem? _selectedTask;

    [ObservableProperty]
    private TimeSpan? _selectedDuration = TimeSpan.FromMinutes(30);

    [ObservableProperty]
    private ObservableCollection<TimeEntry> _recentEntries = [];

    [ObservableProperty]
    private bool _showSummary;

    public TimeLogPageModel(TaskService taskService)
    {
        _taskService = taskService;
        LoadData();
    }

    public ValueTask OnAppearingAsync()
    {
        LoadData();
        return ValueTask.CompletedTask;
    }

    private void LoadData()
    {
        var tasks = _taskService.GetAllTasks().Where(t => !t.IsCompleted).Take(50).ToList();
        AvailableTasks = new ObservableCollection<TaskItem>(tasks);
        SelectedTask ??= AvailableTasks.FirstOrDefault();
        LoadRecentEntries();
    }

    private void LoadRecentEntries()
    {
        var entries = _taskService.GetRecentTimeEntries(15);
        RecentEntries = new ObservableCollection<TimeEntry>(entries);
    }

    [RelayCommand]
    private void LogTime()
    {
        if (SelectedTask is null || SelectedDuration is null || SelectedDuration.Value <= TimeSpan.Zero) return;

        _taskService.AddTimeEntry(SelectedTask.Id, SelectedDuration.Value);
        LoadRecentEntries();
        SelectedDuration = TimeSpan.FromMinutes(30);
    }

    [RelayCommand]
    private void SetView(string view)
    {
        ShowSummary = view == "summary";
    }
}
