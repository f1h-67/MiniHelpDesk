namespace MiniHelpDesk.Models
{
    /*
    klasa: Ticket

    opis:
    Reprezentuje pojedyncze zgłoszenie techniczne w systemie HelpDesk.

    pola / właściwości:
    Id, string, unikalny identyfikator zgłoszenia
    Name, string, nazwa zgłoszenia
    Description, string, opis problemu
    Priority, string, priorytet zgłoszenia (Niski, Średni, Wysoki)
    Status, string, aktualny status zgłoszenia
    */
    public class Ticket
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Średni";
        public string Status { get; set; } = "Nowe";
    }
}