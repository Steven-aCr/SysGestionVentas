using System;
using System.Collections.Generic;

namespace SysGestionVentas.EN.Pagination
{
    public class PagedResult<T> where T : class
    {
        public List<T> Items { get; set; } = new List<T>(); // Lista de elementos en la página actual
        public int TotalCount { get; set; } // Total de elementos disponibles
        public int CurrentPage { get; set; } // Número de página actual
        public int PageSize { get; set; } // Cantidad de elementos por página

        /// <summary>
        /// Total de páginas disponibles , calculado a partir de:
        ///<see cref = "TotalCount" /> y < see cref="PageSize"/>.
        /// Retorna 0 si no hay registros.
        /// </summary>
        public int TotalPages => PageSize > 0
            ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
        public bool HasPreviousPage => CurrentPage > 1; // Indica si hay una página anterior
        public bool HasNextPage => CurrentPage < TotalPages; // Indica si hay una página siguiente

        /// <summary>
        /// Indica el índice del primer elemento en la página actual, 
        /// basado en el número de página y el tamaño de página.
        /// </summary>
        public int FirstItemIndex => TotalCount == 0 ? 
            0 : (CurrentPage - 1) * PageSize + 1;

        /// <summary>
        /// Indica el número del último registro de la página actual en el conjunto total.
        /// </summary>
        public int LastItemIndex => TotalCount == 0 ? 
            0 : Math.Min(CurrentPage * PageSize, TotalCount);
    }
}
