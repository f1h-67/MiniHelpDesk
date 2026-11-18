using MiniHelpDesk.CQRS;
using MiniHelpDesk.Repositories;
using MiniHelpDesk.ViewModels;
using MiniHelpDesk.Views;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();
        builder.Services.AddSingleton<CommandBus>();
        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}