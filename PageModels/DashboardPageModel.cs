using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nalu.Maui.TaskFlow.Models;
using Nalu.Maui.TaskFlow.Services;

namespace Nalu.Maui.TaskFlow.PageModels;

public partial class DashboardPageModel(INavigationService navigationService, TaskService taskService)
    : ObservableObject, IAppearingAware
{
    [ObservableProperty]
    private int _totalTasks;

    [ObservableProperty]
    private int _completedTasks;

    [ObservableProperty]
    private int _overdueTasks;

    [ObservableProperty]
    private int _highPriorityTasks;

    public ValueTask OnAppearingAsync()
    {
        TotalTasks = taskService.TotalTasks;
        CompletedTasks = taskService.CompletedTasks;
        OverdueTasks = taskService.OverdueTasks;
        HighPriorityTasks = taskService.HighPriorityTasks;
        return ValueTask.CompletedTask;
    }

    [RelayCommand(AllowConcurrentExecutions = false)]
    private Task GoToAllTasksAsync()
        => navigationService.GoToAsync(
            Navigation.Relative().Push<TaskListPageModel>());

    [RelayCommand(AllowConcurrentExecutions = false)]
    private Task GoToOverdueAsync()
        => navigationService.GoToAsync(
            Navigation.Relative().Push<TaskListPageModel>()
                .WithIntent(new TaskListFilter { OverdueOnly = true }));

    [RelayCommand(AllowConcurrentExecutions = false)]
    private Task GoToHighPriorityAsync()
        => navigationService.GoToAsync(
            Navigation.Relative().Push<TaskListPageModel>()
                .WithIntent(new TaskListFilter { Priority = TaskPriority.High }));
}
