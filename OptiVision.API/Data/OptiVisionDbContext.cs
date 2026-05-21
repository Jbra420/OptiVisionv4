using Microsoft.EntityFrameworkCore;
using OptiVision.API.Models;

namespace OptiVision.API.Data
{
    public class OptiVisionDbContext : DbContext
    {
        public OptiVisionDbContext(DbContextOptions<OptiVisionDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<MarcoLente> MarcosLentes { get; set; }
        public DbSet<Cita> Citas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relación Usuario - Citas (Uno a muchos)
            modelBuilder.Entity<Cita>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Citas)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sembrado de Datos Iniciales (Seed Data)
            // 1. Usuarios de prueba
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario 
                { 
                    Id = 1, 
                    Nombre = "Juan Perez", 
                    Email = "juan.perez@optivision.com", 
                    PasswordHash = "hashed_password_1", // En producción se usa hashing real (BCrypt / PBKDF2)
                    Rol = "Cliente",
                    Receta = "{\"OjoDerecho\":{\"Esfera\":-1.50,\"Cilindro\":-0.50,\"Eje\":180},\"OjoIzquierdo\":{\"Esfera\":-1.75,\"Cilindro\":-0.75,\"Eje\":170}}",
                    FechaRegistro = DateTime.Parse("2026-05-01T10:00:00Z")
                },
                new Usuario 
                { 
                    Id = 2, 
                    Nombre = "Dr. Alejandro Gomez", 
                    Email = "alejandro.gomez@optivision.com", 
                    PasswordHash = "hashed_password_admin", 
                    Rol = "Administrador",
                    FechaRegistro = DateTime.Parse("2026-05-01T09:00:00Z")
                }
            );

            // 2. Marcos de Lentes de prueba
            modelBuilder.Entity<MarcoLente>().HasData(
                new MarcoLente
                {
                    Id = 1,
                    Nombre = "Classic Wayfarer Black",
                    Marca = "OptiStyle",
                    Precio = 120.00m,
                    ImagenUrl = "classic_wayfarer.png",
                    Modelo3DPath = "wayfarer.glb",
                    Descripcion = "Marco de pasta negra clásica, cómodo y resistente, ideal para rostros ovalados.",
                    TipoMarco = "Pasta",
                    Categoria = "Unisex"
                },
                new MarcoLente
                {
                    Id = 2,
                    Nombre = "Aviator Gold Metal",
                    Marca = "AeroMax",
                    Precio = 150.00m,
                    ImagenUrl = "aviator_gold.png",
                    Modelo3DPath = "aviator.glb",
                    Descripcion = "Elegante diseño estilo aviador en metal dorado, ligero y con soporte nasal ajustable.",
                    TipoMarco = "Metal",
                    Categoria = "Hombre"
                },
                new MarcoLente
                {
                    Id = 3,
                    Nombre = "Sleek Round Tortoise",
                    Marca = "RetroLook",
                    Precio = 110.00m,
                    ImagenUrl = "round_tortoise.png",
                    Modelo3DPath = "round_tortoise.glb",
                    Descripcion = "Estilo vintage redondo con acabado de tortuga. Ofrece un toque bohemio y moderno.",
                    TipoMarco = "Pasta",
                    Categoria = "Mujer"
                }
            );

            // 3. Citas de prueba
            modelBuilder.Entity<Cita>().HasData(
                new Cita
                {
                    Id = 1,
                    UsuarioId = 1,
                    FechaCita = DateTime.Parse("2026-05-25T15:00:00Z"),
                    Estado = "Confirmada",
                    Optica = "Sede Norte",
                    Motivo = "Examen de agudeza visual completo",
                    Notas = "Paciente reporta fatiga visual al trabajar en computadora."
                },
                new Cita
                {
                    Id = 2,
                    UsuarioId = 1,
                    FechaCita = DateTime.Parse("2026-05-30T11:00:00Z"),
                    Estado = "Pendiente",
                    Optica = "Sede Norte",
                    Motivo = "Prueba y adaptación de lentes",
                    Notas = "Requiere asesoría sobre filtros de luz azul."
                }
            );
        }
    }
}
