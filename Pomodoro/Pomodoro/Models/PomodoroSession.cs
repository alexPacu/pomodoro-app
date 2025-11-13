using SQLite;

namespace Pomodoro.Models;

public class PomodoroSession
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public string Type { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
}