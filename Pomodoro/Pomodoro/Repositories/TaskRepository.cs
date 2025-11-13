using Pomodoro.Models;
using Pomodoro.Services;
using SQLite;

namespace Pomodoro.Repositories;

public class TaskRepository
{
    private readonly SQLiteAsyncConnection _db;

    public TaskRepository(DatabaseService dbService)
    {
        _db = dbService.GetConnection();
    }

    public Task<List<TaskItem>> GetAllAsync() => _db.Table<TaskItem>().ToListAsync();
    public Task<int> SaveAsync(TaskItem task) => task.Id == 0
        ? _db.InsertAsync(task)
        : _db.UpdateAsync(task);
    public Task<int> DeleteAsync(TaskItem task) => _db.DeleteAsync(task);
}