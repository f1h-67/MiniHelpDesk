namespace MiniHelpDesk.Domain;

/* klasa: TicketValidationResult
 opis: Przechowuje komunikaty walidacji pól zgłoszenia.
 pola / właściwości: Errors, IReadOnlyDictionary<string, string>, komunikaty przypisane do pól; IsValid, bool, informacja czy nie ma błędów */
public sealed class TicketValidationResult
{
    /* funkcja: TicketValidationResult
     opis: Tworzy wynik walidacji z komunikatami przypisanymi do pól.
     parametry: errors, IReadOnlyDictionary<string, string>, błędy walidacji
     zwraca: void, nic nie zwraca */
    public TicketValidationResult(IReadOnlyDictionary<string, string> errors)
    {
        Errors = errors;
    }

    public IReadOnlyDictionary<string, string> Errors { get; }
    public bool IsValid => Errors.Count == 0;

    /* funkcja: GetError
     opis: Zwraca komunikat walidacji wskazanego pola.
     parametry: fieldName, string, nazwa pola zgłoszenia
     zwraca: string?, komunikat błędu albo null gdy pole jest poprawne */
    public string? GetError(string fieldName) => Errors.TryGetValue(fieldName, out var error) ? error : null;
}
