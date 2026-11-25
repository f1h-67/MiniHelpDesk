using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk;

/* klasa: MainPage
 opis: Wyświetla responsywny formularz i listę zgłoszeń powiązane z ViewModelem.
 pola / właściwości: brak */
public partial class MainPage : ContentPage
{
    /* funkcja: MainPage
     opis: Tworzy stronę główną i przypisuje jej ViewModel.
     parametry: viewModel, HelpDeskViewModel, logika i stan widoku
     zwraca: void, nic nie zwraca */
    public MainPage(HelpDeskViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
