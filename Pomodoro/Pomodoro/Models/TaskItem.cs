using SQLite;

namespace Pomodoro.Models;

public class TaskItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public int Priority { get; set; } = 0;
    public bool IsCompleted { get; set; } = false;
}