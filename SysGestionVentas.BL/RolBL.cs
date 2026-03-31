using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class RolBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Rol"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pRol">Objeto <see cref="Rol"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(Rol pRol)
        {
            var contexto = new ValidationContext(pRol);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pRol, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo rol en el sistema.
        /// </summary>
        /// <param name="pRol">Objeto <see cref="Rol"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<int> GuardarAsync(Rol pRol)
        {
            ValidarEntidad(pRol);
            return await RolDAL.GuardarAsync(pRol);
        }

        /// <summary>
        /// Valida y modifica los datos de un rol existente en el sistema.
        /// </summary>
        /// <param name="pRol">
        /// Objeto <see cref="Rol"/> con el <c>RolId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el rol no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(Rol pRol)
        {
            ValidarEntidad(pRol);
            return await RolDAL.ModificarAsync(pRol);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un rol, cambiando su estado en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pRol">
        /// Objeto <see cref="Rol"/> con el <c>RolId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido, si el rol no existe, o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(Rol pRol)
        {
            if (pRol.RolId <= 0)
                throw new Exception("El ID de rol no es válido.");

            if (pRol.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await RolDAL.EliminarAsync(pRol);
        }

        /// <summary>
        /// Obtiene un rol específico por su identificador, incluyendo
        /// su relación con <see cref="Status"/>.
        /// </summary>
        /// <param name="pRol">Objeto <see cref="Rol"/> con el <c>RolId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Rol"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Rol?> ObtenerPorIdAsync(Rol pRol)
        {
            if (pRol.RolId <= 0)
                throw new Exception("El ID de rol no es válido.");

            return await RolDAL.ObtenerPorIdAsync(pRol);
        }

        /// <summary>
        /// Obtiene una lista de roles aplicando filtros opcionales.
        /// </summary>
        /// <param name="pRol">Objeto <see cref="Rol"/> usado como filtro de búsqueda.</param>
        /// <returns>Lista de objetos <see cref="Rol"/> ordenados por nombre de forma ascendente.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Rol>> ObtenerTodosAsync(Rol pRol)
        {
            return await RolDAL.ObtenerTodosAsync(pRol);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de roles con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Rol}"/> con los filtros y parámetros de paginación.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Rol}"/> con la lista de roles encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.</exception>
        public static async Task<PagedResult<Rol>> BuscarAsync(PagedQuery<Rol> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            // RolDAL no expone BuscarAsync con paginación aún;
            // se delega a ObtenerTodosAsync aplicando paginación en memoria
            // hasta que el DAL sea extendido.
            var todos = await RolDAL.ObtenerTodosAsync(pPagedQuery.Filter);

            var items = todos
                .Skip(pPagedQuery.Skip)
                .Take(pPagedQuery.PageSize)
                .ToList();

            return new PagedResult<Rol>
            {
                Items = items,
                TotalCount = todos.Count,
                CurrentPage = pPagedQuery.Page,
                PageSize = pPagedQuery.PageSize
            };
        }

        #endregion
    }
}