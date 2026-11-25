using MiniHelpDesk.Domain;

namespace MiniHelpDesk.Infrastructure;

/* klasa: InMemoryTicketRepository
 opis: Przechowuje zgłoszenia w pamięci podczas działania aplikacji.
 pola / właściwości: _tickets, Dictionary<Guid, Ticket>, zgłoszenia indeksowane unikalnym identyfikatorem */
public sealed class InMemoryTicketRepository : ITicketRepository
{
    private readonly Dictionary<Guid, Ticket> _tickets = [];

    /* funkcja: Add
     opis: Zapisuje zgłoszenie i odrzuca powtórzony identyfikator.
     parametry: ticket, Ticket, zgłoszenie do zapisania
     zwraca: void, nic nie zwraca */
    public void Add(Ticket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        if (!_tickets.TryAdd(ticket.Id, ticket))
        {
            throw new InvalidOperationException("Zgłoszenie o takim identyfikatorze już istnieje.");
        }
    }

    /* funkcja: GetAll
     opis: Zwraca kopię listy zgłoszeń w kolejności dodawania.
     parametry: brak
     zwraca: IReadOnlyList<Ticket>, lista zgłoszeń */
    public IReadOnlyList<Ticket> GetAll() => _tickets.Values.ToList();

    /* funkcja: GetById
     opis: Wyszukuje zgłoszenie po identyfikatorze.
     parametry: id, Guid, identyfikator zgłoszenia
     zwraca: Ticket?, znalezione zgłoszenie albo null */
    public Ticket? GetById(Guid id) => _tickets.GetValueOrDefault(id);

    /* funkcja: Delete
     opis: Usuwa zgłoszenie z repozytorium.
     parametry: id, Guid, identyfikator zgłoszenia do usunięcia
     zwraca: bool, true jeśli zgłoszenie zostało usunięte */
    public bool Delete(Guid id) => _tickets.Remove(id);
}
