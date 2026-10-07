using LeoKitchenFasti.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LeoKitchenFasti.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // --- TABLAS DE SEGURIDAD (Tus modelos probados) ---
        public DbSet<User> Users { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<UserSession> UserSessions { get; set; } // Opcional, si sigues usando esta tabla para Refresh Tokens

        // --- TABLAS DEL RESTAURANTE (La nueva operación) ---
        public DbSet<Table> Tables { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Collation para soportar correctamente caracteres en español (eñes, acentos)
            modelBuilder.UseCollation("utf8mb4_spanish_ci");

            // --- TU CONFIGURACIÓN EXACTA DE ROLES Y PERMISOS ---

            // Índices únicos para evitar duplicados
            modelBuilder.Entity<Permiso>().HasIndex(p => p.NombreSistema).IsUnique();
            modelBuilder.Entity<Rol>().HasIndex(r => r.Nombre).IsUnique();

            // Configuración de la relación Muchos a Muchos con tabla intermedia explícita
            modelBuilder.Entity<Rol>()
                .HasMany(r => r.Permisos)
                .WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolPermiso", // Nombre de la tabla en BD

                    j => j.HasOne<Permiso>()
                          .WithMany()
                          .HasForeignKey("PermisoId"),

                    j => j.HasOne<Rol>()
                          .WithMany()
                          .HasForeignKey("RolId"),

                    j => j.ToTable("RolPermiso")
                );
        }
    }
}