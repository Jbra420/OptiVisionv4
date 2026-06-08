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
        private readonly IDatabaseService _databaseService;
        
        private ObservableCollection<Cita> _citas = new();
        private DateTime _fechaSeleccionada = DateTime.Now.AddDays(1);
        private TimeSpan _horaSeleccionada = new TimeSpan(10, 0, 0); // 10:00 AM por defecto
        private string _opticaSeleccionada = "Sede Norte (Centro Médico)";
        private string _motivoCita = "Examen de agudeza visual completo";
        private string _notasCita = string.Empty;
        private string _mensajeResultado = string.Empty;

        public CitasViewModel(IApiService apiService, IDatabaseService databaseService)
        {
            _apiService = apiService;
            _databaseService = databaseService;
            Title = "Mis Citas Ópticas";
            
            Citas = new ObservableCollection<Cita>();

            CargarCitasCommand = new Command(async () => await ExecuteCargarCitasCommand());
            AgendarCitaCommand = new Command(async () => await ExecuteAgendarCitaCommand());
            CancelarCitaCommand = new Command<Cita>(async (cita) => await ExecuteCancelarCitaCommand(cita));
            ReprogramarCitaCommand = new Command<Cita>(async (cita) => await ExecuteReprogramarCitaCommand(cita));
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
        public ICommand ReprogramarCitaCommand { get; }

        private async Task ExecuteCargarCitasCommand()
        {
            if (IsBusy) return;
            IsBusy = true;
            MensajeResultado = string.Empty;

            try
            {
                Citas.Clear();
                int usuarioId = App.UsuarioActual?.Id ?? 1; // Fallback al id 1 si no ha iniciado sesión
                // Intentar cargar desde API
                var lista = await _apiService.GetCitasUsuarioAsync(usuarioId);
                
                // Si la API falla o devuelve vacío, intentar local
                if (lista == null || lista.Count == 0)
                {
                    lista = await _databaseService.GetCitasAsync();
                }

                foreach (var item in lista)
                {
                    // Guardar/Actualizar localmente para uso offline
                    await _databaseService.SaveCitaAsync(item);
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
                
                // Guardar localmente también
                await _databaseService.SaveCitaAsync(nuevaCita);

                if (exito)
                {
                    MensajeResultado = "¡Cita agendada correctamente!";
                    NotasCita = string.Empty;
                    // Recargar lista
                    await ExecuteCargarCitasCommand();
                }
                else
                {
                    MensajeResultado = "Cita agendada localmente. (Error en servidor)";
                    await ExecuteCargarCitasCommand();
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

        private async Task ExecuteReprogramarCitaCommand(Cita cita)
        {
            if (cita == null || IsBusy) return;

            // Mostrar date picker personalizado (simulado aquí pidiendo confirmación simple
            // o idealmente una UI de selección, pero lo haremos sumando 1 día para el prototipo)
            bool reprogramar = await Shell.Current.DisplayAlert(
                "Reprogramar", 
                $"¿Deseas reprogramar la cita para el día de mañana a la misma hora?", 
                "Sí", 
                "No");

            if (!reprogramar) return;

            IsBusy = true;
            try
            {
                var nuevaFecha = cita.FechaCita.AddDays(1);
                var exito = await _apiService.ReprogramarCitaAsync(cita.Id, nuevaFecha);
                
                if (exito)
                {
                    cita.FechaCita = nuevaFecha;
                    cita.Estado = "Pendiente";
                    await _databaseService.SaveCitaAsync(cita); // Actualizar local
                    await ExecuteCargarCitasCommand();
                    await Shell.Current.DisplayAlert("Éxito", "Cita reprogramada para mañana.", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "No se pudo reprogramar la cita en el servidor.", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error al reprogramar cita: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
