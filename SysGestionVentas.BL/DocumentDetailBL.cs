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

        #endregion

        #region "CRUD"

        /// <summary>
        /// Calcula los montos de línea, valida y registra un nuevo detalle de documento.
        /// </summary>
        /// <param name="pDetail">Objeto <see cref="DocumentDetail"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<int> GuardarAsync(DocumentDetail pDetail)
        {
            if (pDetail.DocumentId <= 0)
                throw new Exception("El ID de documento no es válido.");

            if (pDetail.ProductId <= 0)
                throw new Exception("El ID de producto no es válido.");

            CalcularMontos(pDetail);
            ValidarEntidad(pDetail);
            return await DocumentDetailDAL.GuardarAsync(pDetail);
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
        /// Elimina físicamente un detalle de documento.
        /// Solo se permite para documentos en estado editable (borrador/pendiente).
        /// La validación del estado del documento padre es responsabilidad de la capa controlador.
        /// </summary>
        /// <param name="pDetail">
        /// Objeto <see cref="DocumentDetail"/> con el <c>DocDetailId</c> del registro a eliminar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se eliminó correctamente.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido, el detalle no existe, o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(DocumentDetail pDetail)
        {
            if (pDetail.DocDetailId <= 0)
                throw new Exception("El ID de detalle no es válido.");

            return await DocumentDetailDAL.EliminarAsync(pDetail);
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