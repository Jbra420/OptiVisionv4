using Microsoft.Maui.Controls;
using OptivisionApp.ViewModels;

namespace OptivisionApp.Views
{
    public partial class CitasPage : ContentPage
    {
        private readonly CitasViewModel _viewModel;

        public CitasPage(CitasViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Cargar automáticamente las citas del usuario al mostrar la página
            _viewModel.CargarCitasCommand.Execute(null);
        }
    }
}
