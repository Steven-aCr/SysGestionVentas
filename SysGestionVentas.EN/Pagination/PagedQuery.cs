using System;

namespace SysGestionVentas.EN.Pagination
{
    public class PagedQuery<T> where T : class
    {
        public T Filter { get; set; } = default!;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int Top { get; set; } = 0; // Si se establece, ignora Page y PageSize
        public DateTime? FromDate { get; set; } // Filtro opcional para fecha de inicio
        public DateTime? ToDate { get; set; } // Filtro opcional para fecha de fin
        public int Skip => (Page - 1) * PageSize; // Calcula el número de registros a omitir

    }
}
