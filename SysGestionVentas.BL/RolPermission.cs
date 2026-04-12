using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class RolPermissionBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="RolPermission"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pRolPermission">Objeto <see cref="RolPermission"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(RolPermission pRolPermission)
        {
            var contexto = new ValidationContext(pRolPermission);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pRolPermission, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra una nueva asignación de permiso a un rol en el sistema.
        /// Verifica que tanto el <c>RolId</c> como el <c>PermissionId</c> sean válidos
        /// antes de persistir.
        /// </summary>
        /// <param name="pRolPermission">
        /// Objeto <see cref="RolPermission"/> con <c>RolId</c>, <c>PermissionId</c>
        /// y <c>AssignedByUser</c> requeridos.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si la asignación ya existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> GuardarAsync(RolPermission pRolPermission)
        {
            if (pRolPermission.RolId <= 0)
                throw new Exception("El ID de rol no es válido.");

            if (pRolPermission.PermissionId <= 0)
                throw new Exception("El ID de permiso no es válido.");

            if (pRolPermission.AssignedByUser <= 0)
                throw new Exception("El ID del usuario asignador no es válido.");

            ValidarEntidad(pRolPermission);
            return await RolPermissionDAL.GuardarAsync(pRolPermission);
        }

        /// <summary>
        /// Realiza la eliminación lógica de una asignación de permiso a un rol,
        /// marcando la asignación como inactiva. No elimina el registro físicamente.
        /// </summary>
        /// <param name="pRolPermission">
        /// Objeto <see cref="RolPermission"/> con el <c>RolId</c> y <c>PermissionId</c>
        /// de la asignación a desactivar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se desactivó correctamente.</returns>
        /// <exception cref="Exception">Se lanza si los IDs no son válidos, si la asignación no existe, o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(RolPermission pRolPermission)
        {
            if (pRolPermission.RolId <= 0)
                throw new Exception("El ID de rol no es válido.");

            if (pRolPermission.PermissionId <= 0)
                throw new Exception("El ID de permiso no es válido.");

            return await RolPermissionDAL.EliminarAsync(pRolPermission);
        }

        /// <summary>
        /// Obtiene la lista de permisos activos asignados a un rol específico.
        /// </summary>
        /// <param name="pRolId">Identificador del rol a consultar. Debe ser mayor a 0.</param>
        /// <returns>
        /// Lista de <see cref="RolPermission"/> con las propiedades de navegación
        /// <c>Rol</c> y <c>Permission</c> cargadas.
        /// </returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<List<RolPermission>> ObtenerPorRolAsync(int pRolId)
        {
            if (pRolId <= 0)
                throw new Exception("El ID de rol no es válido.");

            return await RolPermissionDAL.ObtenerPorRolAsync(pRolId);
        }

        /// <summary>
        /// Obtiene todos los registros activos de asignaciones entre roles y permisos.
        /// </summary>
        /// <returns>
        /// Lista completa de <see cref="RolPermission"/> activos con propiedades
        /// de navegación <c>Rol</c> y <c>Permission</c> cargadas.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<RolPermission>> ObtenerTodosAsync()
        {
            return await RolPermissionDAL.ObtenerTodosAsync();
        }

        #endregion
    }
}