using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Parcial.Models;

namespace Parcial.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Código único
            builder.Entity<Curso>()
                .HasIndex(c => c.Codigo)
                .IsUnique();

            // Validación HorarioInicio < HorarioFin
            builder.Entity<Curso>()
                .HasCheckConstraint("CK_Curso_Horario", "HorarioInicio < HorarioFin");

            // Restricción: un usuario no puede matricularse más de una vez en el mismo curso
            builder.Entity<Matricula>()
                .HasIndex(m => new { m.CursoId, m.UsuarioId })
                .IsUnique();

            // Seed inicial de cursos
            builder.Entity<Curso>().HasData(
                new Curso { Id = 1, Codigo = "CS101", Nombre = "Programación I", Creditos = 4, CupoMaximo = 30, HorarioInicio = new TimeSpan(8, 0, 0), HorarioFin = new TimeSpan(10, 0, 0), Activo = true },
                new Curso { Id = 2, Codigo = "CS102", Nombre = "Bases de Datos", Creditos = 3, CupoMaximo = 25, HorarioInicio = new TimeSpan(10, 0, 0), HorarioFin = new TimeSpan(12, 0, 0), Activo = true },
                new Curso { Id = 3, Codigo = "CS103", Nombre = "Redes", Creditos = 3, CupoMaximo = 20, HorarioInicio = new TimeSpan(14, 0, 0), HorarioFin = new TimeSpan(16, 0, 0), Activo = true }
            );
        }
    }
}
