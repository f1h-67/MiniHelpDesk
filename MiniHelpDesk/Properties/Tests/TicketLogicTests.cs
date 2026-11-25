using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniHelpDesk.Application;
using MiniHelpDesk.Domain;
using MiniHelpDesk.Infrastructure;

namespace MiniHelpDesk.Tests;

/* klasa: TicketLogicTests
 opis: Zawiera testy MSTest walidacji, Command Bus, repozytorium i powiadomień Observable.
 pola / właściwości: brak */
[TestClass]
public sealed class TicketLogicTests
{
    /* funkcja: ValidatorAcceptsTrimmedValidValues
     opis: Sprawdza, że dozwolone dane po usunięciu spacji brzegowych przechodzą walidację.
     parametry: brak
     zwraca: void, nic nie zwraca */
    [TestMethod]
    public void ValidatorAcceptsTrimmedValidValues()
    {
        var result = new TicketValidator().Validate("  Drukarka  ", "  Nie drukuje dokumentów  ", "Średni");

        Assert.IsTrue(result.IsValid);
        Assert.AreEqual(0, result.Errors.Count);
    }

    /* funkcja: ValidatorRejectsShortName
     opis: Sprawdza, że nazwa krótsza niż trzy znaki jest odrzucana.
     parametry: brak
     zwraca: void, nic nie zwraca */
    [TestMethod]
    public void ValidatorRejectsShortName()
    {
        var result = new TicketValidator().Validate(" ab ", "Poprawny opis problemu", "Niski");

        Assert.IsFalse(result.IsValid);
        Assert.IsNotNull(result.GetError(nameof(Ticket.Name)));
    }

    /* funkcja: ValidatorRejectsShortDescription
     opis: Sprawdza, że opis krótszy niż dziesięć znaków jest odrzucany.
     parametry: brak
     zwraca: void, nic nie zwraca */
    [TestMethod]
    public void ValidatorRejectsShortDescription()
    {
        var result = new TicketValidator().Validate("Sieć", " za krótki ", "Wysoki");

        Assert.IsFalse(result.IsValid);
        Assert.IsNotNull(result.GetError(nameof(Ticket.Description)));
    }

    /* funkcja: ValidatorRequiresKnownPriority
     opis: Sprawdza, że brak lub nieznany priorytet uniemożliwia zapis.
     parametry: brak
     zwraca: void, nic nie zwraca */
    [TestMethod]
    public void ValidatorRequiresKnownPriority()
    {
        var result = new TicketValidator().Validate("Komputer", "Nie uruchamia się komputer", "Pilny");

        Assert.IsFalse(result.IsValid);
        Assert.IsNotNull(result.GetError(nameof(Ticket.Priority)));
    }

    /* funkcja: AddCommandCreatesTicketWithNewStatus
     opis: Sprawdza przepływ komendy przez Command Bus i zapis nowego zgłoszenia.
     parametry: brak
     zwraca: void, nic nie zwraca */
    [TestMethod]
    public void AddCommandCreatesTicketWithNewStatus()
    {
        var (repository, bus) = CreateBus();
        var id = Guid.NewGuid();

        bus.Execute(new AddTicketCommand(id, "  Monitor  ", "  Ekran gaśnie po chwili  ", "Wysoki"));

        var ticket = repository.GetById(id);
        Assert.IsNotNull(ticket);
        Assert.AreEqual("Monitor", ticket.Name);
        Assert.AreEqual("Ekran gaśnie po chwili", ticket.Description);
        Assert.AreEqual("Nowe", ticket.Status);
    }

    /* funkcja: InvalidAddCommandDoesNotSave
     opis: Sprawdza, że handler nie zapisuje komendy z niepoprawnymi danymi.
     parametry: brak
     zwraca: void, nic nie zwraca */
    [TestMethod]
    public void InvalidAddCommandDoesNotSave()
    {
        var (repository, bus) = CreateBus();
        var id = Guid.NewGuid();

        Assert.Throws<TicketValidationException>(() => bus.Execute(new AddTicketCommand(id, "PC", "Za krótko", "Niski")));
        Assert.IsNull(repository.GetById(id));
        Assert.AreEqual(0, repository.GetAll().Count);
    }

    /* funkcja: DeleteCommandRemovesTicket
     opis: Sprawdza, że komenda usunięcia usuwa zgłoszenie z repozytorium.
     parametry: brak
     zwraca: void, nic nie zwraca */
    [TestMethod]
    public void DeleteCommandRemovesTicket()
    {
        var (repository, bus) = CreateBus();
        var id = Guid.NewGuid();
        bus.Execute(new AddTicketCommand(id, "Klawiatura", "Nie działa klawisz Enter", "Średni"));

        bus.Execute(new DeleteTicketCommand(id));

        Assert.IsNull(repository.GetById(id));
    }

    /* funkcja: UpdateCommandRaisesPropertyNotifications
     opis: Sprawdza edycję zgłoszenia oraz powiadomienia INotifyPropertyChanged.
     parametry: brak
     zwraca: void, nic nie zwraca */
    [TestMethod]
    public void UpdateCommandRaisesPropertyNotifications()
    {
        var (repository, bus) = CreateBus();
        var id = Guid.NewGuid();
        bus.Execute(new AddTicketCommand(id, "Drukarka", "Nie drukuje dokumentów", "Niski"));
        var ticket = repository.GetById(id)!;
        var changed = new HashSet<string>();
        ticket.PropertyChanged += (_, args) => changed.Add(args.PropertyName ?? string.Empty);

        bus.Execute(new UpdateTicketCommand(id, "Drukarka biurowa", "Nie drukuje kolorowych dokumentów", "Wysoki"));

        Assert.AreSame(ticket, repository.GetById(id));
        Assert.AreEqual("Drukarka biurowa", ticket.Name);
        Assert.AreEqual("Wysoki", ticket.Priority);
        Assert.IsTrue(changed.Contains(nameof(Ticket.Name)));
        Assert.IsTrue(changed.Contains(nameof(Ticket.Description)));
        Assert.IsTrue(changed.Contains(nameof(Ticket.Priority)));
    }

    /* funkcja: StatusCommandNotifiesWithoutReplacingTicket
     opis: Sprawdza zmianę statusu, powiadomienie obserwatora i zachowanie tej samej instancji zgłoszenia.
     parametry: brak
     zwraca: void, nic nie zwraca */
    [TestMethod]
    public void StatusCommandNotifiesWithoutReplacingTicket()
    {
        var (repository, bus) = CreateBus();
        var id = Guid.NewGuid();
        bus.Execute(new AddTicketCommand(id, "Łącze", "Połączenie często się przerywa", "Średni"));
        var ticket = repository.GetById(id)!;
        var statusChanged = false;
        ticket.PropertyChanged += (_, args) => statusChanged |= args.PropertyName == nameof(Ticket.Status);

        bus.Execute(new ChangeTicketStatusCommand(id, "W trakcie"));

        Assert.AreSame(ticket, repository.GetById(id));
        Assert.AreEqual("W trakcie", ticket.Status);
        Assert.IsTrue(statusChanged);
    }

    private static (InMemoryTicketRepository Repository, CommandBus Bus) CreateBus()
    {
        var repository = new InMemoryTicketRepository();
        var validator = new TicketValidator();
        var bus = new CommandBus();
        bus.Register(new AddTicketHandler(repository, validator));
        bus.Register(new DeleteTicketHandler(repository));
        bus.Register(new UpdateTicketHandler(repository, validator));
        bus.Register(new ChangeTicketStatusHandler(repository));
        return (repository, bus);
    }
}
