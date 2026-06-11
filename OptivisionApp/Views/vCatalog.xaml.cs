using OptivisionApp.ViewModels;

namespace OptivisionApp.Views;

public partial class vCatalog : ContentPage
{
    public vCatalog(CatalogoViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
        // Cargar lentes al inicio
        vm.CargarLentesCommand.Execute(null);
    }
}
