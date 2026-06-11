using OptivisionApp.ViewModels;

namespace OptivisionApp.Views;

public partial class vAlerts : ContentPage
{
    public vAlerts(AlertsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
