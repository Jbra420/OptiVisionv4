using OptivisionApp.ViewModels;

namespace OptivisionApp.Views;

public partial class vProfile : ContentPage
{
    private readonly ProfileViewModel _viewModel;

    public vProfile(ProfileViewModel vm)
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
