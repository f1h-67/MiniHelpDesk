using MiniHelpDesk.Tests;
using MiniHelpDesk.ViewModels;

namespace MiniHelpDesk.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private void OnRunTestsClicked(object sender, EventArgs e)
        {
            string results = TestRunner.RunAllTests();
            TestResultsLabel.Text = results;
        }
    }
}