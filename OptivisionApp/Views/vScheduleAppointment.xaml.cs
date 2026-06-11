using OptivisionApp.ViewModels;

namespace OptivisionApp.Views;

public partial class vScheduleAppointment : ContentPage
{
    private readonly AppointmentsViewModel _viewModel;

    public vScheduleAppointment(AppointmentsViewModel vm)
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
