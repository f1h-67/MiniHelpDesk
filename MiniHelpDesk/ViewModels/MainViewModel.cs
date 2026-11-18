using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MiniHelpDesk.CQRS;
using MiniHelpDesk.CQRS.Commands;
using MiniHelpDesk.CQRS.Queries;
using MiniHelpDesk.Models;

namespace MiniHelpDesk.ViewModels
{
    /*
    klasa: MainViewModel

    opis:
    ViewModel obsługujący logikę widoku głównego, wiązanie danych i walidację formularza.

    pola / właściwości:
    Tickets, ObservableCollection<Ticket>, kolekcja zgłoszeń widoczna na UI
    Name, string, wpisywana nazwa
    Description, string, wpisywany opis
    SelectedPriority, string, wybrany priorytet
    ValidationMessage, string, komunikat o błędach walidacji
    */
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CommandBus _commandBus;

        public ObservableCollection<Ticket> Tickets { get; set; } = new();

        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        private string _selectedPriority = "Średni";
        public string SelectedPriority
        {
            get => _selectedPriority;
            set { _selectedPriority = value; OnPropertyChanged(); }
        }

        private string _validationMessage = string.Empty;
        public string ValidationMessage
        {
            get => _validationMessage;
            set { _validationMessage = value; OnPropertyChanged(); }
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public MainViewModel(CommandBus commandBus)
        {
            _commandBus = commandBus;
            AddCommand = new Command(OnAdd);
            DeleteCommand = new Command<Ticket>(OnDelete);
            LoadTickets();
        }

        /*
        funkcja: OnAdd

        opis:
        Waliduje dane wejściowe i dodaje zgłoszenie poprzez CommandBus.

        parametry:
        brak

        zwraca:
        void, nic nie zwraca
        */
        private void OnAdd()
        {
            string trimmedName = Name?.Trim() ?? string.Empty;
            string trimmedDesc = Description?.Trim() ?? string.Empty;

            if (trimmedName.Length < 3)
            {
                ValidationMessage = "Nazwa musi mieć minimum 3 znaki.";
                return;
            }

            if (trimmedDesc.Length < 10)
            {
                ValidationMessage = "Opis musi mieć minimum 10 znaków.";
                return;
            }

            if (string.IsNullOrEmpty(SelectedPriority))
            {
                ValidationMessage = "Wybierz priorytet.";
                return;
            }

            ValidationMessage = string.Empty;

            var newTicket = new Ticket
            {
                Name = trimmedName,
                Description = trimmedDesc,
                Priority = SelectedPriority,
                Status = "Nowe"
            };

            _commandBus.HandleAdd(new AddTicketCommand(newTicket));
            LoadTickets();

            Name = string.Empty;
            Description = string.Empty;
        }

        /*
        funkcja: OnDelete

        opis:
        Usuwa wybrane zgłoszenie za pomocą komendy i odświeża listę.

        parametry:
        ticket, Ticket, zgłoszenie do usunięcia

        zwraca:
        void, nic nie zwraca
        */
        private void OnDelete(Ticket ticket)
        {
            if (ticket != null)
            {
                _commandBus.HandleDelete(new DeleteTicketCommand(ticket.Id));
                LoadTickets();
            }
        }

        /*
        funkcja: LoadTickets

        opis:
        Odświeża kolekcję zgłoszeń na podstawie zapytania CQRS.

        parametry:
        brak

        zwraca:
        void, nic nie zwraca
        */
        private void LoadTickets()
        {
            var list = _commandBus.HandleQuery(new GetTicketsQuery());
            Tickets.Clear();
            foreach (var t in list)
            {
                Tickets.Add(t);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}