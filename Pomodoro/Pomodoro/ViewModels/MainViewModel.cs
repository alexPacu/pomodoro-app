using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pomodoro.Models;
using Pomodoro.Services;
using System.Collections.ObjectModel;

namespace Pomodoro.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DatabaseService _db;
    private readonly System.Timers.Timer _timer;

    private const int PomodoroDuration = 8;//25 * 60;
    private const int RestDuration = 8;//5 * 60;
    private const int LongRestDuration = 8;//15 * 60;

    private int _seconds;
    private int _mode;
    private bool _isRunning;

    private bool _suppressDisplayTimeUpdate;

    public MainViewModel(DatabaseService db)
    {
        _db = db;

        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += OnTick;

        Reset();
        LoadTasksCommand.Execute(null);
    }


    [ObservableProperty] private string displayTime = "25:00";
    [ObservableProperty] private Color pomodoroColor = Colors.Blue;
    [ObservableProperty] private Color restColor = Colors.Gray;
    [ObservableProperty] private Color longRestColor = Colors.Gray;
    [ObservableProperty] private Color modeColor = Colors.Blue;
    [ObservableProperty] private string startButtonText = "START";
    [ObservableProperty] private ObservableCollection<TaskItem> tasks = new();
    [ObservableProperty] private string newTaskTitle = "";

    partial void OnDisplayTimeChanged(string value)
    {
        if (_suppressDisplayTimeUpdate)
            return;

        if (TryParseTime(value, out var seconds))
        {
            _seconds = seconds;
            UpdateDisplay();
        }
    }

    private bool TryParseTime(string? input, out int seconds)
    {
        seconds = 0;
        if (string.IsNullOrWhiteSpace(input))
            return false;

        input = input.Trim();

        if (input.Contains(':'))
        {
            var parts = input.Split(':');
            if (parts.Length != 2)
                return false;

            if (!int.TryParse(parts[0], out var m)) return false;
            if (!int.TryParse(parts[1], out var s)) return false;
            if (m < 0 || s < 0 || s >= 60) return false;

            seconds = m * 60 + s;
            return true;
        }

        if (int.TryParse(input, out var minutes) && minutes >= 0)
        {
            seconds = minutes * 60;
            return true;
        }

        return false;
    }


    private void Reset()
    {
        _timer.Stop();
        _isRunning = false;
        _mode = 0;
        _seconds = PomodoroDuration;

        UpdateDisplay();
        UpdateColors();

        StartButtonText = "START";
    }


    [RelayCommand]
    private void StartPause()
    {
        if (_isRunning)
        {
            _timer.Stop();
            StartButtonText = "START";
        }
        else
        {
            _timer.Start();
            StartButtonText = "PAUSE";
        }

        _isRunning = !_isRunning;
    }


    [RelayCommand]
    private void SelectPomodoro() => SetMode(0, PomodoroDuration, Colors.Blue);

    [RelayCommand]
    private void SelectRest() => SetMode(1, RestDuration, Colors.Green);

    [RelayCommand]
    private void SelectLongRest() => SetMode(2, LongRestDuration, Colors.DarkGreen);

    private void SetMode(int mode, int seconds, Color color)
    {
        if (_isRunning) return;

        _mode = mode;
        _seconds = seconds;
        ModeColor = color;

        UpdateDisplay();
        UpdateColors();
    }


    private void OnTick(object? s, System.Timers.ElapsedEventArgs e)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _seconds--;
            UpdateDisplay();

            if (_seconds > 0)
                return;

            _timer.Stop();
            _isRunning = false;
            StartButtonText = "START";

            if (_mode == 1 || _mode == 2)
            {
                _mode = 0;
                _seconds = PomodoroDuration;
                UpdateColors();
                UpdateDisplay();
                return;
            }

            if (_mode == 0)
            {
                _mode = 1;
                _seconds = RestDuration;

                UpdateColors();
                UpdateDisplay();
            }
        });
    }


    private void UpdateDisplay()
    {
        var m = _seconds / 60;
        var s = _seconds % 60;
        _suppressDisplayTimeUpdate = true;
        DisplayTime = $"{m:D2}:{s:D2}";
        _suppressDisplayTimeUpdate = false;
    }

    private void UpdateColors()
    {
        PomodoroColor = _mode == 0 ? Colors.Blue : Colors.Gray;
        RestColor = _mode == 1 ? Colors.Green : Colors.Gray;
        LongRestColor = _mode == 2 ? Colors.DarkGreen : Colors.Gray;

        ModeColor = _mode switch
        {
            0 => Colors.Blue,
            1 => Colors.Green,
            2 => Colors.DarkGreen,
            _ => Colors.Blue
        };
    }


    [RelayCommand]
    private async Task AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle))
            return;

        var task = new TaskItem { Title = NewTaskTitle };
        await _db.SaveTaskAsync(task);

        Tasks.Add(task);
        NewTaskTitle = "";
    }

    [RelayCommand]
    private async Task DeleteTask(TaskItem task)
    {
        if (task == null) return;

        await _db.DeleteTaskAsync(task);
        Tasks.Remove(task);
    }

    [RelayCommand]
    private async Task LoadTasks()
    {
        var list = await _db.GetTasksAsync();
        Tasks = new ObservableCollection<TaskItem>(list);
    }
}
