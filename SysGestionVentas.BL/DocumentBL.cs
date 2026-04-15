using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using SysGestionVentas.EN.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="Document"/>.
    /// Orquesta validaciones, reglas del ciclo de vida documental y delega
    /// la persistencia a <see cref="DocumentDAL"/>.
    /// </summary>
    public class DocumentBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Document"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pDocument">Objeto <see cref="Document"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// </exception>
        private static void ValidarEntidad(Document pDocument)
        {
            var contexto = new ValidationContext(pDocument);
            var resultados = new List<ValidationResult>();
            bool esValido = Validator.TryValidateObject(pDocument, contexto, resultados, validateAllProperties: true);
            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo documento en el sistema.
        /// </summary>
        /// <param name="pDocument">Objeto <see cref="Document"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<int> GuardarAsync(Document pDocument)
        {
            ValidarEntidad(pDocument);
            return await DocumentDAL.GuardarAsync(pDocument);
        }

        /// <summary>
        /// Valida y modifica los datos editables de un documento existente.
        /// Los campos <c>DocNumber</c>, <c>DocTypeId</c> y <c>CreatedByUser</c>
        /// no son modificables tras la emisión del documento.
        /// </summary>
        /// <param name="pDocument">
        /// Objeto <see cref="Document"/> con el <c>DocumentId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el documento no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(Document pDocument)
        {
            if (pDocument.DocumentId <= 0)
                throw new Exception("El ID de documento no es válido.");

            ValidarEntidad(pDocument);
            return await DocumentDAL.ModificarAsync(pDocument);
        }

        /// <summary>
        /// Realiza la eliminación lógica (anulación) de un documento, cambiando su estado.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pDocument">
        /// Objeto <see cref="Document"/> con el <c>DocumentId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado "Anulado".
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se anuló correctamente.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido, el documento no existe, o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(Document pDocument)
        {
            if (pDocument.DocumentId <= 0)
                throw new Exception("El ID de documento no es válido.");

            if (pDocument.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la anulación del documento.");

            return await DocumentDAL.EliminarAsync(pDocument);
        }

        /// <summary>
        /// Obtiene un documento específico por su identificador, incluyendo sus relaciones
        /// con <see cref="DocumentType"/>, <see cref="Person"/>, <see cref="Status"/>
        /// y el <see cref="User"/> que lo creó.
        /// </summary>
        /// <param name="pDocument">Objeto <see cref="Document"/> con el <c>DocumentId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Document"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Document?> ObtenerPorIdAsync(Document pDocument)
        {
            if (pDocument.DocumentId <= 0)
                throw new Exception("El ID de documento no es válido.");

            return await DocumentDAL.ObtenerPorIdAsync(pDocument);
        }

        /// <summary>
        /// Obtiene una lista de documentos aplicando filtros opcionales.
        /// </summary>
        /// <param name="pDocument">Objeto <see cref="Document"/> usado como filtro de búsqueda.</param>
        /// <returns>Lista de objetos <see cref="Document"/> ordenados por fecha de emisión descendente.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Document>> ObtenerTodosAsync(Document pDocument)
        {
            return await DocumentDAL.ObtenerTodosAsync(pDocument);
        }

        #endregion

        #region "Creación Transaccional con Cliente y Detalles"

        /// <summary>
        /// Crea de forma atómica, en una única transacción de base de datos:
        /// <list type="number">
        ///   <item>Una nueva <see cref="Person"/> con los datos del cliente.</item>
        ///   <item>Un nuevo registro <see cref="Client"/> vinculado a esa persona.</item>
        ///   <item>El encabezado del <see cref="Document"/>.</item>
        ///   <item>Todas las líneas de <see cref="DocumentDetail"/> con cálculo de montos.</item>
        ///   <item>Los <see cref="InventoryMovement"/> correspondientes con actualización de stock.</item>
        ///   <item>El recálculo del <c>TotalAmount</c> del documento.</item>
        /// </list>
        /// Si cualquier paso falla, la transacción completa se revierte garantizando
        /// la integridad referencial de los datos.
        /// </summary>
        /// <param name="pModel">
        /// ViewModel con los datos del cliente (persona), el encabezado del documento
        /// y la colección de líneas de detalle. Debe contener al menos una línea válida.
        /// </param>
        /// <returns>
        /// El <see cref="Document"/> persistido con su <c>DocumentId</c> generado,
        /// para ser utilizado en la redirección post-confirmación.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el modelo no tiene detalles, si el DUI o teléfono ya están registrados,
        /// si algún producto no tiene inventario registrado, si el stock es insuficiente
        /// para una salida, o si el tipo de documento no tiene un tipo de movimiento configurado.
        /// </exception>
        public static async Task<Document> CrearConDetallesAsync(CreateDocumentModel pModel)
        {
            if (pModel.Detalles == null || pModel.Detalles.Count == 0)
                throw new Exception("Debe agregar al menos un producto al documento.");

            using var dbContexto = new DbContexto();
            using var transaction = await dbContexto.Database.BeginTransactionAsync();

            try
            {
                // ── 1. Crear la Persona del cliente ───────────────────────────────────
                var person = new Person
                {
                    FirstName = pModel.FirstName,
                    LastName = pModel.LastName,
                    Adress = pModel.Adress,
                    PhoneNumber = pModel.PhoneNumber,
                    Dui = string.IsNullOrWhiteSpace(pModel.Dui) ? null : pModel.Dui,
                    StatusId = pModel.PersonStatusId
                };

                // Valida unicidad de DUI y teléfono antes de agregar
                await PersonDAL.GuardarEnTransaccionAsync(person, dbContexto);
                await dbContexto.SaveChangesAsync(); // genera PersonId

                // ── 2. Crear el registro Client vinculado a la Persona ────────────────
                var client = new Client
                {
                    PersonId = person.PersonId
                };

                dbContexto.Client.Add(client);
                await dbContexto.SaveChangesAsync(); // genera ClientId

                // ── 3. Persistir el encabezado del documento ──────────────────────────
                var document = new Document
                {
                    DocTypeId = pModel.DocTypeId,
                    DocNumber = pModel.DocNumber,
                    IssueDate = pModel.IssueDate,
                    PersonId = person.PersonId,   // FK a la persona recién creada
                    StatusId = pModel.StatusId,
                    CreatedByUser = pModel.CreatedByUser,
                    TotalAmount = 0                  // se recalcula al final
                };

                dbContexto.Document.Add(document);
                await dbContexto.SaveChangesAsync(); // genera DocumentId

                // ── 4. Resolver el tipo de movimiento desde el tipo de documento ──────
                var docType = await dbContexto.DocumentType
                    .FirstOrDefaultAsync(dt => dt.DocTypeId == pModel.DocTypeId)
                    ?? throw new Exception("No se encontró el tipo de documento seleccionado.");

                if (docType.DefaultMovementTypeId == null)
                    throw new Exception(
                        $"El tipo de documento '{docType.Name}' no tiene un " +
                        $"tipo de movimiento de inventario configurado.");

                int movementTypeId = docType.DefaultMovementTypeId.Value;

                // ── 5. Procesar cada línea de detalle ─────────────────────────────────
                foreach (var linea in pModel.Detalles)
                {
                    // 5a — Calcular montos de la línea
                    var detail = new DocumentDetail
                    {
                        DocumentId = document.DocumentId,
                        ProductId = linea.ProductId,
                        Quantity = linea.Quantity,
                        UnitPrice = linea.UnitPrice,
                        DiscountAmount = linea.DiscountAmount,
                        TaxPercentage = linea.TaxPercentage,
                        Notes = linea.Notes
                    };

                    detail.Subtotal = Math.Max((detail.Quantity * detail.UnitPrice) - detail.DiscountAmount, 0);
                    detail.TaxAmount = Math.Round(detail.Subtotal * (detail.TaxPercentage / 100), 2);
                    detail.TotalAmount = detail.Subtotal + detail.TaxAmount;

                    dbContexto.DocumentDetail.Add(detail);
                    await dbContexto.SaveChangesAsync(); // genera DocDetailId

                    // 5b — Obtener inventario del producto
                    var inventory = await dbContexto.Inventory
                        .FirstOrDefaultAsync(i => i.ProductId == linea.ProductId)
                        ?? throw new Exception(
                            $"El producto '{linea.ProductName ?? linea.ProductId.ToString()}' " +
                            $"no tiene inventario registrado.");

                    // 5c — Validar y actualizar stock según tipo de movimiento
                    switch (movementTypeId)
                    {
                        case 1: // Entrada
                        case 4: // Devolución
                            inventory.CurrentStock += detail.Quantity;
                            break;

                        case 2: // Salida
                        case 5: // Transferencia
                            if (inventory.CurrentStock < detail.Quantity)
                                throw new Exception(
                                    $"Stock insuficiente para '{linea.ProductName ?? linea.ProductId.ToString()}'. " +
                                    $"Disponible: {inventory.CurrentStock}, " +
                                    $"solicitado: {detail.Quantity}.");
                            inventory.CurrentStock -= detail.Quantity;
                            break;

                        case 3: // Ajuste
                            inventory.CurrentStock = detail.Quantity;
                            break;

                        default:
                            throw new Exception("Tipo de movimiento de inventario no reconocido.");
                    }

                    dbContexto.Inventory.Update(inventory);

                    // 5d — Registrar movimiento de inventario vinculado al detalle
                    var movement = new InventoryMovement
                    {
                        MovementTypeId = movementTypeId,
                        Quantity = detail.Quantity,
                        UnitCost = detail.UnitPrice,
                        InventoryId = inventory.InventoryId,
                        CreatedByUser = pModel.CreatedByUser,
                        DocumentDetailId = detail.DocDetailId,
                        Notes = $"Generado desde documento {document.DocNumber}."
                    };

                    dbContexto.InventoryMovement.Add(movement);
                }

                // ── 6. Recalcular total del documento ────────────────────────────────
                var totalDetalles = await dbContexto.DocumentDetail
                    .Where(d => d.DocumentId == document.DocumentId)
                    .SumAsync(d => d.TotalAmount);

                document.TotalAmount = totalDetalles;
                dbContexto.Document.Update(document);

                await dbContexto.SaveChangesAsync();
                await transaction.CommitAsync();

                return document;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de documentos con soporte para paginación.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Document}"/> con los filtros y parámetros de paginación.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Document}"/> con la lista de documentos encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.</exception>
        public static async Task<PagedResult<Document>> BuscarAsync(PagedQuery<Document> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            return await DocumentDAL.BuscarAsync(pPagedQuery);
        }

        #endregion
    }
}