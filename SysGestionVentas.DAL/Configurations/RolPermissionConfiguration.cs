using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysGestionVentas.EN;
using System;

namespace SysGestionVentas.DAL.Configurations
{
    public class RolPermissionConfiguration : IEntityTypeConfiguration<RolPermission>
    {
        public void Configure(EntityTypeBuilder<RolPermission>builder)
        {
            // Configura la clave primaria compuesta
            builder.HasKey(rp => new { rp.RolId, rp.PermissionId });

            //Relación con Rol
            builder.HasOne(rp => rp.Rol)
                .WithMany()
                .HasForeignKey(rp => rp.RolId)
                .OnDelete(DeleteBehavior.Restrict);

            //Relación con Permission
            builder.HasOne(rp => rp.Permission)
                .WithMany()
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Restrict);

            //Relación con User (AssignedBy)
            builder.HasOne(rp => rp.AssignedBy)
                .WithMany()
                .HasForeignKey(rp => rp.AssignedByUser)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
