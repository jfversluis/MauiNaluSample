using System.Collections.ObjectModel;
using Nalu.Maui.TaskFlow.Models;

namespace Nalu.Maui.TaskFlow.Services;

public class TaskService
{
    private readonly List<TaskItem> _tasks = [];
    private readonly List<TimeEntry> _timeEntries = [];
    private int _nextTaskId = 1;
    private int _nextTimeEntryId = 1;

    public TaskService()
    {
        SeedData();
    }

    private void SeedData()
    {
        var tags = new[] { "Design", "Backend", "Frontend", "Bug", "Feature", "Docs", "Testing", "DevOps", "UX", "API" };
        var titles = new[]
        {
            "Set up CI/CD pipeline", "Design login screen", "Implement user auth",
            "Write unit tests for API", "Fix navigation crash", "Add pull-to-refresh",
            "Optimize database queries", "Create onboarding flow", "Update dependencies",
            "Refactor settings page", "Add dark mode support", "Implement push notifications",
            "Review PR #42", "Fix memory leak in list", "Add analytics tracking",
            "Design dashboard layout", "Implement search feature", "Add offline support",
            "Write API documentation", "Fix keyboard overlap issue", "Add haptic feedback",
            "Implement drag and drop", "Create splash screen", "Add biometric auth",
            "Optimize image loading", "Fix date picker bug", "Add swipe gestures",
            "Implement caching layer", "Update color scheme", "Add accessibility labels"
        };

        var random = new Random(42);

        for (var i = 0; i < 200; i++)
        {
            var titleBase = titles[i % titles.Length];
            var suffix = i >= titles.Length ? $" (v{i / titles.Length + 1})" : "";
            var daysOffset = random.Next(-5, 30);
            var taskTags = Enumerable.Range(0, random.Next(1, 4))
                .Select(_ => tags[random.Next(tags.Length)])
                .Distinct()
                .ToList();

            _tasks.Add(new TaskItem
            {
                Id = _nextTaskId++,
                Title = $"{titleBase}{suffix}",
                Description = $"Detailed description for: {titleBase}{suffix}. This task involves multiple steps and requires careful attention.",
                Priority = (TaskPriority)random.Next(3),
                IsCompleted = random.NextDouble() < 0.25,
                DueDate = DateTime.Today.AddDays(daysOffset),
                CreatedAt = DateTime.Now.AddDays(-random.Next(1, 60)),
                Tags = taskTags,
                TotalTimeLogged = TimeSpan.FromMinutes(random.Next(0, 480))
            });
        }
    }

    public List<TaskItem> GetAllTasks() => [.. _tasks];

    public List<TaskItem> GetFilteredTasks(TaskListFilter? filter)
    {
        IEnumerable<TaskItem> query = _tasks;

        if (filter?.Priority is { } priority)
            query = query.Where(t => t.Priority == priority);

        if (filter?.OverdueOnly == true)
            query = query.Where(t => t.DueDate < DateTime.Today && !t.IsCompleted);

        return query.ToList();
    }

    public TaskItem? GetTask(int id) => _tasks.FirstOrDefault(t => t.Id == id);

    public void UpdateTask(TaskItem task)
    {
        var index = _tasks.FindIndex(t => t.Id == task.Id);
        if (index >= 0) _tasks[index] = task;
    }

    public TaskItem CreateTask(string title, string description, TaskPriority priority, DateTime? dueDate, List<string>? tags)
    {
        var task = new TaskItem
        {
            Id = _nextTaskId++,
            Title = title,
            Description = description,
            Priority = priority,
            DueDate = dueDate,
            Tags = tags ?? []
        };
        _tasks.Insert(0, task);
        return task;
    }

    public void ReorderTask(int taskId, int newIndex)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (task is null) return;
        _tasks.Remove(task);
        _tasks.Insert(Math.Min(newIndex, _tasks.Count), task);
    }

    public int TotalTasks => _tasks.Count;
    public int CompletedTasks => _tasks.Count(t => t.IsCompleted);
    public int OverdueTasks => _tasks.Count(t => t.DueDate < DateTime.Today && !t.IsCompleted);
    public int HighPriorityTasks => _tasks.Count(t => t.Priority == TaskPriority.High && !t.IsCompleted);

    public void AddTimeEntry(int taskId, TimeSpan duration)
    {
        var task = _tasks.FirstOrDefault(t => t.Id == taskId);
        if (task is null) return;

        _timeEntries.Add(new TimeEntry
        {
            Id = _nextTimeEntryId++,
            TaskId = taskId,
            TaskTitle = task.Title,
            Duration = duration,
            LoggedAt = DateTime.Now
        });
        task.TotalTimeLogged += duration;
    }

    public List<TimeEntry> GetRecentTimeEntries(int count = 20)
        => _timeEntries.OrderByDescending(e => e.LoggedAt).Take(count).ToList();

    public List<string> GetAllTags()
        => _tasks.SelectMany(t => t.Tags).Distinct().OrderBy(t => t).ToList();
}
