using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;

namespace SysGestionVentas.DAL
{
    public class PermissionDAL
    {
        /// <summary>
        /// Registra un nuevo permiso en la base de datos.
        /// </summary>
        /// <param name="pPermission">Objeto <see cref="Permission"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación.</exception>
        public static async Task<int> GuardarAsync(Permission pPermission)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    pPermission.CreatedAt = DateTime.UtcNow;
                    dbContexto.Add(pPermission);
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
        /// Modifica los datos de un permiso existente en la base de datos.
        /// </summary>
        /// <param name="pPermission">
        /// Objeto <see cref="Permission"/> con el <c>PermissionId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el permiso no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Permission pPermission)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var permission = await dbContexto.Permission.FirstOrDefaultAsync(
                        p => p.PermissionId == pPermission.PermissionId);

                    if (permission == null)
                        throw new Exception($"No se encontró el permiso con ID {pPermission.PermissionId}.");

                    permission.Name = pPermission.Name;
                    permission.Description = pPermission.Description;
                    permission.IsActive = pPermission.IsActive;

                    dbContexto.Update(permission);
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
        /// Realiza una eliminación lógica de un permiso, desactivándolo en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pPermission">
        /// Objeto <see cref="Permission"/> con el <c>PermissionId</c> del registro a desactivar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se desactivó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el permiso no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Permission pPermission)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var permission = await dbContexto.Permission.FirstOrDefaultAsync(
                        p => p.PermissionId == pPermission.PermissionId);

                    if (permission == null)
                        throw new Exception($"No se encontró el permiso con ID {pPermission.PermissionId}.");

                    // Eliminación lógica: se desactiva el permiso
                    // en lugar de eliminarlo físicamente de la base de datos.
                    permission.IsActive = false;

                    dbContexto.Update(permission);
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
        /// Obtiene un permiso específico por su identificador.
        /// </summary>
        /// <param name="pPermission">Objeto <see cref="Permission"/> con el <c>PermissionId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Permission"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Permission?> ObtenerPorIdAsync(Permission pPermission)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Permission
                        .FirstOrDefaultAsync(p => p.PermissionId == pPermission.PermissionId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de permisos aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pPermission">
        /// Objeto <see cref="Permission"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>Name</c>: filtra por coincidencia parcial en el nombre (null = sin filtro).</description></item>
        ///   <item><description><c>IsActive</c>: filtra por estado activo/inactivo (null = devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <param name="pIsActive">
        /// Filtro de estado: <c>true</c> = solo activos, <c>false</c> = solo inactivos, <c>null</c> = todos.
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Permission"/> que cumplen los filtros indicados,
        /// ordenados por nombre de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Permission>> ObtenerTodosAsync(Permission pPermission, bool? pIsActive = null)
        {
            var result = new List<Permission>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Permission
                        .Where(p =>
                            (pPermission.Name == null || p.Name!.Contains(pPermission.Name)) &&
                            (pIsActive == null || p.IsActive == pIsActive)
                        )
                        .OrderBy(p => p.Name)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}