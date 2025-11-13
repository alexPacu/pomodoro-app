using SQLite;
using System.IO;
using Pomodoro.Models;

namespace Pomodoro.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection _database;

    public DatabaseService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "idokocka.db3");
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<TaskItem>().Wait();
        _database.CreateTableAsync<PomodoroSession>().Wait();
    }

    public SQLiteAsyncConnection GetConnection() => _database;
}