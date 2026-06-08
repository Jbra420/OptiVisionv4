using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OptivisionApp.Models;

namespace OptivisionApp.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public ApiService()
        {
            // Detectar automáticamente la URL del servidor local según el sistema operativo
            // para facilitar pruebas en emuladores y dispositivos físicos.
            #if ANDROID
            _baseUrl = "http://10.0.2.2:5001/api/";
            #else
            _baseUrl = "http://localhost:5001/api/";
            #endif

            // Configurar HttpClient con soporte para ignorar problemas de certificados autofirmados (desarrollo)
            var handler = GetPlatformMessageHandler();
            _httpClient = handler != null ? new HttpClient(handler) : new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<Usuario?> LoginAsync(string email, string password)
        {
            try
            {
                var loginData = new { Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync("auth/login", loginData);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    if (responseData != null)
                    {
                        return new Usuario
                        {
                            Id = responseData.Id,
                            Nombre = responseData.Nombre,
                            Email = responseData.Email,
                            Rol = responseData.Rol,
                            Receta = responseData.Receta
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en LoginAsync: {ex.Message}");
            }
            return null;
        }

        public async Task<bool> RegisterAsync(string nombre, string email, string password)
        {
            try
            {
                var regData = new { Nombre = nombre, Email = email, Password = password };
                var response = await _httpClient.PostAsJsonAsync("auth/register", regData);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en RegisterAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<List<MarcoLente>> GetLentesAsync(string? categoria = null)
        {
            try
            {
                string url = "lentes";
                if (!string.IsNullOrEmpty(categoria))
                {
                    url += $"?categoria={Uri.EscapeDataString(categoria)}";
                }

                var response = await _httpClient.GetFromJsonAsync<List<MarcoLente>>(url);
                return response ?? new List<MarcoLente>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetLentesAsync: {ex.Message}");
                return GetFallbackLentes(); // Datos de respaldo locales por si no hay conexión al API
            }
        }

        public async Task<List<Cita>> GetCitasUsuarioAsync(int usuarioId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<Cita>>($"citas/usuario/{usuarioId}");
                return response ?? new List<Cita>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetCitasUsuarioAsync: {ex.Message}");
                return GetFallbackCitas(usuarioId); // Datos de respaldo locales
            }
        }

        public async Task<bool> CrearCitaAsync(Cita cita)
        {
            try
            {
                var model = new
                {
                    UsuarioId = cita.UsuarioId,
                    FechaCita = cita.FechaCita,
                    Optica = cita.Optica,
                    Motivo = cita.Motivo,
                    Notas = cita.Notas
                };
                var response = await _httpClient.PostAsJsonAsync("citas", model);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CrearCitaAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarRecetaAsync(int usuarioId, string recetaJson)
        {
            try
            {
                var model = new { RecetaJson = recetaJson };
                var response = await _httpClient.PutAsJsonAsync($"auth/perfil/{usuarioId}/receta", model);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarRecetaAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CancelarCitaAsync(int citaId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"citas/{citaId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CancelarCitaAsync: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ReprogramarCitaAsync(int citaId, DateTime nuevaFecha)
        {
            try
            {
                var model = new { NuevaFecha = nuevaFecha };
                var response = await _httpClient.PutAsJsonAsync($"citas/{citaId}/reprogramar", model);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ReprogramarCitaAsync: {ex.Message}");
                return false;
            }
        }

        // --- MÉTODOS DE RESPALDO Y UTILIDADES ---

        private HttpMessageHandler? GetPlatformMessageHandler()
        {
            #if ANDROID
            var handler = new Xamarin.Android.Net.AndroidMessageHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            return handler;
            #elif IOS
            var handler = new NSUrlSessionHandler();
            handler.TrustOverrideForUrl = (sender, url, trust) => true;
            return handler;
            #else
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            return handler;
            #endif
        }

        private List<MarcoLente> GetFallbackLentes()
        {
            return new List<MarcoLente>
            {
                new MarcoLente { Id = 1, Nombre = "Classic Wayfarer Black", Marca = "OptiStyle", Precio = 120.00m, ImagenUrl = "classic_wayfarer.jpg", TipoMarco = "Pasta", Categoria = "Unisex", Descripcion = "Respaldo: Diseño Wayfarer clásico." },
                new MarcoLente { Id = 2, Nombre = "Aviator Gold Metal", Marca = "AeroMax", Precio = 150.00m, ImagenUrl = "aviator_gold.jpg", TipoMarco = "Metal", Categoria = "Hombre", Descripcion = "Respaldo: Aviadores metálicos." },
                new MarcoLente { Id = 3, Nombre = "Sleek Round Tortoise", Marca = "RetroLook", Precio = 110.00m, ImagenUrl = "round_tortoise.jpg", TipoMarco = "Pasta", Categoria = "Mujer", Descripcion = "Respaldo: Diseño bohemio retro." }
            };
        }

        private List<Cita> GetFallbackCitas(int usuarioId)
        {
            return new List<Cita>
            {
                new Cita { Id = 1, UsuarioId = usuarioId, FechaCita = DateTime.Now.AddDays(3), Estado = "Confirmada", Optica = "Sede Norte (Local)", Motivo = "Examen de agudeza visual" }
            };
        }

        private class LoginResponseDto
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Rol { get; set; } = string.Empty;
            public string? Receta { get; set; }
            public string Token { get; set; } = string.Empty;
        }
    }
}
