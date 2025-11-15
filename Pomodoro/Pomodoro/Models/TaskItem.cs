using SQLite;

namespace Pomodoro.Models;

[Table("Tasks")]
public class TaskItem
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Title { get; set; }
}