using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using OptivisionApp.Services;
using OptivisionApp.ViewModels;
using OptivisionApp.Views;

namespace OptivisionApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // 1. Registro de Servicios (Inyección de Dependencias como Singletons)
            builder.Services.AddSingleton<IApiService, ApiService>();
            builder.Services.AddSingleton<IArService, ArService>();
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();

            // 2. Registro de ViewModels (Transient)
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<CatalogoViewModel>();
            builder.Services.AddTransient<CitasViewModel>();

            // 3. Registro de Views/Pages (Transient)
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<CatalogoPage>();
            builder.Services.AddTransient<CitasPage>();

            return builder.Build();
        }
    }
}
