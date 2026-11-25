namespace MiniHelpDesk.Domain;

/* klasa: TicketOptions
 opis: Udostępnia dozwolone priorytety i statusy zgłoszeń.
 pola / właściwości: Priorities, IReadOnlyList<string>, priorytety dostępne w formularzu; Statuses, IReadOnlyList<string>, statusy dostępne dla zgłoszeń */
public static class TicketOptions
{
    public static IReadOnlyList<string> Priorities { get; } = ["Niski", "Średni", "Wysoki"];
    public static IReadOnlyList<string> Statuses { get; } = ["Nowe", "W trakcie", "Zamknięte"];
}
