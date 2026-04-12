using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.DAL.Configurations
{
    public class ProductDiscountConfiguration : IEntityTypeConfiguration<ProductDiscount>
    {
        public void Configure(EntityTypeBuilder<ProductDiscount> builder)
        {
            // Configurar la clave primaria compuesta
            builder.HasKey(pd => new { pd.ProductId, pd.DiscountId });

            // Configuración de la relación con Product
            builder.HasOne(pd => pd.Product)
                   .WithMany()
                   .HasForeignKey(pd => pd.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la relación con Discount
            builder.HasOne(pd => pd.Discount)
                   .WithMany()
                   .HasForeignKey(pd => pd.DiscountId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la relación con User (AssignedBy)
            builder.HasOne(pd => pd.AssignedBy)
                   .WithMany()
                   .HasForeignKey(pd => pd.AssignedByUser)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
