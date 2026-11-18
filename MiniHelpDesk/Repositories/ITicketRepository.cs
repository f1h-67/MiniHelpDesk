using System.Collections.Generic;
using MiniHelpDesk.Models;

namespace MiniHelpDesk.Repositories
{
    /*
    klasa: ITicketRepository

    opis:
    Interfejs definiujący operacje dostępu do danych zgłoszeń w pamięci.

    pola / właściwości:
    brak
    */
    public interface ITicketRepository
    {
        void Add(Ticket ticket);
        void Delete(string id);
        List<Ticket> GetAll();
    }
}