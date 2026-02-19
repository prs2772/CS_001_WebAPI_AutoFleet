using Microsoft.EntityFrameworkCore;
using AutoFleet.Core.Entities;

namespace AutoFleet.Infrastructure.Data
{
    public class AutoFleetDbContext : DbContext
    {
        public AutoFleetDbContext(DbContextOptions<AutoFleetDbContext> options) : base(options)
        {
        }

        // Aquí definimos las tablas
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración adicional (Fluent API) si no quieres usar Data Annotations
            // Por ejemplo, definir precisión para el precio
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.Price)
                .HasColumnType("decimal(18,2)");
        }
    }
}
