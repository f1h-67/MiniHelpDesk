namespace MiniHelpDesk.Models;

/*
klasa: TicketValidator
opis: Waliduje pola zgłoszenia.
pola / właściwości: brak
*/
public static class TicketValidator
{
    /*
    funkcja: ValidateName
    opis: Sprawdza czy nazwa ma min 3 znaki.
    parametry: name, string?, nazwa
    zwraca: string, błąd lub pusty ciąg
    */
    public static string ValidateName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 3) return "Nazwa min. 3 znaki.";
        return "";
    }

    /*
    funkcja: ValidateDescription
    opis: Sprawdza czy opis ma min 10 znaków.
    parametry: description, string?, opis
    zwraca: string, błąd lub pusty ciąg
    */
    public static string ValidateDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description) || description.Trim().Length < 10) return "Opis min. 10 znaków.";
        return "";
    }
}