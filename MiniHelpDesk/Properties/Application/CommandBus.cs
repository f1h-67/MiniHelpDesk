namespace MiniHelpDesk.Application;

/* klasa: ICommandHandler
 opis: Wspólny kontrakt handlera obsługującego komendę zmieniającą dane.
 pola / właściwości: brak */
public interface ICommandHandler<in TCommand>
{
    /* funkcja: Handle
     opis: Obsługuje komendę i wykonuje odpowiednią zmianę danych.
     parametry: command, TCommand, komenda przekazana do handlera
     zwraca: void, nic nie zwraca */
    void Handle(TCommand command);
}

/* klasa: CommandBus
 opis: Kieruje komendy do zarejestrowanych handlerów bez łączenia widoku z repozytorium.
 pola / właściwości: _handlers, Dictionary<Type, Action<object>>, handlery komend indeksowane typem komendy */
public sealed class CommandBus
{
    private readonly Dictionary<Type, Action<object>> _handlers = [];

    /* funkcja: Register
     opis: Rejestruje handler dla konkretnego typu komendy.
     parametry: handler, ICommandHandler<TCommand>, handler komendy
     zwraca: void, nic nie zwraca */
    public void Register<TCommand>(ICommandHandler<TCommand> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        if (!_handlers.TryAdd(typeof(TCommand), command => handler.Handle((TCommand)command)))
        {
            throw new InvalidOperationException($"Handler dla {typeof(TCommand).Name} jest już zarejestrowany.");
        }
    }

    /* funkcja: Execute
     opis: Wysyła komendę do zarejestrowanego handlera.
     parametry: command, TCommand, komenda zmieniająca dane
     zwraca: void, nic nie zwraca */
    public void Execute<TCommand>(TCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (!_handlers.TryGetValue(typeof(TCommand), out var handler))
        {
            throw new InvalidOperationException($"Brak handlera dla {typeof(TCommand).Name}.");
        }

        handler(command);
    }
}
