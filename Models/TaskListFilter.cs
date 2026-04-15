namespace Nalu.Maui.TaskFlow.Models;

public class TaskListFilter
{
    public string? Category { get; init; }
    public TaskPriority? Priority { get; init; }
    public bool? OverdueOnly { get; init; }
}
