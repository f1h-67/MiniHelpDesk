namespace MiniHelpDesk.Application;

/* klasa: AddTicketCommand
 opis: Przekazuje dane nowego zgłoszenia do handlera dodawania.
 pola / właściwości: TicketId, Guid, identyfikator nowego zgłoszenia; Name, string, nazwa zgłoszenia; Description, string, opis zgłoszenia; Priority, string, wybrany priorytet */
public sealed record AddTicketCommand(Guid TicketId, string Name, string Description, string Priority);

/* klasa: DeleteTicketCommand
 opis: Przekazuje identyfikator zgłoszenia do usunięcia.
 pola / właściwości: TicketId, Guid, identyfikator zgłoszenia */
public sealed record DeleteTicketCommand(Guid TicketId);

/* klasa: UpdateTicketCommand
 opis: Przekazuje nowe dane edytowanego zgłoszenia.
 pola / właściwości: TicketId, Guid, identyfikator zgłoszenia; Name, string, nowa nazwa; Description, string, nowy opis; Priority, string, nowy priorytet */
public sealed record UpdateTicketCommand(Guid TicketId, string Name, string Description, string Priority);

/* klasa: ChangeTicketStatusCommand
 opis: Przekazuje nowy status zgłoszenia do handlera zmiany statusu.
 pola / właściwości: TicketId, Guid, identyfikator zgłoszenia; Status, string, nowy status */
public sealed record ChangeTicketStatusCommand(Guid TicketId, string Status);

/* klasa: GetTicketsQuery
 opis: Reprezentuje zapytanie o wszystkie zapisane zgłoszenia.
 pola / właściwości: brak */
public sealed record GetTicketsQuery;
