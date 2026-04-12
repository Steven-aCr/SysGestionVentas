using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class CategoryBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Category"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pCategory">Objeto <see cref="Category"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(Category pCategory)
        {
            var contexto = new ValidationContext(pCategory);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pCategory, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra una nueva categoría en el sistema.
        /// </summary>
        /// <param name="pCategory">Objeto <see cref="Category"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<int> GuardarAsync(Category pCategory)
        {
            ValidarEntidad(pCategory);
            return await CategoryDAL.GuardarAsync(pCategory);
        }

        /// <summary>
        /// Valida y modifica los datos de una categoría existente en el sistema.
        /// </summary>
        /// <param name="pCategory">
        /// Objeto <see cref="Category"/> con el <c>CategoryId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si la categoría no existe o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> ModificarAsync(Category pCategory)
        {
            if (pCategory.CategoryId <= 0)
                throw new Exception("El ID de categoría no es válido.");

            ValidarEntidad(pCategory);
            return await CategoryDAL.ModificarAsync(pCategory);
        }

        /// <summary>
        /// Realiza la eliminación lógica de una categoría cambiando su estado en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pCategory">
        /// Objeto <see cref="Category"/> con el <c>CategoryId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el ID no es válido, si la categoría no existe,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> EliminarAsync(Category pCategory)
        {
            if (pCategory.CategoryId <= 0)
                throw new Exception("El ID de categoría no es válido.");

            if (pCategory.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await CategoryDAL.EliminarAsync(pCategory);
        }

        /// <summary>
        /// Obtiene una categoría específica por su identificador, incluyendo
        /// sus relaciones con <see cref="Status"/> y el <see cref="User"/> creador.
        /// </summary>
        /// <param name="pCategory">Objeto <see cref="Category"/> con el <c>CategoryId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Category"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Category?> ObtenerPorIdAsync(Category pCategory)
        {
            if (pCategory.CategoryId <= 0)
                throw new Exception("El ID de categoría no es válido.");

            return await CategoryDAL.ObtenerPorIdAsync(pCategory);
        }

        /// <summary>
        /// Obtiene una lista de categorías aplicando filtros opcionales.
        /// </summary>
        /// <param name="pCategory">Objeto <see cref="Category"/> usado como filtro de búsqueda.</param>
        /// <returns>Lista de objetos <see cref="Category"/> ordenados por nombre de forma ascendente.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Category>> ObtenerTodosAsync(Category pCategory)
        {
            return await CategoryDAL.ObtenerTodosAsync(pCategory);
        }

        #endregion
    }
}