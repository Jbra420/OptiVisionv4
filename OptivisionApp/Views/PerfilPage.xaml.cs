using Microsoft.Maui.Controls;
using OptivisionApp.ViewModels;

namespace OptivisionApp.Views
{
    public partial class PerfilPage : ContentPage
    {
        public PerfilPage(PerfilViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
