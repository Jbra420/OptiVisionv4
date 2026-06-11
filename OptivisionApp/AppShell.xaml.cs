using OptivisionApp.Views;

namespace OptivisionApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Registrar rutas para páginas fuera del TabBar
        Routing.RegisterRoute("vLensDetail", typeof(vLensDetail));
        Routing.RegisterRoute("vARSimulator", typeof(vARSimulator));
        Routing.RegisterRoute("vVisualTest", typeof(vVisualTest));
        Routing.RegisterRoute("vAlerts", typeof(vAlerts));
        Routing.RegisterRoute("vAppointments", typeof(vAppointments));
        Routing.RegisterRoute("vScheduleAppointment", typeof(vScheduleAppointment));

        // Redirigir a MainApp si hay sesión activa
        if (App.UsuarioActual != null)
        {
            CurrentItem = this.Items.FirstOrDefault(x => x.Route == "MainApp");
        }
    }
}
