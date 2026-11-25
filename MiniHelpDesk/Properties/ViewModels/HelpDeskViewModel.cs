using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using MiniHelpDesk.Application;
using MiniHelpDesk.Domain;
using MiniHelpDesk.Tests;

namespace MiniHelpDesk.ViewModels;

/* klasa: HelpDeskViewModel
 opis: Obsługuje formularz, walidację, liczniki i komendy ekranu zgłoszeń.
 pola / właściwości: Tickets, ObservableCollection<TicketRowViewModel>, obserwowalna lista zgłoszeń; NameInput, string, wpisywana nazwa; DescriptionInput, string, wpisywany opis; PriorityInput, string, wybrany priorytet; OpenCount, int, liczba zgłoszeń innych niż zamknięte; TotalCount, int, liczba wszystkich zgłoszeń */
public sealed class HelpDeskViewModel : ObservableObject
{
    private readonly CommandBus _commandBus;
    private readonly GetTicketsHandler _getTickets;
    private readonly TicketValidator _validator;
    private readonly MSTestRunner _testRunner;
    private string _nameInput = string.Empty;
    private string _descriptionInput = string.Empty;
    private string _priorityInput = string.Empty;
    private string _testReport = string.Empty;
    private string _feedback = string.Empty;
    private TicketValidationResult _validation = new(new Dictionary<string, string>());
    private Guid? _editingTicketId;
    private bool _nameTouched;
    private bool _descriptionTouched;
    private bool _priorityTouched;
    private bool _hasSubmitted;
    private int _totalCount;
    private int _openCount;

    /* funkcja: HelpDeskViewModel
     opis: Łączy komendy i zapytania z formularzem oraz przygotowuje listę zgłoszeń.
     parametry: commandBus, CommandBus, obsługa komend; getTickets, GetTicketsHandler, odczyt zgłoszeń; validator, TicketValidator, walidacja formularza; testRunner, MSTestRunner, uruchamianie testów
     zwraca: void, nic nie zwraca */
    public HelpDeskViewModel(CommandBus commandBus, GetTicketsHandler getTickets, TicketValidator validator, MSTestRunner testRunner)
    {
        _commandBus = commandBus;
        _getTickets = getTickets;
        _validator = validator;
        _testRunner = testRunner;

        Priorities = TicketOptions.Priorities;
        Statuses = TicketOptions.Statuses;
        Tickets = new ObservableCollection<TicketRowViewModel>();
        Tickets.CollectionChanged += OnTicketsChanged;

        SaveCommand = new RelayCommand(_ => Save());
        CancelEditCommand = new RelayCommand(_ => CancelEdit(), _ => IsEditing);
        EditTicketCommand = new RelayCommand(item => BeginEdit((Ticket)item!), item => item is Ticket);
        DeleteTicketCommand = new RelayCommand(item => Delete((Ticket)item!), item => item is Ticket);
        RunTestsCommand = new RelayCommand(_ => RunTests());

        foreach (var ticket in _getTickets.Handle(new GetTicketsQuery()))
        {
            Tickets.Add(CreateRow(ticket));
        }
    }

    public ObservableCollection<TicketRowViewModel> Tickets { get; }
    public IReadOnlyList<string> Priorities { get; }
    public IReadOnlyList<string> Statuses { get; }
    public ICommand SaveCommand { get; }
    public ICommand CancelEditCommand { get; }
    public ICommand EditTicketCommand { get; }
    public ICommand DeleteTicketCommand { get; }
    public ICommand RunTestsCommand { get; }

    public string NameInput
    {
        get => _nameInput;
        set
        {
            if (SetProperty(ref _nameInput, value ?? string.Empty))
            {
                _nameTouched = true;
                ClearFeedback();
                UpdateValidation();
            }
        }
    }

    public string DescriptionInput
    {
        get => _descriptionInput;
        set
        {
            if (SetProperty(ref _descriptionInput, value ?? string.Empty))
            {
                _descriptionTouched = true;
                ClearFeedback();
                UpdateValidation();
            }
        }
    }

    public string PriorityInput
    {
        get => _priorityInput;
        set
        {
            if (SetProperty(ref _priorityInput, value ?? string.Empty))
            {
                _priorityTouched = true;
                ClearFeedback();
                UpdateValidation();
            }
        }
    }

    public string NameError => _validation.GetError(nameof(Ticket.Name)) ?? string.Empty;
    public string DescriptionError => _validation.GetError(nameof(Ticket.Description)) ?? string.Empty;
    public string PriorityError => _validation.GetError(nameof(Ticket.Priority)) ?? string.Empty;
    public bool ShowNameError => (_nameTouched || _hasSubmitted) && NameError.Length > 0;
    public bool ShowDescriptionError => (_descriptionTouched || _hasSubmitted) && DescriptionError.Length > 0;
    public bool ShowPriorityError => (_priorityTouched || _hasSubmitted) && PriorityError.Length > 0;
    public bool IsEditing => _editingTicketId.HasValue;
    public string FormTitle => IsEditing ? "Edytuj zgłoszenie" : "Nowe zgłoszenie";
    public string SaveButtonText => IsEditing ? "Zapisz zmiany" : "Dodaj zgłoszenie";
    public bool IsTestReportVisible => !string.IsNullOrWhiteSpace(TestReport);

    public string TestReport
    {
        get => _testReport;
        private set
        {
            if (SetProperty(ref _testReport, value))
            {
                OnPropertyChanged(nameof(IsTestReportVisible));
            }
        }
    }

    public string Feedback
    {
        get => _feedback;
        private set => SetProperty(ref _feedback, value);
    }

    public int TotalCount
    {
        get => _totalCount;
        private set => SetProperty(ref _totalCount, value);
    }

    public int OpenCount
    {
        get => _openCount;
        private set => SetProperty(ref _openCount, value);
    }

    /* funkcja: Save
     opis: Waliduje formularz i wysyła komendę dodania albo edycji zgłoszenia.
     parametry: brak
     zwraca: void, nic nie zwraca */
    public void Save()
    {
        _hasSubmitted = true;
        UpdateValidation();
        OnPropertyChanged(nameof(ShowNameError));
        OnPropertyChanged(nameof(ShowDescriptionError));
        OnPropertyChanged(nameof(ShowPriorityError));

        if (!_validation.IsValid)
        {
            return;
        }

        try
        {
            if (_editingTicketId is Guid ticketId)
            {
                _commandBus.Execute(new UpdateTicketCommand(ticketId, _nameInput.Trim(), _descriptionInput.Trim(), _priorityInput.Trim()));
                Feedback = "Zmiany zapisano.";
            }
            else
            {
                var newId = Guid.NewGuid();
                _commandBus.Execute(new AddTicketCommand(newId, _nameInput.Trim(), _descriptionInput.Trim(), _priorityInput.Trim()));
                var addedTicket = _getTickets.Handle(new GetTicketsQuery()).First(ticket => ticket.Id == newId);
                Tickets.Add(CreateRow(addedTicket));
                Feedback = "Zgłoszenie dodano.";
            }

            ResetForm();
        }
        catch (TicketValidationException exception)
        {
            _validation = exception.Validation;
            NotifyValidationProperties();
        }
    }

    /* funkcja: BeginEdit
     opis: Wypełnia formularz danymi wskazanego zgłoszenia.
     parametry: ticket, Ticket, zgłoszenie do edycji
     zwraca: void, nic nie zwraca */
    public void BeginEdit(Ticket ticket)
    {
        _editingTicketId = ticket.Id;
        _nameInput = ticket.Name;
        _descriptionInput = ticket.Description;
        _priorityInput = ticket.Priority;
        _nameTouched = false;
        _descriptionTouched = false;
        _priorityTouched = false;
        _hasSubmitted = false;
        _validation = _validator.Validate(_nameInput, _descriptionInput, _priorityInput);
        Feedback = string.Empty;
        NotifyFormProperties();
        if (CancelEditCommand is RelayCommand cancelCommand)
        {
            cancelCommand.NotifyCanExecuteChanged();
        }
    }

    /* funkcja: CancelEdit
     opis: Zamyka edycję i czyści formularz bez zmiany zgłoszenia.
     parametry: brak
     zwraca: void, nic nie zwraca */
    public void CancelEdit()
    {
        if (IsEditing)
        {
            ResetForm();
            Feedback = string.Empty;
        }
    }

    private void Delete(Ticket ticket)
    {
        _commandBus.Execute(new DeleteTicketCommand(ticket.Id));
        var row = Tickets.FirstOrDefault(item => item.Ticket.Id == ticket.Id);
        if (row is not null)
        {
            Tickets.Remove(row);
        }

        if (_editingTicketId == ticket.Id)
        {
            ResetForm();
        }
    }

    private void ChangeStatus(Guid ticketId, string status)
    {
        _commandBus.Execute(new ChangeTicketStatusCommand(ticketId, status));
    }

    private TicketRowViewModel CreateRow(Ticket ticket) => new(ticket, ChangeStatus);

    private void RunTests()
    {
        var result = _testRunner.RunAll();
        TestReport = $"{result.Passed}/{result.Total} testów MSTest zaliczonych · {result.Failed} błędów" + Environment.NewLine + string.Join(Environment.NewLine, result.Details);
    }

    private void UpdateValidation()
    {
        _validation = _validator.Validate(_nameInput, _descriptionInput, _priorityInput);
        NotifyValidationProperties();
    }

    private void NotifyValidationProperties()
    {
        OnPropertyChanged(nameof(NameError));
        OnPropertyChanged(nameof(DescriptionError));
        OnPropertyChanged(nameof(PriorityError));
        OnPropertyChanged(nameof(ShowNameError));
        OnPropertyChanged(nameof(ShowDescriptionError));
        OnPropertyChanged(nameof(ShowPriorityError));
    }

    private void NotifyFormProperties()
    {
        OnPropertyChanged(nameof(NameInput));
        OnPropertyChanged(nameof(DescriptionInput));
        OnPropertyChanged(nameof(PriorityInput));
        OnPropertyChanged(nameof(IsEditing));
        OnPropertyChanged(nameof(FormTitle));
        OnPropertyChanged(nameof(SaveButtonText));
        NotifyValidationProperties();
    }

    private void ResetForm()
    {
        _editingTicketId = null;
        _nameInput = string.Empty;
        _descriptionInput = string.Empty;
        _priorityInput = string.Empty;
        _nameTouched = false;
        _descriptionTouched = false;
        _priorityTouched = false;
        _hasSubmitted = false;
        _validation = _validator.Validate(_nameInput, _descriptionInput, _priorityInput);
        NotifyFormProperties();
        if (CancelEditCommand is RelayCommand cancelCommand)
        {
            cancelCommand.NotifyCanExecuteChanged();
        }
    }

    private void ClearFeedback()
    {
        if (!string.IsNullOrEmpty(Feedback))
        {
            Feedback = string.Empty;
        }
    }

    private void OnTicketsChanged(object? sender, NotifyCollectionChangedEventArgs args)
    {
        if (args.OldItems is not null)
        {
            foreach (TicketRowViewModel row in args.OldItems)
            {
                row.PropertyChanged -= OnTicketRowChanged;
            }
        }

        if (args.NewItems is not null)
        {
            foreach (TicketRowViewModel row in args.NewItems)
            {
                row.PropertyChanged += OnTicketRowChanged;
            }
        }

        RefreshCounts();
    }

    private void OnTicketRowChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName is null or nameof(TicketRowViewModel.Status))
        {
            RefreshCounts();
        }
    }

    private void RefreshCounts()
    {
        TotalCount = Tickets.Count;
        OpenCount = Tickets.Count(ticket => ticket.Status != "Zamknięte");
    }
}
