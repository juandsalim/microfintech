// src/Infrastructure/Data/ApplicationDbContext.cs
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Aquí le decimos a EF Core qué entidades queremos convertir en tablas
    public DbSet<Account> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Fluent API: Aquí definimos reglas estrictas para la base de datos
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Currency).IsRequired().HasMaxLength(3);
            entity.Property(e => e.Balance).HasPrecision(18, 4); // Precisión financiera crítica
        });
    }
}