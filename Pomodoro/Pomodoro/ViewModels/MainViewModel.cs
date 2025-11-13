using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pomodoro.Models;
using Pomodoro.Services;
using System.Collections.ObjectModel;
using System.Timers;
using Microsoft.Maui.ApplicationModel.Communication;

namespace Pomodoro.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly DatabaseService _dbService;
    private readonly System.Timers.Timer _timer;

    public MainViewModel(DatabaseService dbService)
    {
        _dbService = dbService;
        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += OnTimerElapsed;
        ResetCommand.Execute(null);
    }

    [ObservableProperty] private string displayTime = "25:00";
    [ObservableProperty] private double progress = 0;
    [ObservableProperty] private string sessionType = "Munka";
    [ObservableProperty] private ObservableCollection<PomodoroSession> todaySessions = new();

    private int _secondsRemaining;
    private bool _isWorkSession = true;

    [RelayCommand]
    private void Start()
    {
        if (_timer.Enabled) return;
        _secondsRemaining = _isWorkSession ? 25 * 60 : 5 * 60;
        UpdateDisplay();
        _timer.Start();

        var session = new PomodoroSession
        {
            StartTime = DateTime.Now,
            Type = _isWorkSession ? "Work" : "Break",
            DurationMinutes = _isWorkSession ? 25 : 5
        };
        _dbService.GetConnection().InsertAsync(session);
    }

    [RelayCommand] private void Pause() => _timer.Stop();

    [RelayCommand]
    private void Reset()
    {
        _timer.Stop();
        _isWorkSession = true;
        SessionType = "Munka";
        _secondsRemaining = 25 * 60;
        Progress = 0;
        UpdateDisplay();
        LoadTodaySessions();
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        _secondsRemaining--;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpdateDisplay();
            var total = _isWorkSession ? 25 * 60 : 5 * 60;
            Progress = 1.0 - (double)_secondsRemaining / total;

            if (_secondsRemaining <= 0)
            {
                _timer.Stop();
                _isWorkSession = !_isWorkSession;
                SessionType = _isWorkSession ? "Munka" : "Szünet";
                try
                {
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));
                }
                catch
                {
                }
                Start();
            }
        });
    }

    private void UpdateDisplay()
    {
        var m = _secondsRemaining / 60;
        var s = _secondsRemaining % 60;
        DisplayTime = $"{m:D2}:{s:D2}";
    }

    private async void LoadTodaySessions()
    {
        var today = DateTime.Today;
        var sessions = await _dbService.GetConnection()
            .Table<PomodoroSession>()
            .Where(s => s.StartTime.Date == today)
            .ToListAsync();
        TodaySessions = new ObservableCollection<PomodoroSession>(sessions);
    }
}