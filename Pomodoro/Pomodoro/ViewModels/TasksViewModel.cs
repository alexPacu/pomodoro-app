using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pomodoro.Models;
using Pomodoro.Repositories;
using System.Collections.ObjectModel;

namespace Pomodoro.ViewModels;

public partial class TasksViewModel : ObservableObject
{
    private readonly TaskRepository _repo;

    public TasksViewModel(TaskRepository repo)
    {
        _repo = repo;
        Tasks = new();
        LoadTasksCommand.Execute(null);
    }

    [ObservableProperty] private ObservableCollection<TaskItem> tasks;
    [ObservableProperty] private string newTaskTitle = "";

    [RelayCommand]
    private async Task LoadTasks()
    {
        var list = await _repo.GetAllAsync();
        Tasks = new ObservableCollection<TaskItem>(list);
    }

    [RelayCommand]
    private void AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle)) return;
        var task = new TaskItem { Title = NewTaskTitle, Priority = 1 };
        _repo.SaveAsync(task).ContinueWith(_ => LoadTasksCommand.Execute(null));
        NewTaskTitle = "";
    }

    [RelayCommand]
    private async Task ToggleComplete(TaskItem task)
    {
        if (task == null) return;
        task.IsCompleted = !task.IsCompleted;
        await _repo.SaveAsync(task);
    }

    [RelayCommand]
    private async Task DeleteTask(TaskItem task)
    {
        if (task == null) return;
        await _repo.DeleteAsync(task);
        Tasks.Remove(task);
    }
}