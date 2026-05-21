using Microsoft.Maui.Controls;
using OptivisionApp.ViewModels;

namespace OptivisionApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
