using OptivisionApp.ViewModels;

namespace OptivisionApp.Views;

public partial class vRegister : ContentPage
{
    public vRegister(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
