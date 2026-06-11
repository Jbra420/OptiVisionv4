using OptivisionApp.ViewModels;

namespace OptivisionApp.Views;

public partial class vLogin : ContentPage
{
    public vLogin(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
