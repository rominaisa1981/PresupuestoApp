using Microsoft.EntityFrameworkCore;
using PresupuestoApi.Models;

namespace PresupuestoApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Quincena> Quincenas => Set<Quincena>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Usuario
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Quincena: una quincena única por usuario/año/mes/numero
        modelBuilder.Entity<Quincena>()
            .HasIndex(q => new { q.UsuarioId, q.Anio, q.Mes, q.Numero })
            .IsUnique();

        // IMPORTANTE: Restrict (no Cascade) en las relaciones con Usuario
        // para evitar el error "multiple cascade paths" en SQL Server.
        // El cascade real ocurre vía Quincena -> Movimientos.
        modelBuilder.Entity<Quincena>()
            .HasOne(q => q.Usuario)
            .WithMany(u => u.Quincenas)
            .HasForeignKey(q => q.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Categoria
        modelBuilder.Entity<Categoria>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Categorias)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Categoria>()
            .Property(c => c.PresupuestoMensual)
            .HasColumnType("decimal(18,2)");

        // Movimiento -> Usuario: Restrict (lo gestionamos manualmente si hay que borrar usuario)
        modelBuilder.Entity<Movimiento>()
            .HasOne(m => m.Usuario)
            .WithMany(u => u.Movimientos)
            .HasForeignKey(m => m.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Movimiento -> Quincena: Cascade (borrar quincena borra sus movimientos)
        modelBuilder.Entity<Movimiento>()
            .HasOne(m => m.Quincena)
            .WithMany(q => q.Movimientos)
            .HasForeignKey(m => m.QuincenaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Movimiento -> Categoria: SetNull (si borras categoría, los movimientos quedan sin categoría)
        modelBuilder.Entity<Movimiento>()
            .HasOne(m => m.Categoria)
            .WithMany(c => c.Movimientos)
            .HasForeignKey(m => m.CategoriaId)
            .OnDelete(DeleteBehavior.SetNull);

        // Auto-referencia padre/hijo
        modelBuilder.Entity<Movimiento>()
            .HasOne(m => m.MovimientoPadre)
            .WithMany(m => m.SubMovimientos)
            .HasForeignKey(m => m.MovimientoPadreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}