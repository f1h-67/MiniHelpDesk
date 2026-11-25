using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MiniHelpDesk.ViewModels;

/* klasa: ObservableObject
 opis: Udostępnia powiadomienia o zmianie właściwości dla ViewModeli.
 pola / właściwości: PropertyChanged, PropertyChangedEventHandler?, zdarzenie informujące widok o zmianach */
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    /* funkcja: SetProperty
     opis: Zmienia wartość pola i zgłasza powiadomienie, gdy wartość jest inna.
     parametry: storage, T, pole przechowujące wartość; value, T, nowa wartość; propertyName, string, nazwa powiązanej właściwości
     zwraca: bool, true jeśli wartość została zmieniona */
    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
        {
            return false;
        }

        storage = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }

    /* funkcja: OnPropertyChanged
     opis: Powiadamia widok o zmianie wskazanej właściwości.
     parametry: propertyName, string?, nazwa zmienionej właściwości
     zwraca: void, nic nie zwraca */
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
