using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DiligenciaProveedores.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DiligenciaProveedores.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Proveedor> Proveedores { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Proveedor>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.RazonSocial)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(e => e.NombreComercial)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(e => e.RUC)
                      .IsRequired()
                      .HasMaxLength(11);

                entity.HasIndex(e => e.RUC).IsUnique();

                entity.Property(e => e.Telefono)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(e => e.CorreoElectronico)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.Property(e => e.SitioWeb)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(e => e.Direccion)
                      .IsRequired()
                      .HasMaxLength(500);

                entity.Property(e => e.Pais)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(e => e.FacturacionAnualUSD)
                      .IsRequired()
                      .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FechaCreacion)
                      .IsRequired()
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(e => e.FechaActualizacion);
            });
        }
    }
}
