using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using MiniHelpDesk.Application;
using MiniHelpDesk.Domain;
using MiniHelpDesk.Infrastructure;
using MiniHelpDesk.Tests;
using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk;

/* klasa: MauiProgram
 opis: Rejestruje strony i wspólne usługi aplikacji w kontenerze MAUI.
 pola / właściwości: brak */
public static class MauiProgram
{
    /* funkcja: CreateMauiApp
     opis: Buduje aplikację MAUI oraz rejestruje repozytorium, Command Bus, handlery i ViewModel.
     parametry: brak
     zwraca: MauiApp, skonfigurowana aplikacja MAUI */
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddSingleton<ITicketRepository, InMemoryTicketRepository>();
        builder.Services.AddSingleton<TicketValidator>();
        builder.Services.AddSingleton<AddTicketHandler>();
        builder.Services.AddSingleton<DeleteTicketHandler>();
        builder.Services.AddSingleton<UpdateTicketHandler>();
        builder.Services.AddSingleton<ChangeTicketStatusHandler>();
        builder.Services.AddSingleton<GetTicketsHandler>();
        builder.Services.AddSingleton<CommandBus>(services =>
        {
            var bus = new CommandBus();
            bus.Register(services.GetRequiredService<AddTicketHandler>());
            bus.Register(services.GetRequiredService<DeleteTicketHandler>());
            bus.Register(services.GetRequiredService<UpdateTicketHandler>());
            bus.Register(services.GetRequiredService<ChangeTicketStatusHandler>());
            return bus;
        });
        builder.Services.AddSingleton<MSTestRunner>();
        builder.Services.AddSingleton<HelpDeskViewModel>();
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
