using MiniHelpDesk.Domain;

namespace MiniHelpDesk.Infrastructure;

/* klasa: ITicketRepository
 opis: Określa operacje zapisu i odczytu zgłoszeń niezależne od miejsca przechowywania.
 pola / właściwości: brak */
public interface ITicketRepository
{
    /* funkcja: Add
     opis: Dodaje zgłoszenie do repozytorium.
     parametry: ticket, Ticket, zgłoszenie do zapisania
     zwraca: void, nic nie zwraca */
    void Add(Ticket ticket);

    /* funkcja: GetAll
     opis: Zwraca wszystkie zgłoszenia.
     parametry: brak
     zwraca: IReadOnlyList<Ticket>, kopia listy zgłoszeń */
    IReadOnlyList<Ticket> GetAll();

    /* funkcja: GetById
     opis: Wyszukuje zgłoszenie po identyfikatorze.
     parametry: id, Guid, identyfikator zgłoszenia
     zwraca: Ticket?, znalezione zgłoszenie albo null */
    Ticket? GetById(Guid id);

    /* funkcja: Delete
     opis: Usuwa zgłoszenie o podanym identyfikatorze.
     parametry: id, Guid, identyfikator zgłoszenia do usunięcia
     zwraca: bool, true jeśli zgłoszenie zostało usunięte */
    bool Delete(Guid id);
}
