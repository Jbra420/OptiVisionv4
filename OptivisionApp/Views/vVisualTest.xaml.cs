using OptivisionApp.ViewModels;

namespace OptivisionApp.Views;

public partial class vVisualTest : ContentPage
{
    public vVisualTest(VisualTestViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
