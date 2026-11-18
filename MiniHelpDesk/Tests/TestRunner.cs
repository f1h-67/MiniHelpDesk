using System;
using MiniHelpDesk.CQRS;
using MiniHelpDesk.CQRS.Commands;
using MiniHelpDesk.CQRS.Queries;
using MiniHelpDesk.Models;
using MiniHelpDesk.Repositories;

namespace MiniHelpDesk.Tests
{
    /*
    klasa: TestRunner

    opis:
    Zawiera testy jednostkowe oparte na asercjach MSTest uruchamiane z poziomu aplikacji.

    pola / właściwości:
    brak
    */
    public static class TestRunner
    {
        /*
        funkcja: RunAllTests

        opis:
        Wykonuje zestaw testów jednostkowych i zwraca podsumowanie w formie tekstu.

        parametry:
        brak

        zwraca:
        string, podsumowanie wyników testów
        */
        public static string RunAllTests()
        {
            int passed = 0;
            int total = 3;
            string report = "";

            // Test 1: Dodawanie zgłoszenia przez repozytorium
            try
            {
                var repo = new InMemoryTicketRepository();
                var t = new Ticket { Name = "Test Awaria", Description = "Opis awarii systemu testowego", Priority = "Wysoki" };
                repo.Add(t);
                var all = repo.GetAll();

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, all.Count);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("Test Awaria", all[0].Name);
                passed++;
                report += "[OK] Test 1: Dodawanie zgłoszenia\n";
            }
            catch (Exception ex)
            {
                report += $"[BŁĄD] Test 1: {ex.Message}\n";
            }

            // Test 2: Usuwanie zgłoszenia
            try
            {
                var repo = new InMemoryTicketRepository();
                var t = new Ticket { Id = "123", Name = "Do usunięcia", Description = "Opis testowy długi", Priority = "Niski" };
                repo.Add(t);
                repo.Delete("123");
                var all = repo.GetAll();

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(0, all.Count);
                passed++;
                report += "[OK] Test 2: Usuwanie zgłoszenia\n";
            }
            catch (Exception ex)
            {
                report += $"[BŁĄD] Test 2: {ex.Message}\n";
            }

            // Test 3: Przepływ CQRS (CommandBus -> Handler -> Query)
            try
            {
                var repo = new InMemoryTicketRepository();
                var bus = new CommandBus(repo);
                var t = new Ticket { Name = "CQRS Awaria", Description = "Opis zgłoszenia CQRS", Priority = "Średni" };

                bus.HandleAdd(new AddTicketCommand(t));
                var resultList = bus.HandleQuery(new GetTicketsQuery());

                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(1, resultList.Count);
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual("CQRS Awaria", resultList[0].Name);
                passed++;
                report += "[OK] Test 3: Przepływ CQRS\n";
            }
            catch (Exception ex)
            {
                report += $"[BŁĄD] Test 3: {ex.Message}\n";
            }

            return $"Wynik testów: {passed}/{total} zaliczonych.\n\n" + report;
        }
    }
}