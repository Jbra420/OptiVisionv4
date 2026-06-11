using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using OptivisionApp.Models;
using OptivisionApp.Services;

namespace OptivisionApp.ViewModels
{
    public class PerfilViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;

        private string _nombreCompleto = string.Empty;
        private string _email = string.Empty;
        private bool _tieneResultados = false;

        public ObservableCollection<ResultadoTest> HistorialTest { get; } = new();

        public PerfilViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Mi Perfil";

            CerrarSesionCommand = new Command(async () => await CerrarSesionAsync());
            CargarDatosCommand = new Command(async () => await CargarDatosAsync());
        }

        public string NombreCompleto
        {
            get => _nombreCompleto;
            set => SetProperty(ref _nombreCompleto, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public bool TieneResultados
        {
            get => _tieneResultados;
            set => SetProperty(ref _tieneResultados, value);
        }

        public ICommand CerrarSesionCommand { get; }
        public ICommand CargarDatosCommand { get; }

        public async Task CargarDatosAsync()
        {
            if (App.UsuarioActual != null)
            {
                NombreCompleto = App.UsuarioActual.Nombre;
                Email = App.UsuarioActual.Email;

                var resultados = await _databaseService.GetResultadosTestAsync(App.UsuarioActual.Id);
                HistorialTest.Clear();
                foreach (var res in resultados)
                {
                    HistorialTest.Add(res);
                }

                TieneResultados = HistorialTest.Count > 0;
            }
        }

        private async Task CerrarSesionAsync()
        {
            App.UsuarioActual = null;
            Preferences.Remove("UserId");
            Preferences.Remove("UserEmail");
            
            // Regresar al inicio reseteando el shell (que automáticamente detectará sesión nula y mostrará Login)
            Application.Current!.MainPage = new AppShell();
        }
    }
}
