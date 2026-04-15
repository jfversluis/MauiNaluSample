namespace Nalu.Maui.TaskFlow.Models;

public class TimeEntry
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public string TaskTitle { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.Now;
}
