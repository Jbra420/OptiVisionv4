using OptivisionApp.ViewModels;

namespace OptivisionApp.Views;

public partial class vARIntro : ContentPage
{
    public vARIntro(ARIntroViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
