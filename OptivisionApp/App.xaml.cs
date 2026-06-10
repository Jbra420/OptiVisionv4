namespace OptivisionApp;

public partial class App : Application
{
	public static Models.Usuario? UsuarioActual { get; set; }

	public App()
	{
		InitializeComponent();

		// Verificar si hay una sesión guardada
		string savedEmail = Preferences.Get("UserEmail", string.Empty);
		int savedUserId = Preferences.Get("UserId", 0);

		// Configurar siempre AppShell para que Shell.Current no sea null
		MainPage = new AppShell();
			
		if (!string.IsNullOrEmpty(savedEmail) && savedUserId != 0)
		{
            // Restaurar sesión en memoria
			UsuarioActual = new Models.Usuario 
			{ 
				Id = savedUserId, 
				Email = savedEmail,
				Nombre = "Usuario Local"
			};

            // Retrasar la navegación para permitir que Shell se inicialice completamente
            Application.Current?.Dispatcher.Dispatch(async () => 
            {
                await Shell.Current.GoToAsync("//MainApp");
            });
        }
        // Si no hay sesión, no hacemos nada porque HomePage es la primera ruta en AppShell y cargará por defecto.
	}
}

