using SQLite;
using Pomodoro.Models;

namespace Pomodoro.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService()
    {
        var path = Path.Combine(FileSystem.AppDataDirectory, "pomodoro.db3");
        _db = new SQLiteAsyncConnection(path);

        _db.CreateTableAsync<TaskItem>().Wait();
    }

    public Task<List<TaskItem>> GetTasksAsync()
        => _db.Table<TaskItem>().ToListAsync();

    public Task<int> SaveTaskAsync(TaskItem task)
    {
        if (task.Id != 0)
            return _db.UpdateAsync(task);

        return _db.InsertAsync(task);
    }

    public Task<int> DeleteTaskAsync(TaskItem task)
        => _db.DeleteAsync(task);
}
