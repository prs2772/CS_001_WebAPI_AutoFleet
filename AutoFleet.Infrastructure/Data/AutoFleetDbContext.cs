using Microsoft.EntityFrameworkCore;
using AutoFleet.Core.Entities;

namespace AutoFleet.Infrastructure.Data;

public class AutoFleetDbContext : DbContext
{
    public AutoFleetDbContext(DbContextOptions<AutoFleetDbContext> options) : base(options)
    {
    }

    // Defining Entities
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. VIN (CRITICAL)
        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.Vin)
            .IsUnique(); // Grants DB rejects duplicated

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Vin)
            .HasMaxLength(17)
            .IsFixedLength(); // Optimizated: CHAR(17)faster and better on this case than VARCHAR(17)

        // 2. Decimal precision on Vehicle and Km
        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Price)
            .HasColumnType("decimal(18,2)"); // 18 digits, 2 decimals

        modelBuilder.Entity<Vehicle>()
            .Property(v => v.KmPerLiter)
            .HasColumnType("decimal(5,2)"); // 999.99 (Enough)

        // 3. Enum as text to give clues to DB admins
        modelBuilder.Entity<Vehicle>()
            .Property(v => v.Status)
            .HasConversion<string>(); // Stores "Available" instead number

        base.OnModelCreating(modelBuilder);
    }
}

