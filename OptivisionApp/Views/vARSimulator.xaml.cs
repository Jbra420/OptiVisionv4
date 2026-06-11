using OptivisionApp.ViewModels;

namespace OptivisionApp.Views;

public partial class vARSimulator : ContentPage
{
    public vARSimulator(ARSimulatorViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
