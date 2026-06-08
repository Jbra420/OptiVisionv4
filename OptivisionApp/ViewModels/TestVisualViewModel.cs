using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using OptivisionApp.Models;
using OptivisionApp.Services;

namespace OptivisionApp.ViewModels
{
    public class TestVisualViewModel : BaseViewModel
    {
        private readonly IDatabaseService _databaseService;

        private int _nivelActual = 1;
        private int _aciertos = 0;
        private string _letraActual = string.Empty;
        private double _tamanoLetra = 100;
        private bool _testCompletado = false;
        private bool _mostrandoDisclaimer = true;
        private string _mensajeResultado = string.Empty;
        private string _recomendacion = string.Empty;

        // Letras Snellen comunes
        private readonly string[] _letras = { "E", "F", "P", "T", "O", "Z", "L", "D" };
        private readonly Random _random = new Random();

        public TestVisualViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
            Title = "Test Visual Preventivo";

            IniciarTestCommand = new Command(IniciarTest);
            ResponderCommand = new Command<string>(Responder);
            AgendarCitaCommand = new Command(async () => await Shell.Current.GoToAsync("///CitasPage"));
        }

        public bool MostrandoDisclaimer
        {
            get => _mostrandoDisclaimer;
            set => SetProperty(ref _mostrandoDisclaimer, value);
        }

        public bool TestEnCurso => !MostrandoDisclaimer && !TestCompletado;

        public bool TestCompletado
        {
            get => _testCompletado;
            set 
            {
                SetProperty(ref _testCompletado, value);
                OnPropertyChanged(nameof(TestEnCurso));
            }
        }

        public string LetraActual
        {
            get => _letraActual;
            set => SetProperty(ref _letraActual, value);
        }

        public double TamanoLetra
        {
            get => _tamanoLetra;
            set => SetProperty(ref _tamanoLetra, value);
        }

        public string Progreso => $"Nivel {_nivelActual} de 5";

        public string MensajeResultado
        {
            get => _mensajeResultado;
            set => SetProperty(ref _mensajeResultado, value);
        }

        public string Recomendacion
        {
            get => _recomendacion;
            set => SetProperty(ref _recomendacion, value);
        }

        public ICommand IniciarTestCommand { get; }
        public ICommand ResponderCommand { get; }
        public ICommand AgendarCitaCommand { get; }

        private void IniciarTest()
        {
            MostrandoDisclaimer = false;
            TestCompletado = false;
            _nivelActual = 1;
            _aciertos = 0;
            GenerarNuevaLetra();
        }

        private void GenerarNuevaLetra()
        {
            LetraActual = _letras[_random.Next(_letras.Length)];
            
            // Reducir tamaño progresivamente (de nivel 1 a 5)
            // Nivel 1: 120, Nivel 2: 90, Nivel 3: 60, Nivel 4: 30, Nivel 5: 15
            TamanoLetra = 150 - (_nivelActual * 28); 
            
            OnPropertyChanged(nameof(Progreso));
            OnPropertyChanged(nameof(TestEnCurso));
        }

        private async void Responder(string respuesta)
        {
            if (respuesta == LetraActual)
            {
                _aciertos++;
            }

            if (_nivelActual < 5)
            {
                _nivelActual++;
                GenerarNuevaLetra();
            }
            else
            {
                await FinalizarTestAsync();
            }
        }

        private async Task FinalizarTestAsync()
        {
            TestCompletado = true;
            OnPropertyChanged(nameof(TestEnCurso));

            int puntajeFinal = _aciertos * 20; // 0 a 100
            string nivel;

            if (puntajeFinal >= 80)
            {
                nivel = "Óptima";
                Recomendacion = "Tu agudeza visual parece estar muy bien. Mantén chequeos preventivos anuales.";
            }
            else if (puntajeFinal >= 40)
            {
                nivel = "Regular";
                Recomendacion = "Tienes cierta dificultad para leer letras pequeñas. Te sugerimos agendar una cita de control.";
            }
            else
            {
                nivel = "Baja";
                Recomendacion = "Detectamos dificultad significativa. Es altamente recomendable que reserves una cita pronto para un examen profesional.";
            }

            MensajeResultado = $"Puntaje: {puntajeFinal}/100\nNivel: {nivel}";

            // Guardar localmente
            if (App.UsuarioActual != null)
            {
                var resultado = new ResultadoTest
                {
                    UsuarioId = App.UsuarioActual.Id,
                    Puntaje = puntajeFinal,
                    NivelAgudeza = nivel,
                    Recomendacion = Recomendacion,
                    FechaTest = DateTime.Now
                };
                await _databaseService.SaveResultadoTestAsync(resultado);
            }
        }
    }
}
