using OptivisionApp.Models;
using SQLite;

namespace OptivisionApp.Services;

public class MockDatabaseService
{
    private SQLiteAsyncConnection? _db;

    // ID del usuario actualmente logueado
    public static int CurrentUserId { get; set; } = 0;

    async Task Init()
    {
        if (_db != null) return;

        // V6: forzar recreación para asegurar que cargue 'lentes1.png' en todos los lentes
        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "OptivisionDataV6.db");
        _db = new SQLiteAsyncConnection(databasePath);

        await _db.CreateTableAsync<User>();
        await _db.CreateTableAsync<Appointment>();
        await _db.CreateTableAsync<VisualTestRecord>();
        await _db.CreateTableAsync<Lense>();

        // Seed inicial de lentes (solo si la tabla está vacía)
        var count = await _db.Table<Lense>().CountAsync();
        if (count == 0)
        {
            await _db.InsertAllAsync(new[]
            {
                new Lense
                {
                    Name = "Aviator Gold",
                    Category = "Sol",
                    Price = 189.00,
                    ImageIcon = "lentes1.png",
                    Stock = 8,
                    Rating = 4.9,
                    ReviewCount = 128,
                    Description = "Lentes polarizados con protección UV400 y marco de titanio ultraligero. Ideales para uso diario y exteriores.",
                    Characteristics = "UV400,Antirreflejo,Polarizado",
                    FrameColors = "Gold,Blue,Black,Ruby",
                    WhatsAppNumber = "593995987809"
                },
                new Lense
                {
                    Name = "Optic Pro 02",
                    Category = "Astigmatismo",
                    Price = 145.00,
                    ImageIcon = "lentes1.png",
                    Stock = 5,
                    Rating = 4.7,
                    ReviewCount = 89,
                    Description = "Marco antirreflejo con filtro de luz azul, ideal para uso prolongado frente a pantallas digitales.",
                    Characteristics = "Antirreflejo,Luz azul,Liviano",
                    FrameColors = "Blue,Black",
                    WhatsAppNumber = "593995987809"
                },
                new Lense
                {
                    Name = "Reader Slim",
                    Category = "Bifocal",
                    Price = 89.00,
                    ImageIcon = "lentes1.png",
                    Stock = 12,
                    Rating = 4.5,
                    ReviewCount = 54,
                    Description = "Ultra ligero de titanio, perfectos para lectura diaria y trabajo de oficina sin fatiga visual.",
                    Characteristics = "Ultra ligero,Titanio,Bifocal",
                    FrameColors = "Black,Silver",
                    WhatsAppNumber = "593995987809"
                },
                new Lense
                {
                    Name = "Clarity Vision",
                    Category = "Miopía",
                    Price = 112.00,
                    ImageIcon = "lentes1.png",
                    Stock = 7,
                    Rating = 4.6,
                    ReviewCount = 72,
                    Description = "Diseñados específicamente para miopía leve a moderada con recubrimiento antirreflejo premium.",
                    Characteristics = "Antirreflejo,HD,Miopía",
                    FrameColors = "Black,Gold,Ruby",
                    WhatsAppNumber = "593995987809"
                },
                new Lense
                {
                    Name = "Sport Shield",
                    Category = "Sol",
                    Price = 210.00,
                    ImageIcon = "lentes1.png",
                    Stock = 4,
                    Rating = 4.8,
                    ReviewCount = 201,
                    Description = "Lentes de alto rendimiento para deportes. Protección UV400 total y resistentes a impactos.",
                    Characteristics = "UV400,Impacto,Polarizado,Deportivo",
                    FrameColors = "Black,Blue",
                    WhatsAppNumber = "593995987809"
                },
                new Lense
                {
                    Name = "Prisma Pro",
                    Category = "Astigmatismo",
                    Price = 165.00,
                    ImageIcon = "lentes1.png",
                    Stock = 6,
                    Rating = 4.4,
                    ReviewCount = 38,
                    Description = "Corrección avanzada de astigmatismo con lentes de alta definición y marco flexible de memoria.",
                    Characteristics = "Alta definición,Flexible,Astigmatismo",
                    FrameColors = "Gold,Silver,Black",
                    WhatsAppNumber = "593995987809"
                }
            });
        }
    }

    // ─── Usuarios (HU-01) ─────────────────────────────────────────────────────

    public async Task<bool> RegisterUserAsync(string name, string email, string password)
    {
        await Init();
        var existing = await _db!.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
        if (existing != null) return false;

        var hash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 10);
        var user = new User { Name = name, Email = email, PasswordHash = hash };
        await _db.InsertAsync(user);
        return true;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        await Init();
        var user = await _db!.Table<User>().Where(u => u.Email == email).FirstOrDefaultAsync();
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            CurrentUserId = user.Id;
            return user;
        }
        return null;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        await Init();
        return await _db!.Table<User>().Where(u => u.Id == id).FirstOrDefaultAsync();
    }

    // ─── Citas (HU-03) ────────────────────────────────────────────────────────

    public async Task<int> AddAppointmentAsync(Appointment appointment)
    {
        await Init();
        appointment.UserId = CurrentUserId;
        return await _db!.InsertAsync(appointment);
    }

    public async Task<List<Appointment>> GetUserAppointmentsAsync()
    {
        await Init();
        return await _db!.Table<Appointment>()
            .Where(a => a.UserId == CurrentUserId)
            .ToListAsync();
    }

    public async Task<List<Appointment>> GetUpcomingAppointmentsAsync()
    {
        await Init();
        var today = DateTime.Today;
        var all = await _db!.Table<Appointment>()
            .Where(a => a.UserId == CurrentUserId)
            .ToListAsync();
        return all.Where(a => a.Date.Date >= today && a.Status != "Cancelada")
                  .OrderBy(a => a.Date)
                  .Take(3)
                  .ToList();
    }

    public async Task<bool> CheckTimeConflictAsync(DateTime date, TimeSpan time)
    {
        await Init();
        var all = await _db!.Table<Appointment>().ToListAsync();
        return all.Any(a => a.Date.Date == date.Date && a.Time == time && a.Status != "Cancelada");
    }

    public async Task<int> UpdateAppointmentAsync(Appointment appointment)
    {
        await Init();
        return await _db!.UpdateAsync(appointment);
    }

    public async Task<int> DeleteAppointmentAsync(Appointment appointment)
    {
        await Init();
        return await _db!.DeleteAsync(appointment);
    }

    // ─── Test Visual (HU-05) ──────────────────────────────────────────────────

    public async Task<int> SaveVisualTestAsync(VisualTestRecord record)
    {
        await Init();
        record.UserId = CurrentUserId;
        return await _db!.InsertAsync(record);
    }

    public async Task<VisualTestRecord?> GetLastVisualTestAsync()
    {
        await Init();
        var records = await _db!.Table<VisualTestRecord>()
            .Where(r => r.UserId == CurrentUserId)
            .ToListAsync();
        return records.OrderByDescending(r => r.TestDate).FirstOrDefault();
    }

    public async Task<List<VisualTestRecord>> GetVisualTestHistoryAsync()
    {
        await Init();
        var records = await _db!.Table<VisualTestRecord>()
            .Where(r => r.UserId == CurrentUserId)
            .ToListAsync();
        return records.OrderByDescending(r => r.TestDate).ToList();
    }

    // ─── Catálogo (HU-02 / HU-10) ────────────────────────────────────────────

    public async Task<List<Lense>> GetCatalogAsync(string category = "Todos")
    {
        await Init();
        if (category == "Todos")
            return await _db!.Table<Lense>().ToListAsync();

        return await _db!.Table<Lense>()
            .Where(l => l.Category == category)
            .ToListAsync();
    }

    public async Task<Lense?> GetLenseByIdAsync(int id)
    {
        await Init();
        return await _db!.Table<Lense>().Where(l => l.Id == id).FirstOrDefaultAsync();
    }
}
