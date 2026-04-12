using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;

namespace SysGestionVentas.DAL
{

    public class RolPermissionDAL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Verifica si ya existe una asignación activa entre el rol y el permiso indicados.
        /// </summary>
        /// <param name="pRolPermission">Entidad con <c>RolId</c> y <c>PermissionId</c> a verificar.</param>
        /// <param name="pDbContexto">Instancia activa del contexto de base de datos.</param>
        /// <returns>
        /// <c>true</c> si la asignación ya existe y está activa; <c>false</c> en caso contrario.
        /// </returns>
        private static async Task<bool> ExisteAsignacionAsync(RolPermission pRolPermission, DbContexto pDbContexto)
        {
            return await pDbContexto.RolPermission
                .AnyAsync(rp => rp.RolId == pRolPermission.RolId
                             && rp.PermissionId == pRolPermission.PermissionId
                             && rp.IsActive);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Registra una nueva asignación de permiso a un rol.
        /// </summary>
        /// <param name="pRolPermission">
        /// Entidad <see cref="RolPermission"/> con <c>RolId</c>, <c>PermissionId</c>
        /// y <c>AssignedByUser</c> requeridos.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente,
        /// <c>0</c> si ocurrió un error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si la asignación ya existe o si ocurre un error en la base de datos.
        /// </exception>
        public static async Task<int> GuardarAsync(RolPermission pRolPermission)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    bool existeAsignacion = await ExisteAsignacionAsync(pRolPermission, dbContexto);
                    if (existeAsignacion)
                        throw new Exception("El permiso ya está asignado a este rol.");

                    pRolPermission.AssignedAt = DateTime.UtcNow;
                    pRolPermission.IsActive = true;
                    dbContexto.RolPermission.Add(pRolPermission);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Realiza la eliminación lógica de una asignación de permiso a un rol,
        /// marcando <c>IsActive = false</c> sin eliminar el registro físicamente.
        /// </summary>
        /// <param name="pRolPermission">
        /// Entidad con <c>RolId</c> y <c>PermissionId</c> de la asignación a desactivar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se desactivó correctamente,
        /// <c>0</c> si ocurrió un error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si la asignación no existe o si ocurre un error en la base de datos.
        /// </exception>
        public static async Task<int> EliminarAsync(RolPermission pRolPermission)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var asignacion = await dbContexto.RolPermission
                        .FirstOrDefaultAsync(rp => rp.RolId == pRolPermission.RolId
                                                && rp.PermissionId == pRolPermission.PermissionId
                                                && rp.IsActive);

                    if (asignacion == null)
                        throw new Exception("La asignación no existe o ya fue desactivada.");

                    asignacion.IsActive = false;
                    dbContexto.RolPermission.Update(asignacion);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene la lista de permisos activos asignados a un rol específico.
        /// </summary>
        /// <param name="pRolId">Identificador del rol a consultar.</param>
        /// <returns>
        /// Lista de <see cref="RolPermission"/> con las propiedades de navegación
        /// <c>Rol</c> y <c>Permission</c> cargadas. Retorna una lista vacía si no hay resultados.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre un error en la base de datos.
        /// </exception>
        public static async Task<List<RolPermission>> ObtenerPorRolAsync(int pRolId)
        {
            var result = new List<RolPermission>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.RolPermission
                        .Include(rp => rp.Rol)
                        .Include(rp => rp.Permission)
                        .Where(rp => rp.RolId == pRolId && rp.IsActive)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene todos los registros de asignaciones activas entre roles y permisos.
        /// </summary>
        /// <returns>
        /// Lista completa de <see cref="RolPermission"/> activos con propiedades
        /// de navegación <c>Rol</c> y <c>Permission</c> cargadas.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre un error en la base de datos.
        /// </exception>
        public static async Task<List<RolPermission>> ObtenerTodosAsync()
        {
            var result = new List<RolPermission>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.RolPermission
                        .Include(rp => rp.Rol)
                        .Include(rp => rp.Permission)
                        .Where(rp => rp.IsActive)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        #endregion
    }
}