using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class InventoryMovementBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="InventoryMovement"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pInventoryMovement">Objeto <see cref="InventoryMovement"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación definidas en la entidad.
        /// El mensaje de la excepción contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(InventoryMovement pInventoryMovement)
        {
            var contexto = new ValidationContext(pInventoryMovement);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pInventoryMovement, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo movimiento de inventario en el sistema.
        /// </summary>
        /// <param name="pInventoryMovement">
        /// Objeto <see cref="InventoryMovement"/> con los datos del movimiento a registrar.
        /// Los campos <c>MovementType</c>, <c>Quantity</c>, <c>UnitCost</c>,
        /// <c>CreatedByUser</c> e <c>InventoryId</c> son obligatorios.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación en base de datos.</exception>
        public static async Task<int> GuardarAsync(InventoryMovement pInventoryMovement)
        {
            ValidarEntidad(pInventoryMovement);
            return await InventoryMovementDAL.GuardarAsync(pInventoryMovement);
        }

        /// <summary>
        /// Valida y modifica los datos de un movimiento de inventario existente en el sistema.
        /// </summary>
        /// <param name="pInventoryMovement">
        /// Objeto <see cref="InventoryMovement"/> con el <c>InventoryMovementId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el movimiento no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(InventoryMovement pInventoryMovement)
        {
            ValidarEntidad(pInventoryMovement);
            return await InventoryMovementDAL.ModificarAsync(pInventoryMovement);
        }

        /// <summary>
        /// Elimina físicamente un movimiento de inventario de la base de datos.
        /// Esta operación es irreversible.
        /// </summary>
        /// <param name="pInventoryMovement">
        /// Objeto <see cref="InventoryMovement"/> con el <c>InventoryMovementId</c> del registro a eliminar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se eliminó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(InventoryMovement pInventoryMovement)
        {
            if (pInventoryMovement.InventoryMovementId <= 0)
                throw new Exception("El ID de movimiento de inventario no es válido.");

            return await InventoryMovementDAL.EliminarAsync(pInventoryMovement);
        }

        /// <summary>
        /// Obtiene un movimiento de inventario específico por su identificador, incluyendo
        /// sus relaciones con <see cref="Inventory"/> y el usuario que lo creó.
        /// </summary>
        /// <param name="pInventoryMovement">Objeto <see cref="InventoryMovement"/> con el <c>InventoryMovementId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="InventoryMovement"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<InventoryMovement?> ObtenerPorIdAsync(InventoryMovement pInventoryMovement)
        {
            if (pInventoryMovement.InventoryMovementId <= 0)
                throw new Exception("El ID de movimiento de inventario no es válido.");

            return await InventoryMovementDAL.ObtenerPorIdAsync(pInventoryMovement);
        }

        /// <summary>
        /// Obtiene una lista de movimientos de inventario aplicando filtros opcionales.
        /// Los parámetros con valor <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pInventoryMovement">
        /// Objeto <see cref="InventoryMovement"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>InventoryId</c>: filtra por inventario asociado (0 = sin filtro).</description></item>
        ///   <item><description><c>CreatedByUser</c>: filtra por usuario que registró el movimiento (0 = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="InventoryMovement"/> que cumplen los filtros indicados,
        /// ordenados por fecha de creación de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<InventoryMovement>> ObtenerTodosAsync(InventoryMovement pInventoryMovement)
        {
            return await InventoryMovementDAL.ObtenerTodosAsync(pInventoryMovement);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de movimientos de inventario con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{InventoryMovement}"/> que define los filtros, el tamaño de página
        /// y el número de página. No puede ser <c>null</c>.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{InventoryMovement}"/> con la lista de movimientos encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.</exception>
        public static async Task<PagedResult<InventoryMovement>> BuscarAsync(PagedQuery<InventoryMovement> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            return await InventoryMovementDAL.BuscarAsync(pPagedQuery);
        }

        #endregion
    }
}