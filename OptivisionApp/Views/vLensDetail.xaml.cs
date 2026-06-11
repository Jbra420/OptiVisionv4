using OptivisionApp.ViewModels;

namespace OptivisionApp.Views;

public partial class vLensDetail : ContentPage
{
    public vLensDetail(LensDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
