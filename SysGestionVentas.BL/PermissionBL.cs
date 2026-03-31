using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class PermissionBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Permission"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pPermission">Objeto <see cref="Permission"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(Permission pPermission)
        {
            var contexto = new ValidationContext(pPermission);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pPermission, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo permiso en el sistema.
        /// </summary>
        /// <param name="pPermission">Objeto <see cref="Permission"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<int> GuardarAsync(Permission pPermission)
        {
            ValidarEntidad(pPermission);
            return await PermissionDAL.GuardarAsync(pPermission);
        }

        /// <summary>
        /// Valida y modifica los datos de un permiso existente en el sistema.
        /// </summary>
        /// <param name="pPermission">
        /// Objeto <see cref="Permission"/> con el <c>PermissionId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el permiso no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(Permission pPermission)
        {
            ValidarEntidad(pPermission);
            return await PermissionDAL.ModificarAsync(pPermission);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un permiso, desactivándolo en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pPermission">
        /// Objeto <see cref="Permission"/> con el <c>PermissionId</c> del registro a desactivar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se desactivó correctamente.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido, si el permiso no existe, o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(Permission pPermission)
        {
            if (pPermission.PermissionId <= 0)
                throw new Exception("El ID de permiso no es válido.");

            return await PermissionDAL.EliminarAsync(pPermission);
        }

        /// <summary>
        /// Obtiene un permiso específico por su identificador.
        /// </summary>
        /// <param name="pPermission">Objeto <see cref="Permission"/> con el <c>PermissionId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Permission"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Permission?> ObtenerPorIdAsync(Permission pPermission)
        {
            if (pPermission.PermissionId <= 0)
                throw new Exception("El ID de permiso no es válido.");

            return await PermissionDAL.ObtenerPorIdAsync(pPermission);
        }

        /// <summary>
        /// Obtiene una lista de permisos aplicando filtros opcionales.
        /// </summary>
        /// <param name="pPermission">Objeto <see cref="Permission"/> usado como filtro de búsqueda.</param>
        /// <param name="pIsActive">
        /// Filtro de estado: <c>true</c> = solo activos, <c>false</c> = solo inactivos, <c>null</c> = todos.
        /// </param>
        /// <returns>Lista de objetos <see cref="Permission"/> ordenados por nombre de forma ascendente.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Permission>> ObtenerTodosAsync(Permission pPermission, bool? pIsActive = null)
        {
            return await PermissionDAL.ObtenerTodosAsync(pPermission, pIsActive);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de permisos con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Permission}"/> con los filtros y parámetros de paginación.
        /// </param>
        /// <param name="pIsActive">
        /// Filtro de estado: <c>true</c> = solo activos, <c>false</c> = solo inactivos, <c>null</c> = todos.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Permission}"/> con la lista de permisos encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.</exception>
        public static async Task<PagedResult<Permission>> BuscarAsync(PagedQuery<Permission> pPagedQuery, bool? pIsActive = null)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            var todos = await PermissionDAL.ObtenerTodosAsync(pPagedQuery.Filter, pIsActive);

            var items = todos
                .Skip(pPagedQuery.Skip)
                .Take(pPagedQuery.PageSize)
                .ToList();

            return new PagedResult<Permission>
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