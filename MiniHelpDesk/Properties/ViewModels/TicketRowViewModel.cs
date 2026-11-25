using System.ComponentModel;
using MiniHelpDesk.Domain;

namespace MiniHelpDesk.ViewModels;

/* klasa: TicketRowViewModel
 opis: Udostępnia dane jednego zgłoszenia dla wiersza CollectionView i wysyła zmianę statusu jako komendę.
 pola / właściwości: Ticket, Ticket, źródłowe zgłoszenie; Name, string, nazwa; Description, string, opis; Priority, string, priorytet; Status, string, aktualny status; SelectedStatus, string, status wybrany w kontrolce */
public sealed class TicketRowViewModel : ObservableObject
{
    private readonly Action<Guid, string> _changeStatus;

    /* funkcja: TicketRowViewModel
     opis: Tworzy element widoku powiązany z jednym zgłoszeniem.
     parametry: ticket, Ticket, zgłoszenie źródłowe; changeStatus, Action<Guid, string>, akcja wysyłająca komendę zmiany statusu
     zwraca: void, nic nie zwraca */
    public TicketRowViewModel(Ticket ticket, Action<Guid, string> changeStatus)
    {
        Ticket = ticket;
        _changeStatus = changeStatus;
        Ticket.PropertyChanged += OnTicketPropertyChanged;
    }

    public Ticket Ticket { get; }
    public string Name => Ticket.Name;
    public string Description => Ticket.Description;
    public string Priority => Ticket.Priority;
    public string Status => Ticket.Status;

    public string SelectedStatus
    {
        get => Ticket.Status;
        set
        {
            if (!string.IsNullOrWhiteSpace(value) && value != Ticket.Status)
            {
                _changeStatus(Ticket.Id, value);
            }
        }
    }

    private void OnTicketPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is null or nameof(Ticket.Status))
        {
            OnPropertyChanged(nameof(Status));
            OnPropertyChanged(nameof(SelectedStatus));
        }

        if (args.PropertyName is null or nameof(Ticket.Name))
        {
            OnPropertyChanged(nameof(Name));
        }

        if (args.PropertyName is null or nameof(Ticket.Description))
        {
            OnPropertyChanged(nameof(Description));
        }

        if (args.PropertyName is null or nameof(Ticket.Priority))
        {
            OnPropertyChanged(nameof(Priority));
        }
    }
}
