namespace SysGestionVentas.ApiClient.DTOs.Producto
{
    public class ProductoSalida
    {
        public int IdProducto { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public string CodigoBarras { get; set; } = string.Empty;

        public string? ImagenUrl { get; set; }

        public DateTime FechaCreacion { get; set; }

        public int IdEstado { get; set; }

        public int IdCategoria { get; set; }
    }
}