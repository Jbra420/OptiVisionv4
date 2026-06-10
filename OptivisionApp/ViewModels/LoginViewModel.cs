using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using OptivisionApp.Services;

namespace OptivisionApp.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly IApiService _apiService;
        private readonly IDatabaseService _databaseService;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _nombre = string.Empty;
        private bool _isRegisterMode;
        private string _errorMessage = string.Empty;

        public LoginViewModel(IApiService apiService, IDatabaseService databaseService)
        {
            _apiService = apiService;
            _databaseService = databaseService;
            Title = "Iniciar Sesión";
            
            SubmitCommand = new Command(async () => await ExecuteSubmitCommand());
            ToggleModeCommand = new Command(ExecuteToggleModeCommand);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Nombre
        {
            get => _nombre;
            set => SetProperty(ref _nombre, value);
        }

        public bool IsRegisterMode
        {
            get => _isRegisterMode;
            set 
            {
                SetProperty(ref _isRegisterMode, value);
                Title = _isRegisterMode ? "Crear Cuenta" : "Iniciar Sesión";
                OnPropertyChanged(nameof(SubmitButtonText));
                OnPropertyChanged(nameof(ToggleText));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set 
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

        public string SubmitButtonText => IsRegisterMode ? "REGISTRARSE" : "INICIAR SESIÓN";
        public string ToggleText => IsRegisterMode ? "¿Ya tienes cuenta? Inicia sesión" : "¿No tienes cuenta? Regístrate";

        public ICommand SubmitCommand { get; }
        public ICommand ToggleModeCommand { get; }

        private async Task ExecuteSubmitCommand()
        {
            if (IsRegisterMode)
            {
                await ExecuteRegisterCommand();
            }
            else
            {
                await ExecuteLoginCommand();
            }
        }

        private async Task ExecuteLoginCommand()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Por favor, completa todos los campos.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var usuario = await _apiService.LoginAsync(Email, Password);
                
                // Si la API falla, intentar login local (Modo Offline)
                if (usuario == null)
                {
                    usuario = await _databaseService.GetUsuarioAsync(Email, Password);
                }

                if (usuario != null)
                {
                    App.UsuarioActual = usuario;
                    // Guardar localmente para persistencia de sesión (Preferences)
                    Microsoft.Maui.Storage.Preferences.Set("UserId", usuario.Id);
                    Microsoft.Maui.Storage.Preferences.Set("UserEmail", usuario.Email);

                    // Guardar localmente para futuras sesiones offline
                    await _databaseService.SaveUsuarioAsync(usuario);
                    await Shell.Current.GoToAsync("//MainApp");
                }
                else
                {
                    ErrorMessage = "Correo o contraseña incorrectos.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al conectar: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExecuteRegisterCommand()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(Nombre) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Por favor, completa todos los campos.";
                return;
            }

            IsBusy = true;
            ErrorMessage = string.Empty;

            try
            {
                var success = await _apiService.RegisterAsync(Nombre, Email, Password);
                if (success)
                {
                    var usuario = await _apiService.LoginAsync(Email, Password);
                    if (usuario != null)
                    {
                        App.UsuarioActual = usuario;
                        
                        // Guardar persistencia
                        Microsoft.Maui.Storage.Preferences.Set("UserId", usuario.Id);
                        Microsoft.Maui.Storage.Preferences.Set("UserEmail", usuario.Email);

                        await Shell.Current.GoToAsync("//MainApp");
                    }
                    else
                    {
                        IsRegisterMode = false;
                        ErrorMessage = "Registro exitoso. Inicia sesión.";
                    }
                }
                else
                {
                    ErrorMessage = "Error en el registro. Correo ya existe o datos inválidos.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error de red: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ExecuteToggleModeCommand()
        {
            IsRegisterMode = !IsRegisterMode;
            ErrorMessage = string.Empty;
            Nombre = string.Empty;
        }
    }
}
