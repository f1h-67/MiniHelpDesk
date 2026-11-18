using System.Collections.Generic;
using System.Linq;
using MiniHelpDesk.Models;

namespace MiniHelpDesk.Repositories
{
    /*
    klasa: InMemoryTicketRepository

    opis:
    Implementacja repozytorium zgłoszeń przechowująca dane w pamięci aplikacji.

    pola / właściwości:
    _tickets, List<Ticket>, wewnętrzna lista zgłoszeń
    */
    public class InMemoryTicketRepository : ITicketRepository
    {
        private readonly List<Ticket> _tickets = new();

        /*
        funkcja: Add

        opis:
        Dodaje nowe zgłoszenie do pamięci.

        parametry:
        ticket, Ticket, zgłoszenie do dodania

        zwraca:
        void, nic nie zwraca
        */
        public void Add(Ticket ticket)
        {
            _tickets.Add(ticket);
        }

        /*
        funkcja: Delete

        opis:
        Usuwa zgłoszenie na podstawie jego identyfikatora.

        parametry:
        id, string, identyfikator zgłoszenia do usunięcia

        zwraca:
        void, nic nie zwraca
        */
        public void Delete(string id)
        {
            var item = _tickets.FirstOrDefault(t => t.Id == id);
            if (item != null)
            {
                _tickets.Remove(item);
            }
        }

        /*
        funkcja: GetAll

        opis:
        Zwraca listę wszystkich zgłoszeń.

        parametry:
        brak

        zwraca:
        List<Ticket>, lista wszystkich zgłoszeń
        */
        public List<Ticket> GetAll()
        {
            return new List<Ticket>(_tickets);
        }
    }
}