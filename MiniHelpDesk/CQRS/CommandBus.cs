using System.Collections.Generic;
using MiniHelpDesk.CQRS.Commands;
using MiniHelpDesk.CQRS.Queries;
using MiniHelpDesk.Models;
using MiniHelpDesk.Repositories;

namespace MiniHelpDesk.CQRS
{
    /*
    klasa: CommandBus

    opis:
    Pośrednik routujący komendy i zapytania do odpowiednich handlerów.

    pola / właściwości:
    _repository, ITicketRepository, repozytorium zgłoszeń
    */
    public class CommandBus
    {
        private readonly ITicketRepository _repository;

        public CommandBus(ITicketRepository repository)
        {
            _repository = repository;
        }

        /*
        funkcja: HandleAdd
        opis: Obsługuje dodanie zgłoszenia przez repozytorium.
        parametry: command, AddTicketCommand, komenda dodania
        zwraca: void, nic nie zwraca
        */
        public void HandleAdd(AddTicketCommand command)
        {
            _repository.Add(command.Ticket);
        }

        /*
        funkcja: HandleDelete
        opis: Obsługuje usunięcie zgłoszenia przez repozytorium.
        parametry: command, DeleteTicketCommand, komenda usunięcia
        zwraca: void, nic nie zwraca
        */
        public void HandleDelete(DeleteTicketCommand command)
        {
            _repository.Delete(command.Id);
        }

        /*
        funkcja: HandleQuery
        opis: Obsługuje zapytanie o pobranie listy zgłoszeń.
        parametry: query, GetTicketsQuery, zapytanie o listę
        zwraca: List<Ticket>, lista zgłoszeń
        */
        public List<Ticket> HandleQuery(GetTicketsQuery query)
        {
            return _repository.GetAll();
        }
    }
}