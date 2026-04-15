using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN.ViewModels
{
    /// <summary>
    /// ViewModel que representa una línea de detalle dentro del formulario
    /// de creación de documento. Los campos calculados (Subtotal, TaxAmount,
    /// TotalAmount) son recalculados en la capa BL antes de persistirse.
    /// </summary>
    public class CreateDocumentDetailModel
    {
        /// <summary>Identificador del producto seleccionado.</summary>
        [Required(ErrorMessage = "El producto es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "Seleccione un producto válido.")]
        public int ProductId { get; set; }

        /// <summary>Nombre del producto (solo para visualización en la vista).</summary>
        public string? ProductName { get; set; }

        /// <summary>Cantidad solicitada. Debe ser mayor a 0.</summary>
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Quantity { get; set; }

        /// <summary>Precio unitario de venta. Se autocompletado desde el inventario.</summary>
        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a $0.00.")]
        public decimal UnitPrice { get; set; }

        /// <summary>Monto de descuento aplicado a la línea. Por defecto 0.</summary>
        [Range(0, double.MaxValue, ErrorMessage = "El descuento no puede ser negativo.")]
        public decimal DiscountAmount { get; set; } = 0;

        /// <summary>Porcentaje de impuesto aplicado. Por defecto 13%.</summary>
        [Range(0, 100)]
        public decimal TaxPercentage { get; set; } = 13;

        /// <summary>Notas opcionales de la línea.</summary>
        [StringLength(255)]
        public string? Notes { get; set; }
    }
}
