using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using OptivisionApp.Models;
using OptivisionApp.Services;

namespace OptivisionApp.ViewModels
{
    public class CatalogoViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly IArService _arService;
        
        private ObservableCollection<MarcoLente> _lentes = new();
        private MarcoLente? _lenteSeleccionado;
        private string _categoriaFiltro = "Todos";
        private bool _isArActive;
        private string _arStatusMessage = string.Empty;

        public CatalogoViewModel(IApiService apiService, IArService arService)
        {
            _apiService = apiService;
            _arService = arService;
            Title = "Probador Virtual AR";

            Lentes = new ObservableCollection<MarcoLente>();

            CargarLentesCommand = new Command(async () => await ExecuteCargarLentesCommand());
            ProbarLenteCommand = new Command<MarcoLente>(async (lente) => await ExecuteProbarLenteCommand(lente));
            DetenerPruebaCommand = new Command(ExecuteDetenerPruebaCommand);
            FiltrarCategoriaCommand = new Command<string>(async (cat) => await ExecuteFiltrarCategoriaCommand(cat));
        }

        public ObservableCollection<MarcoLente> Lentes
        {
            get => _lentes;
            set => SetProperty(ref _lentes, value);
        }

        public MarcoLente? LenteSeleccionado
        {
            get => _lenteSeleccionado;
            set => SetProperty(ref _lenteSeleccionado, value);
        }

        public string CategoriaFiltro
        {
            get => _categoriaFiltro;
            set => SetProperty(ref _categoriaFiltro, value);
        }

        public bool IsArActive
        {
            get => _isArActive;
            set => SetProperty(ref _isArActive, value);
        }

        public string ArStatusMessage
        {
            get => _arStatusMessage;
            set => SetProperty(ref _arStatusMessage, value);
        }

        public ICommand CargarLentesCommand { get; }
        public ICommand ProbarLenteCommand { get; }
        public ICommand DetenerPruebaCommand { get; }
        public ICommand FiltrarCategoriaCommand { get; }

        private async Task ExecuteCargarLentesCommand()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Lentes.Clear();
                var catParam = CategoriaFiltro == "Todos" ? null : CategoriaFiltro;
                var lista = await _apiService.GetLentesAsync(catParam);
                
                foreach (var item in lista)
                {
                    Lentes.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar catálogo: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteProbarLenteCommand(MarcoLente lente)
        {
            if (lente == null) return;

            LenteSeleccionado = lente;
            IsArActive = true;
            ArStatusMessage = $"Iniciando cámara AR para: {lente.Nombre}...";

            try
            {
                await _arService.InicializarCamaraAsync();
                ArStatusMessage = "Seguimiento facial activo (Face Tracking OK). Moviendo rostro...";
                await Task.Delay(800); // Dar un delay agradable para visualización de estados en la UI
                
                await _arService.AplicarFiltroLentesAsync(lente.Id);
                ArStatusMessage = $"Probando virtualmente: {lente.Nombre}";
            }
            catch (Exception ex)
            {
                ArStatusMessage = $"Error al iniciar AR: {ex.Message}";
            }
        }

        private void ExecuteDetenerPruebaCommand()
        {
            _arService.DetenerPruebaVirtual();
            IsArActive = false;
            LenteSeleccionado = null;
            ArStatusMessage = string.Empty;
        }

        private async Task ExecuteFiltrarCategoriaCommand(string categoria)
        {
            CategoriaFiltro = categoria;
            await ExecuteCargarLentesCommand();
        }
    }
}
