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

		if (!string.IsNullOrEmpty(savedEmail) && savedUserId != 0)
		{
			// Restaurar sesión en memoria
			UsuarioActual = new Models.Usuario 
			{ 
				Id = savedUserId, 
				Email = savedEmail,
				Nombre = "Usuario Local" // Opcionalmente, cargar datos completos de BD
			};
			
			// Si hay sesión, llevarlo a la app principal
			MainPage = new AppShell();
		}
		else
		{
			// Si no hay sesión, llevarlo a la pantalla de inicio (HomePage -> NavigationPage para ruteos simples)
			MainPage = new NavigationPage(new Views.HomePage());
		}
	}
}

