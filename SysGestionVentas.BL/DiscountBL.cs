using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class DiscountBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Discount"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pDiscount">Objeto <see cref="Discount"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(Discount pDiscount)
        {
            var contexto = new ValidationContext(pDiscount);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pDiscount, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo descuento en el sistema.
        /// Verifica unicidad del nombre y coherencia del rango de fechas en la capa DAL.
        /// </summary>
        /// <param name="pDiscount">Objeto <see cref="Discount"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el nombre ya existe, si las fechas son incoherentes,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> GuardarAsync(Discount pDiscount)
        {
            ValidarEntidad(pDiscount);
            return await DiscountDAL.GuardarAsync(pDiscount);
        }

        /// <summary>
        /// Valida y modifica los datos de un descuento existente en el sistema.
        /// Verifica unicidad del nombre y coherencia del rango de fechas en la capa DAL.
        /// </summary>
        /// <param name="pDiscount">
        /// Objeto <see cref="Discount"/> con el <c>DiscountId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el descuento no existe, si el nombre está duplicado, si las fechas son
        /// incoherentes, o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> ModificarAsync(Discount pDiscount)
        {
            if (pDiscount.DiscountId <= 0)
                throw new Exception("El ID de descuento no es válido.");

            ValidarEntidad(pDiscount);
            return await DiscountDAL.ModificarAsync(pDiscount);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un descuento cambiando su estado en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pDiscount">
        /// Objeto <see cref="Discount"/> con el <c>DiscountId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el ID no es válido, si el descuento no existe,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> EliminarAsync(Discount pDiscount)
        {
            if (pDiscount.DiscountId <= 0)
                throw new Exception("El ID de descuento no es válido.");

            if (pDiscount.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await DiscountDAL.EliminarAsync(pDiscount);
        }

        /// <summary>
        /// Obtiene un descuento específico por su identificador,
        /// incluyendo su relación con <see cref="Status"/>.
        /// </summary>
        /// <param name="pDiscount">Objeto <see cref="Discount"/> con el <c>DiscountId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Discount"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Discount?> ObtenerPorIdAsync(Discount pDiscount)
        {
            if (pDiscount.DiscountId <= 0)
                throw new Exception("El ID de descuento no es válido.");

            return await DiscountDAL.ObtenerPorIdAsync(pDiscount);
        }

        /// <summary>
        /// Obtiene una lista de descuentos aplicando filtros opcionales de nombre,
        /// estado y rango de fechas.
        /// </summary>
        /// <param name="pDiscount">Objeto <see cref="Discount"/> usado como filtro de búsqueda.</param>
        /// <param name="pFromDate">Filtra por <c>StartDate</c> mayor o igual a esta fecha (null = sin filtro).</param>
        /// <param name="pToDate">Filtra por <c>EndDate</c> menor o igual a esta fecha (null = sin filtro).</param>
        /// <returns>Lista de objetos <see cref="Discount"/> ordenados por nombre de forma ascendente.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Discount>> ObtenerTodosAsync(
            Discount pDiscount,
            DateTime? pFromDate = null,
            DateTime? pToDate = null)
        {
            return await DiscountDAL.ObtenerTodosAsync(pDiscount, pFromDate, pToDate);
        }

        /// <summary>
        /// Obtiene los descuentos vigentes a una fecha específica, filtrando por estado activo
        /// y rango de fechas válido.
        /// </summary>
        /// <param name="pDate">Fecha de referencia para evaluar la vigencia del descuento.</param>
        /// <param name="pActiveStatusId">Identificador del estado "Activo" en la tabla <see cref="Status"/>.</param>
        /// <returns>
        /// Lista de objetos <see cref="Discount"/> vigentes, ordenados por porcentaje descendente.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>StatusId</c> activo no es válido o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<List<Discount>> ObtenerVigentesAsync(DateTime pDate, int pActiveStatusId)
        {
            if (pActiveStatusId <= 0)
                throw new Exception("El ID de estado activo no es válido.");

            return await DiscountDAL.ObtenerVigentesAsync(pDate, pActiveStatusId);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de descuentos con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Discount}"/> con los filtros y parámetros de paginación.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Discount}"/> con la lista de descuentos encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">
        /// Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<PagedResult<Discount>> BuscarAsync(PagedQuery<Discount> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            return await DiscountDAL.BuscarAsync(pPagedQuery);
        }

        #endregion
    }
}