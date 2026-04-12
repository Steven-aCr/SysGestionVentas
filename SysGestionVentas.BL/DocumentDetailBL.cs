using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="DocumentDetail"/>.
    /// Centraliza el cálculo de subtotales, impuestos y totales de línea,
    /// y delega la persistencia a <see cref="DocumentDetailDAL"/>.
    /// </summary>
    public class DocumentDetailBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="DocumentDetail"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pDetail">Objeto <see cref="DocumentDetail"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// </exception>
        private static void ValidarEntidad(DocumentDetail pDetail)
        {
            var contexto = new ValidationContext(pDetail);
            var resultados = new List<ValidationResult>();
            bool esValido = Validator.TryValidateObject(pDetail, contexto, resultados, validateAllProperties: true);
            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        /// <summary>
        /// Calcula y asigna los campos derivados de una línea de detalle:
        /// <c>Subtotal</c>, <c>TaxAmount</c> y <c>TotalAmount</c>.
        /// La fórmula aplicada es:
        /// <list type="bullet">
        ///   <item><description>Subtotal = (Quantity × UnitPrice) - DiscountAmount</description></item>
        ///   <item><description>TaxAmount = Subtotal × (TaxPercentage / 100)</description></item>
        ///   <item><description>TotalAmount = Subtotal + TaxAmount</description></item>
        /// </list>
        /// </summary>
        /// <param name="pDetail">Objeto <see cref="DocumentDetail"/> al que se le calcularán los montos.</param>
        private static void CalcularMontos(DocumentDetail pDetail)
        {
            pDetail.Subtotal = (pDetail.Quantity * pDetail.UnitPrice) - pDetail.DiscountAmount;
            if (pDetail.Subtotal < 0) pDetail.Subtotal = 0;
            pDetail.TaxAmount = Math.Round(pDetail.Subtotal * (pDetail.TaxPercentage / 100), 2);
            pDetail.TotalAmount = pDetail.Subtotal + pDetail.TaxAmount;
        }

        /// <summary>
        /// Determina el identificador del tipo de movimiento de inventario correspondiente
        /// al tipo de documento indicado, consultando el <c>DefaultMovementTypeId</c>
        /// configurado en <see cref="DocumentType"/>.
        /// </summary>
        /// <param name="pDocumentId">Identificador del documento padre.</param>
        /// <param name="pDbContexto">Contexto de base de datos activo.</param>
        /// <returns>
        /// El <c>MovementTypeId</c> configurado para el tipo de documento.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el documento no existe, si el tipo de documento no tiene
        /// un movimiento por defecto configurado, o si no existe inventario para el producto.
        /// </exception>
        private static async Task<int> ResolverTipoMovimientoAsync(
            int pDocumentId, DbContexto pDbContexto)
        {
            var document = await pDbContexto.Document
                .Include(d => d.DocumentType)
                .FirstOrDefaultAsync(d => d.DocumentId == pDocumentId)
                ?? throw new Exception($"No se encontró el documento con ID {pDocumentId}.");

            if (document.DocumentType?.DefaultMovementTypeId == null)
                throw new Exception(
                    $"El tipo de documento '{document.DocumentType?.Name}' " +
                    $"no tiene un tipo de movimiento configurado.");

            return document.DocumentType.DefaultMovementTypeId.Value;
        }

        /// <summary>
        /// Recalcula y actualiza el <c>TotalAmount</c> del documento padre
        /// sumando los totales de todas sus líneas de detalle dentro de una transacción activa.
        /// No llama a <c>SaveChangesAsync</c>; esa responsabilidad recae en el llamador.
        /// </summary>
        /// <param name="pDocumentId">Identificador del documento a recalcular.</param>
        /// <param name="pDbContexto">Contexto de base de datos activo con transacción abierta.</param>
        /// <exception cref="Exception">Se lanza si el documento no existe.</exception>
        private static async Task RecalcularTotalDocumentoAsync(int pDocumentId, DbContexto pDbContexto)
        {
            var document = await pDbContexto.Document
                .FirstOrDefaultAsync(d => d.DocumentId == pDocumentId)
                ?? throw new Exception($"No se encontró el documento con ID {pDocumentId}.");

            var detalles = await pDbContexto.DocumentDetail
                .Where(d => d.DocumentId == pDocumentId)
                .ToListAsync();

            document.TotalAmount = detalles.Sum(d => d.TotalAmount);
            pDbContexto.Document.Update(document);
        }
        #endregion

        #region "CRUD"

        /// <summary>
        /// Orquesta de forma transaccional el registro de una nueva línea de detalle,
        /// la generación automática del movimiento de inventario correspondiente,
        /// la actualización del stock y el recálculo del total del documento padre.
        /// </summary>
        /// <param name="pDetail">Objeto <see cref="DocumentDetail"/> con los datos a guardar.</param>
        /// <param name="pCreatedByUser">Identificador del usuario que ejecuta la operación.</param>
        /// <returns>Número de filas afectadas.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el stock es insuficiente, si el tipo de documento no tiene movimiento
        /// configurado, o si ocurre un error durante la transacción.
        /// </exception>
        public static async Task<int> GuardarAsync(DocumentDetail pDetail, int pCreatedByUser)
        {
            if (pDetail.DocumentId <= 0)
                throw new Exception("El ID de documento no es válido.");

            if (pDetail.ProductId <= 0)
                throw new Exception("El ID de producto no es válido.");

            if (pCreatedByUser <= 0)
                throw new Exception("El ID del usuario es obligatorio.");

            CalcularMontos(pDetail);
            ValidarEntidad(pDetail);

            using var dbContexto = new DbContexto();
            using var transaction = await dbContexto.Database.BeginTransactionAsync();

            try
            {
                // 1 — Guardar el detalle
                await DocumentDetailDAL.GuardarEnTransaccionAsync(pDetail, dbContexto);
                await dbContexto.SaveChangesAsync(); // genera el DocDetailId

                // 2 — Resolver inventario a partir del ProductId
                var inventory = await InventoryDAL.ObtenerPorProductoEnTransaccionAsync(
                    pDetail.ProductId, dbContexto)
                    ?? throw new Exception(
                        $"No existe un inventario registrado para el producto con ID {pDetail.ProductId}.");

                // 3 — Resolver tipo de movimiento según el tipo de documento
                int movementTypeId = await ResolverTipoMovimientoAsync(pDetail.DocumentId, dbContexto);

                // 4 — Validar y actualizar stock
                switch (movementTypeId)
                {
                    case 1: // Entrada
                    case 4: // Devolución
                        inventory.CurrentStock += pDetail.Quantity;
                        break;

                    case 2: // Salida
                    case 5: // Transferencia
                        if (inventory.CurrentStock < pDetail.Quantity)
                            throw new Exception(
                                $"Stock insuficiente. Disponible: {inventory.CurrentStock}, " +
                                $"requerido: {pDetail.Quantity}.");
                        inventory.CurrentStock -= pDetail.Quantity;
                        break;

                    case 3: // Ajuste
                        inventory.CurrentStock = pDetail.Quantity;
                        break;

                    default:
                        throw new Exception("Tipo de movimiento no reconocido.");
                }

                InventoryDAL.ActualizarStockEnTransaccion(inventory, dbContexto);

                // 5 — Generar movimiento de inventario vinculado al detalle
                var movement = new InventoryMovement
                {
                    MovementTypeId = movementTypeId,
                    Quantity = pDetail.Quantity,
                    UnitCost = pDetail.UnitPrice,
                    InventoryId = inventory.InventoryId,
                    CreatedByUser = pCreatedByUser,
                    DocumentDetailId = pDetail.DocDetailId,
                    Notes = $"Generado automáticamente desde documento ID {pDetail.DocumentId}."
                };

                await InventoryMovementDAL.RegistrarMovimientoEnTransaccionAsync(movement, dbContexto);

                // 6 — Recalcular total del documento padre
                await RecalcularTotalDocumentoAsync(pDetail.DocumentId, dbContexto);

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

        /// <summary>
        /// Recalcula los montos de línea, valida y modifica un detalle de documento existente.
        /// Solo se permite modificar detalles de documentos en estado editable.
        /// La validación del estado del documento padre es responsabilidad de la capa controlador.
        /// </summary>
        /// <param name="pDetail">
        /// Objeto <see cref="DocumentDetail"/> con el <c>DocDetailId</c> del registro a modificar
        /// y los nuevos valores.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el detalle no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(DocumentDetail pDetail)
        {
            if (pDetail.DocDetailId <= 0)
                throw new Exception("El ID de detalle no es válido.");

            CalcularMontos(pDetail);
            ValidarEntidad(pDetail);
            return await DocumentDetailDAL.ModificarAsync(pDetail);
        }

        /// <summary>
        /// Elimina físicamente una línea de detalle de documento y revierte el movimiento
        /// de inventario asociado, restaurando el stock afectado dentro de una transacción.
        /// </summary>
        /// <param name="pDetail">Objeto <see cref="DocumentDetail"/> con el <c>DocDetailId</c> del registro a eliminar.</param>
        /// <param name="pCreatedByUser">Identificador del usuario que ejecuta la operación.</param>
        /// <returns>Número de filas afectadas.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el detalle no existe, si no hay inventario asociado,
        /// o si ocurre un error durante la transacción.
        /// </exception>
        public static async Task<int> EliminarAsync(DocumentDetail pDetail, int pCreatedByUser)
        {
            if (pDetail.DocDetailId <= 0)
                throw new Exception("El ID de detalle no es válido.");

            if (pCreatedByUser <= 0)
                throw new Exception("El ID del usuario es obligatorio.");

            using var dbContexto = new DbContexto();
            using var transaction = await dbContexto.Database.BeginTransactionAsync();

            try
            {
                // 1 — Obtener el detalle con su movimiento asociado
                var detail = await dbContexto.DocumentDetail
                    .FirstOrDefaultAsync(d => d.DocDetailId == pDetail.DocDetailId)
                    ?? throw new Exception($"No se encontró el detalle con ID {pDetail.DocDetailId}.");

                // 2 — Obtener el movimiento vinculado al detalle
                var movement = await dbContexto.InventoryMovement
                    .FirstOrDefaultAsync(im => im.DocumentDetailId == pDetail.DocDetailId);

                if (movement != null)
                {
                    var inventory = await InventoryDAL.ObtenerPorProductoEnTransaccionAsync(
                        detail.ProductId, dbContexto)
                        ?? throw new Exception(
                            $"No existe inventario para el producto con ID {detail.ProductId}.");

                    // 3 — Revertir el stock según el tipo de movimiento original
                    switch (movement.MovementTypeId)
                    {
                        case 1: // Entrada → revertir restando
                        case 4:
                            if (inventory.CurrentStock < detail.Quantity)
                                throw new Exception("No es posible revertir: el stock resultante sería negativo.");
                            inventory.CurrentStock -= detail.Quantity;
                            break;

                        case 2: // Salida → revertir sumando
                        case 5:
                            inventory.CurrentStock += detail.Quantity;
                            break;

                        case 3: // Ajuste → no se revierte automáticamente
                            break;
                    }

                    InventoryDAL.ActualizarStockEnTransaccion(inventory, dbContexto);

                    // 4 — Registrar movimiento de reversión
                    var reversal = new InventoryMovement
                    {
                        MovementTypeId = movement.MovementTypeId == 2 ? 1 : 2,
                        Quantity = detail.Quantity,
                        UnitCost = detail.UnitPrice,
                        InventoryId = inventory.InventoryId,
                        CreatedByUser = pCreatedByUser,
                        DocumentDetailId = null,
                        Notes = $"Reversión por eliminación de detalle ID {pDetail.DocDetailId}."
                    };

                    await InventoryMovementDAL.RegistrarMovimientoEnTransaccionAsync(reversal, dbContexto);
                }

                // 5 — Eliminar el detalle
                dbContexto.DocumentDetail.Remove(detail);

                // 6 — Recalcular total del documento
                await RecalcularTotalDocumentoAsync(detail.DocumentId, dbContexto);

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

        /// <summary>
        /// Obtiene un detalle de documento específico por su identificador.
        /// </summary>
        /// <param name="pDetail">Objeto <see cref="DocumentDetail"/> con el <c>DocDetailId</c> a buscar.</param>
        /// <returns>El objeto <see cref="DocumentDetail"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<DocumentDetail?> ObtenerPorIdAsync(DocumentDetail pDetail)
        {
            if (pDetail.DocDetailId <= 0)
                throw new Exception("El ID de detalle no es válido.");

            return await DocumentDetailDAL.ObtenerPorIdAsync(pDetail);
        }

        /// <summary>
        /// Obtiene todos los detalles asociados a un documento específico.
        /// </summary>
        /// <param name="pDocumentId">Identificador del documento padre.</param>
        /// <returns>Lista de objetos <see cref="DocumentDetail"/> del documento indicado.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<List<DocumentDetail>> ObtenerPorDocumentoAsync(int pDocumentId)
        {
            if (pDocumentId <= 0)
                throw new Exception("El ID de documento no es válido.");

            return await DocumentDetailDAL.ObtenerPorDocumentoAsync(pDocumentId);
        }

        /// <summary>
        /// Obtiene una lista de detalles de documentos aplicando filtros opcionales.
        /// </summary>
        /// <param name="pDetail">Objeto <see cref="DocumentDetail"/> usado como filtro.</param>
        /// <returns>Lista de objetos <see cref="DocumentDetail"/> ordenados por documento y línea.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<DocumentDetail>> ObtenerTodosAsync(DocumentDetail pDetail)
        {
            return await DocumentDetailDAL.ObtenerTodosAsync(pDetail);
        }

        #endregion
    }
}