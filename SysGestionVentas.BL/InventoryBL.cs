using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class InventoryBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Inventory"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pInventory">Objeto <see cref="Inventory"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación definidas en la entidad.
        /// El mensaje de la excepción contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(Inventory pInventory)
        {
            var contexto = new ValidationContext(pInventory);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pInventory, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo inventario en el sistema.
        /// </summary>
        /// <param name="pInventory">
        /// Objeto <see cref="Inventory"/> con los datos del inventario a registrar.
        /// Los campos <c>PurchasePrice</c>, <c>SalePrice</c>, <c>MinimumStock</c>,
        /// <c>CurrentStock</c> y <c>ProductId</c> son obligatorios.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación en base de datos.</exception>
        public static async Task<int> GuardarAsync(Inventory pInventory)
        {
            ValidarEntidad(pInventory);
            return await InventoryDAL.GuardarAsync(pInventory);
        }

        /// <summary>
        /// Valida y modifica los datos de un inventario existente en el sistema.
        /// </summary>
        /// <param name="pInventory">
        /// Objeto <see cref="Inventory"/> con el <c>InventoryId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el inventario no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(Inventory pInventory)
        {
            ValidarEntidad(pInventory);
            return await InventoryDAL.ModificarAsync(pInventory);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un inventario, cambiando su estado en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pInventory">
        /// Objeto <see cref="Inventory"/> con el <c>InventoryId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(Inventory pInventory)
        {
            if (pInventory.InventoryId <= 0)
                throw new Exception("El ID de inventario no es válido.");

            if (pInventory.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await InventoryDAL.EliminarAsync(pInventory);
        }

        /// <summary>
        /// Obtiene un inventario específico por su identificador, incluyendo
        /// su relación con <see cref="ProductList"/>.
        /// </summary>
        /// <param name="pInventory">Objeto <see cref="Inventory"/> con el <c>InventoryId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Inventory"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Inventory?> ObtenerPorIdAsync(Inventory pInventory)
        {
            if (pInventory.InventoryId <= 0)
                throw new Exception("El ID de inventario no es válido.");

            return await InventoryDAL.ObtenerPorIdAsync(pInventory);
        }

        /// <summary>
        /// Obtiene una lista de inventarios aplicando filtros opcionales.
        /// Los parámetros con valor <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pInventory">
        /// Objeto <see cref="Inventory"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>ProductId</c>: filtra por producto asociado (0 = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Inventory"/> que cumplen los filtros indicados,
        /// ordenados por producto de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Inventory>> ObtenerTodosAsync(Inventory pInventory)
        {
            return await InventoryDAL.ObtenerTodosAsync(pInventory);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de inventarios con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Inventory}"/> que define los filtros, el tamaño de página
        /// y el número de página. No puede ser <c>null</c>.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Inventory}"/> con la lista de inventarios encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.</exception>
        public static async Task<PagedResult<Inventory>> BuscarAsync(PagedQuery<Inventory> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            return await InventoryDAL.BuscarAsync(pPagedQuery);
        }

        #endregion
    }
}