using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using OptivisionApp.Models;
using OptivisionApp.Services;

namespace OptivisionApp.ViewModels
{
    public class CatalogoViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly IDatabaseService _databaseService;
        
        private ObservableCollection<MarcoLente> _lentes = new();
        private MarcoLente? _lenteSeleccionado;
        private string _categoriaFiltro = "Todos";
        private bool _isArActive;
        
        private ImageSource? _selfieSource;
        private double _lenteOffsetX = 0;
        private double _lenteOffsetY = -50;
        private double _lenteScale = 1.0;
        private bool _hasPhoto = false;

        // Propiedades de Tintes
        private string _selectedTintName = "NINGUNO";
        private bool _isSapphireSelected;
        private bool _isRubySelected;
        private bool _isEmeraldSelected;
        private bool _isAmberSelected;

        public CatalogoViewModel(IApiService apiService, IDatabaseService databaseService)
        {
            _apiService = apiService;
            _databaseService = databaseService;
            Title = "Probador Virtual AR";

            Lentes = new ObservableCollection<MarcoLente>();

            CargarLentesCommand = new Command(async () => await ExecuteCargarLentesCommand());
            ProbarLenteCommand = new Command<MarcoLente>(ExecuteProbarLenteCommand);
            DetenerPruebaCommand = new Command(ExecuteDetenerPruebaCommand);
            ConfirmarLenteCommand = new Command(async () => await ExecuteConfirmarLenteCommand());
            FiltrarCategoriaCommand = new Command<string>(async (cat) => await ExecuteFiltrarCategoriaCommand(cat));
            TomarSelfieCommand = new Command(async () => await ExecuteTomarSelfieCommand());

            // Comandos de Tinte
            SelectTintSapphireCommand = new Command(() => SelectTint("ZAFIRO AZUL"));
            SelectTintRubyCommand = new Command(() => SelectTint("RUBÍ INTENSO"));
            SelectTintEmeraldCommand = new Command(() => SelectTint("ESMERALDA"));
            SelectTintAmberCommand = new Command(() => SelectTint("ÁMBAR DORADO"));
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

        public ImageSource? SelfieSource
        {
            get => _selfieSource;
            set => SetProperty(ref _selfieSource, value);
        }

        public double LenteOffsetX
        {
            get => _lenteOffsetX;
            set => SetProperty(ref _lenteOffsetX, value);
        }

        public double LenteOffsetY
        {
            get => _lenteOffsetY;
            set => SetProperty(ref _lenteOffsetY, value);
        }

        public double LenteScale
        {
            get => _lenteScale;
            set => SetProperty(ref _lenteScale, value);
        }

        public bool HasPhoto
        {
            get => _hasPhoto;
            set => SetProperty(ref _hasPhoto, value);
        }

        public string SelectedTintName
        {
            get => _selectedTintName;
            set => SetProperty(ref _selectedTintName, value);
        }

        public bool IsSapphireSelected
        {
            get => _isSapphireSelected;
            set => SetProperty(ref _isSapphireSelected, value);
        }

        public bool IsRubySelected
        {
            get => _isRubySelected;
            set => SetProperty(ref _isRubySelected, value);
        }

        public bool IsEmeraldSelected
        {
            get => _isEmeraldSelected;
            set => SetProperty(ref _isEmeraldSelected, value);
        }

        public bool IsAmberSelected
        {
            get => _isAmberSelected;
            set => SetProperty(ref _isAmberSelected, value);
        }

        public ICommand CargarLentesCommand { get; }
        public ICommand ProbarLenteCommand { get; }
        public ICommand DetenerPruebaCommand { get; }
        public ICommand ConfirmarLenteCommand { get; }
        public ICommand FiltrarCategoriaCommand { get; }
        public ICommand TomarSelfieCommand { get; }
        public ICommand SelectTintSapphireCommand { get; }
        public ICommand SelectTintRubyCommand { get; }
        public ICommand SelectTintEmeraldCommand { get; }
        public ICommand SelectTintAmberCommand { get; }

        private async Task ExecuteCargarLentesCommand()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                Lentes.Clear();
                var catParam = CategoriaFiltro == "Todos" ? null : CategoriaFiltro;
                
                var lista = await _apiService.GetLentesAsync(catParam);
                
                if (lista == null || lista.Count == 0)
                {
                    lista = await _databaseService.GetMarcosAsync();
                    
                    if (!string.IsNullOrEmpty(catParam))
                    {
                        lista = lista.Where(l => l.Categoria == catParam).ToList();
                    }
                }
                
                foreach (var item in lista)
                {
                    await _databaseService.SaveMarcoAsync(item);
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

        private void ExecuteProbarLenteCommand(MarcoLente lente)
        {
            if (lente == null) return;

            LenteSeleccionado = lente;
            IsArActive = true;
            
            // Valores por defecto al probar un nuevo lente
            LenteOffsetX = 0;
            LenteOffsetY = -50;
            LenteScale = 1.0;
        }

        private async Task ExecuteTomarSelfieCommand()
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        var stream = await photo.OpenReadAsync();
                        SelfieSource = ImageSource.FromStream(() => stream);
                        HasPhoto = true;
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "La cámara no está soportada en este dispositivo", "OK");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Ocurrió un error al tomar la foto: {ex.Message}", "OK");
            }
        }

        private void ExecuteDetenerPruebaCommand()
        {
            IsArActive = false;
            LenteSeleccionado = null;
        }

        private async Task ExecuteConfirmarLenteCommand()
        {
            if (LenteSeleccionado == null) return;
            
            await _databaseService.SaveMarcoAsync(LenteSeleccionado);
            
            await Shell.Current.DisplayAlert("¡Lente Confirmado!", $"Has seleccionado el marco {LenteSeleccionado.Nombre} con el tinte {SelectedTintName}. Se ha guardado tu elección.", "Aceptar");
            
            ExecuteDetenerPruebaCommand();
        }

        private async Task ExecuteFiltrarCategoriaCommand(string categoria)
        {
            CategoriaFiltro = categoria;
            await ExecuteCargarLentesCommand();
        }

        private void SelectTint(string tintName)
        {
            SelectedTintName = tintName;
            IsSapphireSelected = tintName == "ZAFIRO AZUL";
            IsRubySelected = tintName == "RUBÍ INTENSO";
            IsEmeraldSelected = tintName == "ESMERALDA";
            IsAmberSelected = tintName == "ÁMBAR DORADO";
            
            // Simular un sutil cambio de efecto en el lente
            // En un app real, aquí podríamos cambiar el tint/color del PNG o aplicar un Shader
        }
    }
}
