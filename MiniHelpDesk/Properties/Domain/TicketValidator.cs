namespace MiniHelpDesk.Domain;

/* klasa: TicketValidator
 opis: Waliduje nazwę, opis i priorytet przed zapisem zgłoszenia.
 pola / właściwości: brak */
public sealed class TicketValidator
{
    /* funkcja: Validate
     opis: Sprawdza długość nazwy i opisu po przycięciu brzegowych spacji oraz poprawność priorytetu.
     parametry: name, string, nazwa zgłoszenia; description, string, opis zgłoszenia; priority, string?, wybrany priorytet
     zwraca: TicketValidationResult, wynik walidacji z komunikatami dla pól */
    public TicketValidationResult Validate(string? name, string? description, string? priority)
    {
        var errors = new Dictionary<string, string>();
        var cleanName = name?.Trim() ?? string.Empty;
        var cleanDescription = description?.Trim() ?? string.Empty;

        if (cleanName.Length < 3)
        {
            errors[nameof(Ticket.Name)] = "Nazwa musi mieć co najmniej 3 znaki.";
        }

        if (cleanDescription.Length < 10)
        {
            errors[nameof(Ticket.Description)] = "Opis musi mieć co najmniej 10 znaków.";
        }

        if (string.IsNullOrWhiteSpace(priority) || !TicketOptions.Priorities.Contains(priority.Trim(), StringComparer.Ordinal))
        {
            errors[nameof(Ticket.Priority)] = "Wybierz priorytet: Niski, Średni lub Wysoki.";
        }

        return new TicketValidationResult(errors);
    }
}

/* klasa: TicketValidationException
 opis: Zatrzymuje komendę, gdy dane zgłoszenia nie przechodzą walidacji.
 pola / właściwości: Validation, TicketValidationResult, błędy walidacji powodujące odrzucenie komendy */
public sealed class TicketValidationException : InvalidOperationException
{
    /* funkcja: TicketValidationException
     opis: Tworzy wyjątek zawierający szczegółowy wynik walidacji.
     parametry: validation, TicketValidationResult, wynik walidacji odrzuconych danych
     zwraca: void, nic nie zwraca */
    public TicketValidationException(TicketValidationResult validation)
        : base("Zgłoszenie zawiera niepoprawne dane.")
    {
        Validation = validation;
    }

    public TicketValidationResult Validation { get; }
}
