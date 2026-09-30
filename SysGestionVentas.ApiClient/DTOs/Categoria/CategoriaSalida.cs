namespace SysGestionVentas.ApiClient.DTOs.Categoria
{
    public class CategoriaSalida
    {
        public int IdCategoria { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public int IdEstado { get; set; }

        public int CreadoPorUsuario { get; set; }
    }
}