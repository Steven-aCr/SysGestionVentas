using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class ProductListBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="ProductList"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pProduct">Objeto <see cref="ProductList"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(ProductList pProduct)
        {
            var contexto = new ValidationContext(pProduct);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pProduct, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo producto en el sistema.
        /// Verifica unicidad del código de barras en la capa DAL.
        /// </summary>
        /// <param name="pProduct">Objeto <see cref="ProductList"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el código de barras ya existe o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> GuardarAsync(ProductList pProduct)
        {
            ValidarEntidad(pProduct);
            return await ProductListDAL.GuardarAsync(pProduct);
        }

        /// <summary>
        /// Valida y modifica los datos de un producto existente en el sistema.
        /// Verifica unicidad del código de barras en la capa DAL.
        /// El campo <c>CreatedByUser</c> no es modificable por ser de auditoría de creación.
        /// </summary>
        /// <param name="pProduct">
        /// Objeto <see cref="ProductList"/> con el <c>ProductId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el producto no existe, si el código de barras está duplicado,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> ModificarAsync(ProductList pProduct)
        {
            if (pProduct.ProductId <= 0)
                throw new Exception("El ID de producto no es válido.");

            ValidarEntidad(pProduct);
            return await ProductListDAL.ModificarAsync(pProduct);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un producto cambiando su estado en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pProduct">
        /// Objeto <see cref="ProductList"/> con el <c>ProductId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el ID no es válido, si el producto no existe,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> EliminarAsync(ProductList pProduct)
        {
            if (pProduct.ProductId <= 0)
                throw new Exception("El ID de producto no es válido.");

            if (pProduct.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await ProductListDAL.EliminarAsync(pProduct);
        }

        /// <summary>
        /// Obtiene un producto específico por su identificador, incluyendo sus relaciones
        /// con <see cref="Category"/>, <see cref="Status"/>, el <see cref="User"/> creador
        /// y su <see cref="Inventory"/> asociado.
        /// </summary>
        /// <param name="pProduct">Objeto <see cref="ProductList"/> con el <c>ProductId</c> a buscar.</param>
        /// <returns>El objeto <see cref="ProductList"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<ProductList?> ObtenerPorIdAsync(ProductList pProduct)
        {
            if (pProduct.ProductId <= 0)
                throw new Exception("El ID de producto no es válido.");

            return await ProductListDAL.ObtenerPorIdAsync(pProduct);
        }

        /// <summary>
        /// Obtiene una lista de productos aplicando filtros opcionales.
        /// </summary>
        /// <param name="pProduct">Objeto <see cref="ProductList"/> usado como filtro de búsqueda.</param>
        /// <returns>Lista de objetos <see cref="ProductList"/> ordenados por nombre de forma ascendente.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<ProductList>> ObtenerTodosAsync(ProductList pProduct)
        {
            return await ProductListDAL.ObtenerTodosAsync(pProduct);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de productos con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{ProductList}"/> con los filtros y parámetros de paginación.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{ProductList}"/> con la lista de productos encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">
        /// Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<PagedResult<ProductList>> BuscarAsync(PagedQuery<ProductList> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            return await ProductListDAL.BuscarAsync(pPagedQuery);
        }

        #endregion
    }
}