using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nalu.Maui.TaskFlow.Services;

namespace Nalu.Maui.TaskFlow.PageModels;

public partial class TagChip : ObservableObject
{
    public string Name { get; }

    [ObservableProperty]
    private bool _isSelected;

    public TagChip(string name) => Name = name;
}

public partial class SettingsPageModel : ObservableObject, IAppearingAware
{
    private readonly TaskService _taskService;

    public ObservableCollection<TagChip> TagChips { get; } = [];

    [ObservableProperty]
    private bool _isDarkMode;

    partial void OnIsDarkModeChanged(bool value)
    {
        if (Application.Current is not null)
        {
            Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
        }
    }

    [ObservableProperty]
    private bool _showAbout;

    [ObservableProperty]
    private string _appVersion = "TaskFlow v1.0 — Powered by Nalu.Maui";

    public SettingsPageModel(TaskService taskService)
    {
        _taskService = taskService;
        LoadTags();
    }

    public ValueTask OnAppearingAsync()
    {
        LoadTags();
        return ValueTask.CompletedTask;
    }

    private void LoadTags()
    {
        if (TagChips.Count > 0) return;
        foreach (var tag in _taskService.GetAllTags())
            TagChips.Add(new TagChip(tag));
    }

    [RelayCommand]
    private void ToggleTag(TagChip chip)
    {
        chip.IsSelected = !chip.IsSelected;
    }

    [RelayCommand]
    private void ToggleAbout()
    {
        ShowAbout = !ShowAbout;
    }
}
