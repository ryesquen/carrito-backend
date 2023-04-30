using Carrito.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Carrito.Persistence.Contexts
{
    public partial class CarritoContext : DbContext
    {
        public CarritoContext()
        {
        }

        public CarritoContext(DbContextOptions<CarritoContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Articulo> Articulos { get; set; } = null!;
        public virtual DbSet<ArticulosTienda> ArticulosTiendas { get; set; } = null!;
        public virtual DbSet<Cliente> Clientes { get; set; } = null!;
        public virtual DbSet<ClientesArticulo> ClientesArticulos { get; set; } = null!;
        public virtual DbSet<Tienda> Tiendas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Articulo>(entity =>
            {
                entity.Property(e => e.Codigo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
            });

            modelBuilder.Entity<ArticulosTienda>(entity =>
            {
                entity.HasKey(e => new { e.ArticuloId, e.TiendaId })
                    .HasName("PK__Articulo__1F1F69B12CB7F003");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.HasOne(d => d.Articulo)
                    .WithMany(p => p.ArticulosTienda)
                    .HasForeignKey(d => d.ArticuloId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Articulos__Artic__3D5E1FD2");

                entity.HasOne(d => d.Tienda)
                    .WithMany(p => p.ArticulosTienda)
                    .HasForeignKey(d => d.TiendaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Articulos__Tiend__3E52440B");
            });

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.Property(e => e.Apellido)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Direccion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ClientesArticulo>(entity =>
            {
                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.HasOne(d => d.Articulo)
                    .WithMany(p => p.ClientesArticulos)
                    .HasForeignKey(d => d.ArticuloId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ClientesA__Artic__4222D4EF");

                entity.HasOne(d => d.Cliente)
                    .WithMany(p => p.ClientesArticulos)
                    .HasForeignKey(d => d.ClienteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ClientesA__Clien__412EB0B6");
            });

            modelBuilder.Entity<Tienda>(entity =>
            {
                entity.Property(e => e.Direccion)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Sucursal)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}