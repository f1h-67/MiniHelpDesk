namespace MiniHelpDesk.CQRS.Commands
{
    /*
    klasa: DeleteTicketCommand

    opis:
    Komenda odpowiedzialna za żądanie usunięcia zgłoszenia po ID.

    pola / właściwości:
    Id, string, identyfikator zgłoszenia do usunięcia
    */
    public class DeleteTicketCommand
    {
        public string Id { get; set; }
        public DeleteTicketCommand(string id) => Id = id;
    }
}