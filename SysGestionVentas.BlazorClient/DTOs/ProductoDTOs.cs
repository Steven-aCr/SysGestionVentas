namespace SysGestionVentas.BlazorClient.DTOs
{
    public class ProductoDTOs
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public long CategoriaId { get; set; }
    }
}