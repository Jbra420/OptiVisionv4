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
            builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
            builder.Services.AddSingleton<OptivisionApp.Services.MockDatabaseService>();

            // 2. Registro de ViewModels (Transient)
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<CatalogoViewModel>();
            builder.Services.AddTransient<CitasViewModel>();
            builder.Services.AddTransient<TestVisualViewModel>();
            builder.Services.AddTransient<PerfilViewModel>();
            builder.Services.AddTransient<HomeViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<AppointmentsViewModel>();
            builder.Services.AddTransient<VisualTestViewModel>();
            builder.Services.AddTransient<ARSimulatorViewModel>();
            builder.Services.AddTransient<LensDetailViewModel>();
            builder.Services.AddTransient<AlertsViewModel>();
            builder.Services.AddTransient<ARIntroViewModel>();

            // 3. Registro de Views/Pages (Transient)
            builder.Services.AddTransient<vLogin>();
            builder.Services.AddTransient<vRegister>();
            builder.Services.AddTransient<vHome>();
            builder.Services.AddTransient<vCatalog>();
            builder.Services.AddTransient<vARSimulator>();
            builder.Services.AddTransient<vProfile>();
            builder.Services.AddTransient<vAppointments>();
            builder.Services.AddTransient<vScheduleAppointment>();
            builder.Services.AddTransient<vVisualTest>();
            builder.Services.AddTransient<vLensDetail>();

            return builder.Build();
        }
    }
}
