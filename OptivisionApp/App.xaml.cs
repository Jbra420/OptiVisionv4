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
			
		if (string.IsNullOrEmpty(savedEmail) || savedUserId == 0)
		{
			// Si no hay sesión, navegar a HomePage de inmediato
			Shell.Current.GoToAsync("//HomePage");
		}
        else
        {
            // Restaurar sesión en memoria
			UsuarioActual = new Models.Usuario 
			{ 
				Id = savedUserId, 
				Email = savedEmail,
				Nombre = "Usuario Local" // Opcionalmente, cargar datos completos de BD
			};
            Shell.Current.GoToAsync("//MainApp");
        }
	}
}

