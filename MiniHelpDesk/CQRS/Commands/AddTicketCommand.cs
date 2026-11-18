using MiniHelpDesk.Models;

namespace MiniHelpDesk.CQRS.Commands
{
    /*
    klasa: AddTicketCommand

    opis:
    Komenda odpowiedzialna za przekazanie danych nowego zgłoszenia.

    pola / właściwości:
    Ticket, Ticket, obiekt zgłoszenia do dodania
    */
    public class AddTicketCommand
    {
        public Ticket Ticket { get; set; }
        public AddTicketCommand(Ticket ticket) => Ticket = ticket;
    }
}