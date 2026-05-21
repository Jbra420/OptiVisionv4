using Microsoft.Maui.Controls;
using OptivisionApp.ViewModels;

namespace OptivisionApp.Views
{
    public partial class CatalogoPage : ContentPage
    {
        private readonly CatalogoViewModel _viewModel;

        public CatalogoPage(CatalogoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Cargar automáticamente los lentes del catálogo al ingresar a la página
            if (_viewModel.Lentes.Count == 0)
            {
                _viewModel.CargarLentesCommand.Execute(null);
            }
        }
    }
}
