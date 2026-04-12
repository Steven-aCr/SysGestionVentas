using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="Inventory"/>.
    /// Orquesta validaciones de datos, reglas de negocio y delega la persistencia a <see cref="InventoryDAL"/>.
    /// </summary>
    public class InventoryBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Inventory"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pInventory">Objeto <see cref="Inventory"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// </exception>
        private static void ValidarEntidad(Inventory pInventory)
        {
            var contexto = new ValidationContext(pInventory);
            var resultados = new List<ValidationResult>();
            bool esValido = Validator.TryValidateObject(pInventory, contexto, resultados, validateAllProperties: true);
            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        /// <summary>
        /// Valida que el precio de venta sea mayor o igual al precio de compra,
        /// según la restricción de negocio definida en la base de datos.
        /// </summary>
        /// <param name="pInventory">Objeto <see cref="Inventory"/> con los precios a validar.</param>
        /// <exception cref="Exception">Se lanza si el precio de venta es menor al precio de compra.</exception>
        private static void ValidarPrecios(Inventory pInventory)
        {
            if (pInventory.SalePrice < pInventory.PurchasePrice)
                throw new Exception("El precio de venta no puede ser menor al precio de compra.");
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo registro de inventario en el sistema.
        /// Verifica que el precio de venta no sea inferior al precio de compra.
        /// </summary>
        /// <param name="pInventory">Objeto <see cref="Inventory"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si los precios son inconsistentes o si ocurre un error en base de datos.</exception>
        public static async Task<int> GuardarAsync(Inventory pInventory)
        {
            ValidarEntidad(pInventory);
            ValidarPrecios(pInventory);
            return await InventoryDAL.GuardarAsync(pInventory);
        }

        /// <summary>
        /// Valida y modifica los datos de un registro de inventario existente.
        /// Verifica que el precio de venta no sea inferior al precio de compra.
        /// No permite cambiar el producto asociado (<c>ProductId</c>).
        /// </summary>
        /// <param name="pInventory">
        /// Objeto <see cref="Inventory"/> con el <c>InventoryId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el inventario no existe, los precios son inconsistentes, o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(Inventory pInventory)
        {
            if (pInventory.InventoryId <= 0)
                throw new Exception("El ID de inventario no es válido.");

            ValidarEntidad(pInventory);
            ValidarPrecios(pInventory);
            return await InventoryDAL.ModificarAsync(pInventory);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un registro de inventario, cambiando su estado.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pInventory">
        /// Objeto <see cref="Inventory"/> con el <c>InventoryId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido, si el inventario no existe, o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(Inventory pInventory)
        {
            if (pInventory.InventoryId <= 0)
                throw new Exception("El ID de inventario no es válido.");

            if (pInventory.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await InventoryDAL.EliminarAsync(pInventory);
        }

        /// <summary>
        /// Obtiene un registro de inventario específico por su identificador,
        /// incluyendo sus relaciones con <see cref="ProductList"/> y <see cref="Status"/>.
        /// </summary>
        /// <param name="pInventory">Objeto <see cref="Inventory"/> con el <c>InventoryId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Inventory"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Inventory?> ObtenerPorIdAsync(Inventory pInventory)
        {
            if (pInventory.InventoryId <= 0)
                throw new Exception("El ID de inventario no es válido.");

            return await InventoryDAL.ObtenerPorIdAsync(pInventory);
        }

        /// <summary>
        /// Obtiene una lista de registros de inventario aplicando filtros opcionales.
        /// </summary>
        /// <param name="pInventory">Objeto <see cref="Inventory"/> usado como filtro de búsqueda.</param>
        /// <returns>Lista de objetos <see cref="Inventory"/> ordenados por nombre de producto.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Inventory>> ObtenerTodosAsync(Inventory pInventory)
        {
            return await InventoryDAL.ObtenerTodosAsync(pInventory);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de registros de inventario con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Inventory}"/> con los filtros y parámetros de paginación.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Inventory}"/> con la lista de registros encontrados
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