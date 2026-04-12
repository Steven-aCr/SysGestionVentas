using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="InventoryMovement"/>.
    /// Los movimientos de inventario son registros históricos inmutables; por diseño,
    /// no se permite su modificación ni eliminación una vez registrados.
    /// Esta clase orquesta la validación y el registro de nuevos movimientos,
    /// actualizando además el stock del inventario asociado de forma atómica.
    /// </summary>
    public class InventoryMovementBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="InventoryMovement"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pMovement">Objeto <see cref="InventoryMovement"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// </exception>
        private static void ValidarEntidad(InventoryMovement pMovement)
        {
            var contexto = new ValidationContext(pMovement);
            var resultados = new List<ValidationResult>();
            bool esValido = Validator.TryValidateObject(pMovement, contexto, resultados, validateAllProperties: true);
            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "Registro de Movimientos"

        /// <summary>
        /// Valida y registra un nuevo movimiento de inventario en el sistema,
        /// actualizando el stock del inventario asociado de forma atómica dentro
        /// de una transacción. Los tipos de movimiento afectan el stock así:
        /// <list type="bullet">
        ///   <item><description>Entrada (MovementTypeId = 1): incrementa el stock.</description></item>
        ///   <item><description>Salida (MovementTypeId = 2): decrementa el stock.</description></item>
        ///   <item><description>Ajuste (MovementTypeId = 3): reemplaza el stock actual por la cantidad indicada.</description></item>
        ///   <item><description>Devolución (MovementTypeId = 4): incrementa el stock.</description></item>
        ///   <item><description>Transferencia (MovementTypeId = 5): decrementa el stock.</description></item>
        /// </list>
        /// </summary>
        /// <param name="pMovement">
        /// Objeto <see cref="InventoryMovement"/> con los datos del movimiento a registrar.
        /// Los campos <c>MovementTypeId</c>, <c>Quantity</c>, <c>UnitCost</c>,
        /// <c>InventoryId</c> y <c>CreatedByUser</c> son obligatorios.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se registró correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el inventario no existe, si una salida dejaría el stock en negativo,
        /// o si ocurre un error durante la transacción.
        /// </exception>
        public static async Task<int> RegistrarMovimientoAsync(InventoryMovement pMovement)
        {
            if (pMovement.InventoryId <= 0)
                throw new Exception("El ID de inventario no es válido.");

            if (pMovement.CreatedByUser <= 0)
                throw new Exception("El ID del usuario es obligatorio.");

            ValidarEntidad(pMovement);

            using var dbContexto = new DbContexto();
            using var transaction = await dbContexto.Database.BeginTransactionAsync();

            try
            {
                // Obtener el inventario actual dentro de la transacción
                var inventory = await dbContexto.Inventory.FindAsync(pMovement.InventoryId)
                    ?? throw new Exception($"No se encontró el inventario con ID {pMovement.InventoryId}.");

                // Actualizar stock según el tipo de movimiento
                switch (pMovement.MovementTypeId)
                {
                    case 1: // Entrada
                    case 4: // Devolución
                        inventory.CurrentStock += pMovement.Quantity;
                        break;

                    case 2: // Salida
                    case 5: // Transferencia
                        if (inventory.CurrentStock < pMovement.Quantity)
                            throw new Exception("Stock insuficiente para registrar la salida.");
                        inventory.CurrentStock -= pMovement.Quantity;
                        break;

                    case 3: // Ajuste
                        inventory.CurrentStock = pMovement.Quantity;
                        break;

                    default:
                        throw new Exception("Tipo de movimiento no reconocido.");
                }

                dbContexto.Inventory.Update(inventory);

                pMovement.CreatedAt = DateTime.UtcNow;
                dbContexto.InventoryMovement.Add(pMovement);

                int result = await dbContexto.SaveChangesAsync();
                await transaction.CommitAsync();
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region "Consultas"

        /// <summary>
        /// Obtiene un movimiento de inventario específico por su identificador.
        /// </summary>
        /// <param name="pMovement">
        /// Objeto <see cref="InventoryMovement"/> con el <c>InventoryMovementId</c> a buscar.
        /// </param>
        /// <returns>El objeto <see cref="InventoryMovement"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<InventoryMovement?> ObtenerPorIdAsync(InventoryMovement pMovement)
        {
            if (pMovement.InventoryMovementId <= 0)
                throw new Exception("El ID de movimiento no es válido.");

            return await InventoryMovementDAL.ObtenerPorIdAsync(pMovement);
        }

        /// <summary>
        /// Obtiene una lista de movimientos de inventario aplicando filtros opcionales.
        /// </summary>
        /// <param name="pMovement">Objeto <see cref="InventoryMovement"/> usado como filtro.</param>
        /// <param name="pFromDate">Fecha de inicio del rango (null = sin límite inferior).</param>
        /// <param name="pToDate">Fecha de fin del rango (null = sin límite superior).</param>
        /// <returns>Lista de movimientos ordenados por fecha descendente.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<InventoryMovement>> ObtenerTodosAsync(
            InventoryMovement pMovement,
            DateTime? pFromDate = null,
            DateTime? pToDate = null)
        {
            return await InventoryMovementDAL.ObtenerTodosAsync(pMovement, pFromDate, pToDate);
        }

        /// <summary>
        /// Obtiene el historial completo de movimientos de un inventario específico.
        /// </summary>
        /// <param name="pInventoryId">Identificador del inventario a consultar.</param>
        /// <returns>Lista de movimientos del inventario, ordenados por fecha descendente.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<List<InventoryMovement>> ObtenerPorInventarioAsync(int pInventoryId)
        {
            if (pInventoryId <= 0)
                throw new Exception("El ID de inventario no es válido.");

            return await InventoryMovementDAL.ObtenerPorInventarioAsync(pInventoryId);
        }

        /// <summary>
        /// Obtiene todos los movimientos de inventario asociados a un documento específico.
        /// </summary>
        /// <param name="pDocumentId">Identificador del documento a consultar.</param>
        /// <returns>
        /// Lista de <see cref="InventoryMovement"/> asociados al documento,
        /// ordenados por fecha descendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<List<InventoryMovement>> ObtenerPorDocumentoAsync(int pDocumentId)
        {
            if (pDocumentId <= 0)
                throw new Exception("El ID de documento no es válido.");

            return await InventoryMovementDAL.ObtenerPorDocumentoAsync(pDocumentId);
        }

        /// <summary>
        /// Realiza una búsqueda avanzada de movimientos de inventario con soporte para paginación.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{InventoryMovement}"/> con los filtros y parámetros de paginación.
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