using OptivisionApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace OptivisionApp.Services
{
    public interface IDatabaseService
    {
        Task Init();
        Task<List<Usuario>> GetUsuariosAsync();
        Task<int> SaveUsuarioAsync(Usuario usuario);
        Task<Usuario?> GetUsuarioAsync(string email, string password);

        Task<List<Cita>> GetCitasAsync();
        Task<int> SaveCitaAsync(Cita cita);

        Task<List<MarcoLente>> GetMarcosAsync();
        Task<int> SaveMarcoAsync(MarcoLente marco);

        Task<List<ResultadoTest>> GetResultadosTestAsync(int usuarioId);
        Task<int> SaveResultadoTestAsync(ResultadoTest resultado);
    }

    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection _database = null!;

        public DatabaseService()
        {
        }

        public async Task Init()
        {
            if (_database is not null)
                return;

            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "OptivisionLocal.db3");
            _database = new SQLiteAsyncConnection(databasePath);

            // Crear tablas
            await _database.CreateTableAsync<Usuario>();
            await _database.CreateTableAsync<Cita>();
            await _database.CreateTableAsync<MarcoLente>();
            await _database.CreateTableAsync<ResultadoTest>();
        }

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            await Init();
            return await _database.Table<Usuario>().ToListAsync();
        }

        public async Task<int> SaveUsuarioAsync(Usuario usuario)
        {
            await Init();
            if (usuario.Id != 0)
                return await _database.UpdateAsync(usuario);
            else
                return await _database.InsertAsync(usuario);
        }

        public async Task<Usuario?> GetUsuarioAsync(string email, string password)
        {
            await Init();
            // Nota: En una app real la contraseña debe estar hasheada.
            // Esto es solo para simular la base de datos local en MAUI.
            return await _database.Table<Usuario>()
                                  .Where(u => u.Email == email /* && u.Password == password */)
                                  .FirstOrDefaultAsync();
        }

        public async Task<List<Cita>> GetCitasAsync()
        {
            await Init();
            return await _database.Table<Cita>().ToListAsync();
        }

        public async Task<int> SaveCitaAsync(Cita cita)
        {
            await Init();
            if (cita.Id != 0)
                return await _database.UpdateAsync(cita);
            else
                return await _database.InsertAsync(cita);
        }

        public async Task<List<MarcoLente>> GetMarcosAsync()
        {
            await Init();
            return await _database.Table<MarcoLente>().ToListAsync();
        }

        public async Task<int> SaveMarcoAsync(MarcoLente marco)
        {
            await Init();
            if (marco.Id != 0)
                return await _database.UpdateAsync(marco);
            else
                return await _database.InsertAsync(marco);
        }

        public async Task<List<ResultadoTest>> GetResultadosTestAsync(int usuarioId)
        {
            await Init();
            return await _database.Table<ResultadoTest>()
                                  .Where(r => r.UsuarioId == usuarioId)
                                  .OrderByDescending(r => r.FechaTest)
                                  .ToListAsync();
        }

        public async Task<int> SaveResultadoTestAsync(ResultadoTest resultado)
        {
            await Init();
            if (resultado.Id != 0)
                return await _database.UpdateAsync(resultado);
            else
                return await _database.InsertAsync(resultado);
        }
    }
}
