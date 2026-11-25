namespace MiniHelpDesk;

/* klasa: App
 opis: Konfiguruje okno aplikacji i wyświetla główną stronę Mini HelpDesk.
 pola / właściwości: _mainPage, MainPage, strona główna aplikacji */
public partial class App : Microsoft.Maui.Controls.Application
{
    private readonly MainPage _mainPage;

    /* funkcja: App
     opis: Tworzy aplikację z przygotowaną stroną główną.
     parametry: mainPage, MainPage, strona interfejsu Mini HelpDesk
     zwraca: void, nic nie zwraca */
    public App(MainPage mainPage)
    {
        InitializeComponent();
        _mainPage = mainPage;
    }

    /* funkcja: CreateWindow
     opis: Tworzy główne okno aplikacji.
     parametry: activationState, IActivationState?, stan aktywacji aplikacji
     zwraca: Window, okno zawierające stronę główną */
    protected override Window CreateWindow(IActivationState? activationState) => new(_mainPage);
}
