using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nalu.Maui.TaskFlow.Models;
using Nalu.Maui.TaskFlow.Services;

namespace Nalu.Maui.TaskFlow.PageModels;

public class ReplaceableObservableCollection<T> : ObservableCollection<T>
{
    public ReplaceableObservableCollection(IEnumerable<T> items) : base(items) { }

    public void ReplaceAll(IEnumerable<T> items)
    {
        // Use the protected Items property (IList<T>) to bypass individual notifications
        var innerList = Items;
        innerList.Clear();
        foreach (var item in items)
        {
            innerList.Add(item);
        }

        // It's fundamental to update count and indexer first, then send the notification changed event args
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}

public partial class TaskListPageModel : ObservableObject, IEnteringAware, IEnteringAware<TaskListFilter>
{
    private readonly INavigationService _navigationService;
    private readonly TaskService _taskService;
    private TaskListFilter? _activeFilter;

    [ObservableProperty]
    private string _pageTitle = "All Tasks";

    public ReplaceableObservableCollection<TaskItem> Tasks { get; }

    public IVirtualScrollAdapter Adapter { get; }

    [ObservableProperty]
    private bool _isRefreshing;

    public TaskListPageModel(INavigationService navigationService, TaskService taskService)
    {
        _navigationService = navigationService;
        _taskService = taskService;
        Tasks = new ReplaceableObservableCollection<TaskItem>([]);
        Adapter = VirtualScroll.CreateObservableCollectionAdapter(Tasks);
    }

    public ValueTask OnEnteringAsync()
    {
        _activeFilter = null;
        PageTitle = "All Tasks";
        LoadTasks();
        return ValueTask.CompletedTask;
    }

    public ValueTask OnEnteringAsync(TaskListFilter intent)
    {
        _activeFilter = intent;
        PageTitle = intent switch
        {
            { OverdueOnly: true } => "⚠️ Overdue Tasks",
            { Priority: TaskPriority.High } => "🔴 High Priority",
            { Priority: TaskPriority.Medium } => "🟡 Medium Priority",
            { Priority: TaskPriority.Low } => "🟢 Low Priority",
            _ => "All Tasks"
        };
        LoadTasks();
        return ValueTask.CompletedTask;
    }

    private void LoadTasks()
    {
        Tasks.ReplaceAll(_taskService.GetFilteredTasks(_activeFilter));
    }

    [RelayCommand]
    private void Refresh(Action completionCallback)
    {
        // Simulate pulling new data
        _taskService.CreateTask(
            $"New task #{_taskService.TotalTasks + 1}",
            "Pulled from refresh",
            (TaskPriority)Random.Shared.Next(3),
            DateTime.Today.AddDays(Random.Shared.Next(1, 14)),
            ["New"]);

        LoadTasks();
        completionCallback();
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private Task EditTaskAsync(TaskItem task)
        => _navigationService.GoToAsync(
            Navigation.Relative().Push<TaskEditorPageModel>()
                .WithIntent(task.Id));

    [RelayCommand(AllowConcurrentExecutions = false)]
    private Task AddTaskAsync()
        => _navigationService.GoToAsync(
            Navigation.Relative().Push<TaskEditorPageModel>());

    [RelayCommand]
    private void ToggleComplete(TaskItem task)
    {
        task.IsCompleted = !task.IsCompleted;
        _taskService.UpdateTask(task);

        if (_activeFilter?.OverdueOnly is true)
        {
            LoadTasks();
        }
    }

}
