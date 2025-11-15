using CommunityToolkit.Maui;
using Pomodoro.Services;
using Pomodoro.ViewModels;
using Pomodoro.Views;

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
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>(sp =>
            new MainPage(sp.GetRequiredService<MainViewModel>()));

        return builder.Build();
    }
}