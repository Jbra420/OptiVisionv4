using Microsoft.Maui.Controls;
using OptivisionApp.ViewModels;

namespace OptivisionApp.Views
{
    [QueryProperty(nameof(StartInRegisterMode), "register")]
    public partial class LoginPage : ContentPage
    {
        private readonly LoginViewModel _viewModel;

        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;
        }

        public string StartInRegisterMode
        {
            set
            {
                if (bool.TryParse(value, out bool register))
                {
                    _viewModel.IsRegisterMode = register;
                }
            }
        }
    }
}
