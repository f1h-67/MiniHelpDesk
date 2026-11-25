using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MiniHelpDesk.Domain;

/* klasa: Ticket
 opis: Przechowuje dane zgłoszenia i powiadamia interfejs o zmianie właściwości.
 pola / właściwości: Id, Guid, unikalny identyfikator; Name, string, nazwa problemu; Description, string, opis problemu; Priority, string, priorytet zgłoszenia; Status, string, aktualny status */
public sealed class Ticket : INotifyPropertyChanged
{
    private string _name;
    private string _description;
    private string _priority;
    private string _status = "Nowe";

    /* funkcja: Ticket
     opis: Tworzy zgłoszenie o podanym identyfikatorze i szczegółach.
     parametry: id, Guid, unikalny identyfikator zgłoszenia; name, string, nazwa problemu; description, string, opis problemu; priority, string, priorytet zgłoszenia
     zwraca: void, nic nie zwraca */
    public Ticket(Guid id, string name, string description, string priority)
    {
        Id = id;
        _name = name;
        _description = description;
        _priority = priority;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public Guid Id { get; }
    public string Name => _name;
    public string Description => _description;
    public string Priority => _priority;
    public string Status => _status;

    /* funkcja: UpdateDetails
     opis: Aktualizuje nazwę, opis i priorytet istniejącego zgłoszenia.
     parametry: name, string, nowa nazwa; description, string, nowy opis; priority, string, nowy priorytet
     zwraca: void, nic nie zwraca */
    public void UpdateDetails(string name, string description, string priority)
    {
        SetProperty(ref _name, name.Trim(), nameof(Name));
        SetProperty(ref _description, description.Trim(), nameof(Description));
        SetProperty(ref _priority, priority.Trim(), nameof(Priority));
    }

    /* funkcja: ChangeStatus
     opis: Zmienia status zgłoszenia i zgłasza powiadomienie o zmianie właściwości.
     parametry: status, string, nowy status zgłoszenia
     zwraca: void, nic nie zwraca */
    public void ChangeStatus(string status)
    {
        SetProperty(ref _status, status, nameof(Status));
    }

    private void SetProperty(ref string storage, string value, string propertyName)
    {
        if (storage == value)
        {
            return;
        }

        storage = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
