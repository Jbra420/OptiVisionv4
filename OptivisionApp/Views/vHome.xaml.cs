using OptivisionApp.ViewModels;

namespace OptivisionApp.Views;

public partial class vHome : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public vHome(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel != null)
        {
            await _viewModel.RefreshAsync();
        }
    }
}
