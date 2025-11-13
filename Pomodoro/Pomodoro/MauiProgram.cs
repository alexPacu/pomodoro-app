using Pomodoro.ViewModels;
using CommunityToolkit.Maui;
using Pomodoro.Services;
using Pomodoro.Repositories;

namespace Pomodoro;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(f => f.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"));

        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<TaskRepository>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<TasksViewModel>();

        return builder.Build();
    }
}