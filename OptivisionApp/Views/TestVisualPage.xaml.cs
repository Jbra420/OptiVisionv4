using Microsoft.Maui.Controls;
using OptivisionApp.ViewModels;

namespace OptivisionApp.Views
{
    public partial class TestVisualPage : ContentPage
    {
        public TestVisualPage(TestVisualViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
