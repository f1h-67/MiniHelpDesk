using MiniHelpDesk.Domain;
using MiniHelpDesk.Infrastructure;

namespace MiniHelpDesk.Application;

/* klasa: AddTicketHandler
 opis: Waliduje i zapisuje nowe zgłoszenie w repozytorium.
 pola / właściwości: _repository, ITicketRepository, repozytorium zgłoszeń; _validator, TicketValidator, walidator danych */
public sealed class AddTicketHandler(ITicketRepository repository, TicketValidator validator) : ICommandHandler<AddTicketCommand>
{
    private readonly ITicketRepository _repository = repository;
    private readonly TicketValidator _validator = validator;

    /* funkcja: Handle
     opis: Sprawdza dane komendy i zapisuje nowe zgłoszenie ze statusem Nowe.
     parametry: command, AddTicketCommand, dane nowego zgłoszenia
     zwraca: void, nic nie zwraca */
    public void Handle(AddTicketCommand command)
    {
        var validation = _validator.Validate(command.Name, command.Description, command.Priority);
        if (!validation.IsValid)
        {
            throw new TicketValidationException(validation);
        }

        _repository.Add(new Ticket(command.TicketId, command.Name.Trim(), command.Description.Trim(), command.Priority.Trim()));
    }
}

/* klasa: DeleteTicketHandler
 opis: Usuwa zgłoszenie z repozytorium.
 pola / właściwości: _repository, ITicketRepository, repozytorium zgłoszeń */
public sealed class DeleteTicketHandler(ITicketRepository repository) : ICommandHandler<DeleteTicketCommand>
{
    private readonly ITicketRepository _repository = repository;

    /* funkcja: Handle
     opis: Wykonuje komendę usunięcia zgłoszenia.
     parametry: command, DeleteTicketCommand, identyfikator zgłoszenia do usunięcia
     zwraca: void, nic nie zwraca */
    public void Handle(DeleteTicketCommand command)
    {
        _repository.Delete(command.TicketId);
    }
}

/* klasa: UpdateTicketHandler
 opis: Waliduje i aktualizuje nazwę, opis oraz priorytet istniejącego zgłoszenia.
 pola / właściwości: _repository, ITicketRepository, repozytorium zgłoszeń; _validator, TicketValidator, walidator danych */
public sealed class UpdateTicketHandler(ITicketRepository repository, TicketValidator validator) : ICommandHandler<UpdateTicketCommand>
{
    private readonly ITicketRepository _repository = repository;
    private readonly TicketValidator _validator = validator;

    /* funkcja: Handle
     opis: Waliduje nowe dane i aktualizuje wskazane zgłoszenie.
     parametry: command, UpdateTicketCommand, identyfikator i nowe dane zgłoszenia
     zwraca: void, nic nie zwraca */
    public void Handle(UpdateTicketCommand command)
    {
        var validation = _validator.Validate(command.Name, command.Description, command.Priority);
        if (!validation.IsValid)
        {
            throw new TicketValidationException(validation);
        }

        var ticket = _repository.GetById(command.TicketId)
            ?? throw new KeyNotFoundException("Nie znaleziono zgłoszenia do edycji.");
        ticket.UpdateDetails(command.Name, command.Description, command.Priority);
    }
}

/* klasa: ChangeTicketStatusHandler
 opis: Zmienia status zgłoszenia po sprawdzeniu dozwolonych wartości.
 pola / właściwości: _repository, ITicketRepository, repozytorium zgłoszeń */
public sealed class ChangeTicketStatusHandler(ITicketRepository repository) : ICommandHandler<ChangeTicketStatusCommand>
{
    private readonly ITicketRepository _repository = repository;

    /* funkcja: Handle
     opis: Ustawia jeden z dozwolonych statusów i odrzuca nieznaną wartość.
     parametry: command, ChangeTicketStatusCommand, identyfikator i nowy status
     zwraca: void, nic nie zwraca */
    public void Handle(ChangeTicketStatusCommand command)
    {
        if (!TicketOptions.Statuses.Contains(command.Status, StringComparer.Ordinal))
        {
            throw new ArgumentException("Wybrano nieznany status zgłoszenia.", nameof(command));
        }

        var ticket = _repository.GetById(command.TicketId)
            ?? throw new KeyNotFoundException("Nie znaleziono zgłoszenia do zmiany.");
        ticket.ChangeStatus(command.Status);
    }
}

/* klasa: GetTicketsHandler
 opis: Odczytuje listę zgłoszeń na podstawie zapytania CQRS.
 pola / właściwości: _repository, ITicketRepository, źródło zgłoszeń */
public sealed class GetTicketsHandler(ITicketRepository repository)
{
    private readonly ITicketRepository _repository = repository;

    /* funkcja: Handle
     opis: Wykonuje zapytanie o wszystkie zgłoszenia.
     parametry: query, GetTicketsQuery, zapytanie o listę zgłoszeń
     zwraca: IReadOnlyList<Ticket>, odczytane zgłoszenia */
    public IReadOnlyList<Ticket> Handle(GetTicketsQuery query) => _repository.GetAll();
}
