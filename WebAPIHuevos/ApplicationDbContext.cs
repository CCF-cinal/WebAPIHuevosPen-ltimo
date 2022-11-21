using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPIHuevos.Entidades;

namespace WebAPIHuevos
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EncargadoHuevo>()
                .HasKey(en => new { en.EncargadoId, en.HuevoId });
        }

        public DbSet<Encargado> Encargados { get; set; }
        public DbSet<Huevo> Huevos { get; set; }
        public DbSet<Cursos> Cursos { get; set; }

        public DbSet<EncargadoHuevo> AlumnoClase { get; set; }
    }
}
