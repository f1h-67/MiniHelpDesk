using System.Windows.Input;

namespace MiniHelpDesk.ViewModels;

/* klasa: RelayCommand
 opis: Łączy przycisk widoku z metodą ViewModelu bez umieszczania logiki w code-behind.
 pola / właściwości: _execute, Action<object?>, wykonywana operacja; _canExecute, Predicate<object?>?, warunek dostępności komendy */
public sealed class RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null) : ICommand
{
    private readonly Action<object?> _execute = execute;
    private readonly Predicate<object?>? _canExecute = canExecute;

    public event EventHandler? CanExecuteChanged;

    /* funkcja: CanExecute
     opis: Określa, czy komenda może zostać wykonana dla podanego parametru.
     parametry: parameter, object?, parametr komendy
     zwraca: bool, informacja czy wykonanie komendy jest dozwolone */
    public bool CanExecute(object? parameter) => _canExecute?.Invoke(parameter) ?? true;

    /* funkcja: Execute
     opis: Uruchamia metodę przypisaną do komendy.
     parametry: parameter, object?, parametr komendy
     zwraca: void, nic nie zwraca */
    public void Execute(object? parameter) => _execute(parameter);

    /* funkcja: NotifyCanExecuteChanged
     opis: Powiadamia interfejs, że warunek dostępności komendy mógł się zmienić.
     parametry: brak
     zwraca: void, nic nie zwraca */
    public void NotifyCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
