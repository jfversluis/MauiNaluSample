using CommunityToolkit.Maui;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nalu.Maui.TaskFlow.Models;
using Nalu.Maui.TaskFlow.PopupModels;
using Nalu.Maui.TaskFlow.Services;

namespace Nalu.Maui.TaskFlow.PageModels;

public partial class TaskEditorPageModel : ObservableObject, IEnteringAware, IEnteringAware<int>, ILeavingGuard
{
    private readonly INavigationService _navigationService;
    private readonly IPopupService _popupService;
    private readonly TaskService _taskService;
    private int? _editingTaskId;
    private bool _isDirty;
    private bool _isSaving;

    [ObservableProperty]
    private string _pageTitle = "New Task";

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private int _selectedPriorityIndex = 1; // 0=High, 1=Medium, 2=Low

    [ObservableProperty]
    private DateTime _dueDate = DateTime.Today.AddDays(7);

    [ObservableProperty]
    private string _tagsText = string.Empty;

    private static readonly TaskPriority[] PriorityValues = [TaskPriority.High, TaskPriority.Medium, TaskPriority.Low];

    private TaskPriority SelectedPriority
    {
        get => SelectedPriorityIndex >= 0 && SelectedPriorityIndex < PriorityValues.Length
            ? PriorityValues[SelectedPriorityIndex]
            : TaskPriority.Medium;
        set => SelectedPriorityIndex = Array.IndexOf(PriorityValues, value) is var i and >= 0 ? i : 1;
    }

    // Segmented priority button colors
    public Color HighBg => SelectedPriorityIndex == 0 ? Color.FromArgb("#E53935") : Colors.Transparent;
    public Color MediumBg => SelectedPriorityIndex == 1 ? Color.FromArgb("#FB8C00") : Colors.Transparent;
    public Color LowBg => SelectedPriorityIndex == 2 ? Color.FromArgb("#43A047") : Colors.Transparent;
    public Color HighFg => SelectedPriorityIndex == 0 ? Colors.White : Color.FromArgb("#E53935");
    public Color MediumFg => SelectedPriorityIndex == 1 ? Colors.White : Color.FromArgb("#FB8C00");
    public Color LowFg => SelectedPriorityIndex == 2 ? Colors.White : Color.FromArgb("#43A047");

    public TaskEditorPageModel(INavigationService navigationService, IPopupService popupService, TaskService taskService)
    {
        _navigationService = navigationService;
        _popupService = popupService;
        _taskService = taskService;
    }

    public ValueTask OnEnteringAsync()
    {
        _editingTaskId = null;
        PageTitle = "New Task";
        Title = string.Empty;
        Description = string.Empty;
        SelectedPriority = TaskPriority.Medium;
        DueDate = DateTime.Today.AddDays(7);
        TagsText = string.Empty;
        _isDirty = false; // AFTER all property assignments to avoid false dirty state
        NotifyPriorityColors();
        return ValueTask.CompletedTask;
    }

    public ValueTask OnEnteringAsync(int taskId)
    {
        _editingTaskId = taskId;
        PageTitle = "Edit Task";

        var task = _taskService.GetTask(taskId);
        if (task is not null)
        {
            Title = task.Title;
            Description = task.Description;
            SelectedPriority = task.Priority;
            DueDate = task.DueDate ?? DateTime.Today.AddDays(7);
            TagsText = string.Join(", ", task.Tags);
        }

        _isDirty = false; // AFTER all property assignments to avoid false dirty state
        NotifyPriorityColors();
        return ValueTask.CompletedTask;
    }

    private void NotifyPriorityColors()
    {
        OnPropertyChanged(nameof(HighBg));
        OnPropertyChanged(nameof(MediumBg));
        OnPropertyChanged(nameof(LowBg));
        OnPropertyChanged(nameof(HighFg));
        OnPropertyChanged(nameof(MediumFg));
        OnPropertyChanged(nameof(LowFg));
    }

    partial void OnTitleChanged(string value) => _isDirty = true;
    partial void OnDescriptionChanged(string value) => _isDirty = true;
    partial void OnSelectedPriorityIndexChanged(int value)
    {
        _isDirty = true;
        OnPropertyChanged(nameof(HighBg));
        OnPropertyChanged(nameof(MediumBg));
        OnPropertyChanged(nameof(LowBg));
        OnPropertyChanged(nameof(HighFg));
        OnPropertyChanged(nameof(MediumFg));
        OnPropertyChanged(nameof(LowFg));
    }

    [RelayCommand]
    private void SelectPriority(string indexStr) => SelectedPriorityIndex = int.Parse(indexStr);
    partial void OnDueDateChanged(DateTime value) => _isDirty = true;
    partial void OnTagsTextChanged(string value) => _isDirty = true;

    public async ValueTask<bool> CanLeaveAsync()
    {
        if (!_isDirty || _isSaving) return true;

        var result = await _popupService.ShowPopupAsync<DiscardChangesPopupModel, bool>(Shell.Current);
        return result.Result;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Title)) return;

        _isSaving = true;
        var tags = TagsText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        if (_editingTaskId.HasValue)
        {
            var task = _taskService.GetTask(_editingTaskId.Value);
            if (task is not null)
            {
                task.Title = Title;
                task.Description = Description;
                task.Priority = SelectedPriority;
                task.DueDate = DueDate;
                task.Tags = tags;
                _taskService.UpdateTask(task);
            }
        }
        else
        {
            _taskService.CreateTask(Title, Description, SelectedPriority, DueDate, tags);
        }

        _isDirty = false;
        await _navigationService.GoToAsync(Navigation.Relative().Pop());
    }
}
