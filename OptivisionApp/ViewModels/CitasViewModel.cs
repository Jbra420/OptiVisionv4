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
    public class CitasViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        
        private ObservableCollection<Cita> _citas = new();
        private DateTime _fechaSeleccionada = DateTime.Now.AddDays(1);
        private TimeSpan _horaSeleccionada = new TimeSpan(10, 0, 0); // 10:00 AM por defecto
        private string _opticaSeleccionada = "Sede Norte (Centro Médico)";
        private string _motivoCita = "Examen de agudeza visual completo";
        private string _notasCita = string.Empty;
        private string _mensajeResultado = string.Empty;

        public CitasViewModel(IApiService apiService)
        {
            _apiService = apiService;
            Title = "Mis Citas Ópticas";
            
            Citas = new ObservableCollection<Cita>();

            CargarCitasCommand = new Command(async () => await ExecuteCargarCitasCommand());
            AgendarCitaCommand = new Command(async () => await ExecuteAgendarCitaCommand());
            CancelarCitaCommand = new Command<Cita>(async (cita) => await ExecuteCancelarCitaCommand(cita));
        }

        public ObservableCollection<Cita> Citas
        {
            get => _citas;
            set => SetProperty(ref _citas, value);
        }

        public DateTime FechaSeleccionada
        {
            get => _fechaSeleccionada;
            set => SetProperty(ref _fechaSeleccionada, value);
        }

        public TimeSpan HoraSeleccionada
        {
            get => _horaSeleccionada;
            set => SetProperty(ref _horaSeleccionada, value);
        }

        public string OpticaSeleccionada
        {
            get => _opticaSeleccionada;
            set => SetProperty(ref _opticaSeleccionada, value);
        }

        public string MotivoCita
        {
            get => _motivoCita;
            set => SetProperty(ref _motivoCita, value);
        }

        public string NotasCita
        {
            get => _notasCita;
            set => SetProperty(ref _notasCita, value);
        }

        public string MensajeResultado
        {
            get => _mensajeResultado;
            set 
            {
                SetProperty(ref _mensajeResultado, value);
                OnPropertyChanged(nameof(HasMensajeResultado));
            }
        }

        public bool HasMensajeResultado => !string.IsNullOrWhiteSpace(MensajeResultado);
        public DateTime MinDate => DateTime.Now.Date;

        public ICommand CargarCitasCommand { get; }
        public ICommand AgendarCitaCommand { get; }
        public ICommand CancelarCitaCommand { get; }

        private async Task ExecuteCargarCitasCommand()
        {
            if (IsBusy) return;
            IsBusy = true;
            MensajeResultado = string.Empty;

            try
            {
                Citas.Clear();
                int usuarioId = App.UsuarioActual?.Id ?? 1; // Fallback al id 1 si no ha iniciado sesión
                var lista = await _apiService.GetCitasUsuarioAsync(usuarioId);
                
                foreach (var item in lista)
                {
                    Citas.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cargar citas: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteAgendarCitaCommand()
        {
            if (IsBusy) return;

            IsBusy = true;
            MensajeResultado = string.Empty;

            try
            {
                // Combinar fecha y hora
                var fechaCompleta = FechaSeleccionada.Date + HoraSeleccionada;

                var nuevaCita = new Cita
                {
                    UsuarioId = App.UsuarioActual?.Id ?? 1,
                    FechaCita = fechaCompleta,
                    Optica = OpticaSeleccionada,
                    Motivo = MotivoCita,
                    Notas = NotasCita,
                    Estado = "Pendiente"
                };

                var exito = await _apiService.CrearCitaAsync(nuevaCita);
                if (exito)
                {
                    MensajeResultado = "¡Cita agendada correctamente!";
                    NotasCita = string.Empty;
                    // Recargar lista
                    await ExecuteCargarCitasCommand();
                }
                else
                {
                    MensajeResultado = "Error al agendar la cita en el servidor.";
                }
            }
            catch (Exception ex)
            {
                MensajeResultado = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteCancelarCitaCommand(Cita cita)
        {
            if (cita == null || IsBusy) return;

            bool confirmar = await Shell.Current.DisplayAlert(
                "Confirmar", 
                "¿Estás seguro de que deseas cancelar esta cita?", 
                "Sí", 
                "No");

            if (!confirmar) return;

            IsBusy = true;
            try
            {
                var exito = await _apiService.CancelarCitaAsync(cita.Id);
                if (exito)
                {
                    await ExecuteCargarCitasCommand();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo cancelar la cita.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al cancelar cita: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
