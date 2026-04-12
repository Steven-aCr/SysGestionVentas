using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class ProductDiscountBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="ProductDiscount"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pProductDiscount">Objeto <see cref="ProductDiscount"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(ProductDiscount pProductDiscount)
        {
            var contexto = new ValidationContext(pProductDiscount);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pProductDiscount, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra una nueva asignación de descuento a un producto.
        /// Verifica que el producto, el descuento y el usuario asignador sean válidos,
        /// y que la asignación no exista previamente como activa en la capa DAL.
        /// </summary>
        /// <param name="pProductDiscount">
        /// Entidad <see cref="ProductDiscount"/> con <c>ProductId</c>, <c>DiscountId</c>
        /// y <c>AssignedByUser</c> requeridos.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si los IDs no son válidos, si la asignación ya existe,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> GuardarAsync(ProductDiscount pProductDiscount)
        {
            if (pProductDiscount.ProductId <= 0)
                throw new Exception("El ID de producto no es válido.");

            if (pProductDiscount.DiscountId <= 0)
                throw new Exception("El ID de descuento no es válido.");

            if (pProductDiscount.AssignedByUser <= 0)
                throw new Exception("El ID del usuario asignador no es válido.");

            ValidarEntidad(pProductDiscount);
            return await ProductDiscountDAL.GuardarAsync(pProductDiscount);
        }

        /// <summary>
        /// Realiza la eliminación lógica de una asignación de descuento a un producto,
        /// marcando la asignación como inactiva. No elimina el registro físicamente.
        /// </summary>
        /// <param name="pProductDiscount">
        /// Entidad <see cref="ProductDiscount"/> con el <c>ProductId</c> y <c>DiscountId</c>
        /// de la asignación a desactivar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se desactivó correctamente.</returns>
        /// <exception cref="Exception">
        /// Se lanza si los IDs no son válidos, si la asignación no existe,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> EliminarAsync(ProductDiscount pProductDiscount)
        {
            if (pProductDiscount.ProductId <= 0)
                throw new Exception("El ID de producto no es válido.");

            if (pProductDiscount.DiscountId <= 0)
                throw new Exception("El ID de descuento no es válido.");

            return await ProductDiscountDAL.EliminarAsync(pProductDiscount);
        }

        /// <summary>
        /// Obtiene la lista de descuentos activos asignados a un producto específico.
        /// </summary>
        /// <param name="pProductId">Identificador del producto a consultar. Debe ser mayor a 0.</param>
        /// <returns>
        /// Lista de <see cref="ProductDiscount"/> con las propiedades de navegación
        /// <c>Product</c> y <c>Discount</c> cargadas.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el ID no es válido o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<List<ProductDiscount>> ObtenerPorProductoAsync(int pProductId)
        {
            if (pProductId <= 0)
                throw new Exception("El ID de producto no es válido.");

            return await ProductDiscountDAL.ObtenerPorProductoAsync(pProductId);
        }

        /// <summary>
        /// Obtiene todos los registros activos de asignaciones entre productos y descuentos.
        /// </summary>
        /// <returns>
        /// Lista completa de <see cref="ProductDiscount"/> activos con propiedades
        /// de navegación <c>Product</c> y <c>Discount</c> cargadas.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<ProductDiscount>> ObtenerTodosAsync()
        {
            return await ProductDiscountDAL.ObtenerTodosAsync();
        }

        #endregion
    }
}