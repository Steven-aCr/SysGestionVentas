using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;

namespace SysGestionVentas.DAL
{
    public class RolDAL
    {
        /// <summary>
        /// Registra un nuevo rol en la base de datos.
        /// </summary>
        /// <param name="pRol">Objeto <see cref="Rol"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación.</exception>
        public static async Task<int> GuardarAsync(Rol pRol)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    pRol.CreateAt = DateTime.Now;
                    dbContexto.Add(pRol);
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
        /// Modifica los datos de un rol existente en la base de datos.
        /// </summary>
        /// <param name="pRol">
        /// Objeto <see cref="Rol"/> con el <c>RolId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el rol no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Rol pRol)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var rol = await dbContexto.Rol.FirstOrDefaultAsync(
                        r => r.RolId == pRol.RolId);

                    if (rol == null)
                        throw new Exception($"No se encontró el rol con ID {pRol.RolId}.");

                    rol.Name = pRol.Name;
                    rol.Description = pRol.Description;

                    dbContexto.Update(rol);
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
        /// Realiza una eliminación lógica de un rol, cambiando su estado en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pRol">
        /// Objeto <see cref="Rol"/> con el <c>RolId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado "inactivo/eliminado".
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el rol no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Rol pRol)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var rol = await dbContexto.Rol.FirstOrDefaultAsync(
                        r => r.RolId == pRol.RolId);

                    if (rol == null)
                        throw new Exception($"No se encontró el rol con ID {pRol.RolId}.");

                    // Eliminación lógica: se cambia el estado del rol
                    // en lugar de eliminarlo físicamente de la base de datos.
                    rol.StatusId = pRol.StatusId;

                    dbContexto.Update(rol);
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
        /// Obtiene un rol específico por su identificador, incluyendo
        /// su relación con <see cref="RolPermission"/>.
        /// </summary>
        /// <param name="pRol">Objeto <see cref="Rol"/> con el <c>RolId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Rol"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Rol> ObtenerPorIdAsync(Rol pRol)
        {
            var result = new Rol();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Rol
                        .Include(r => r.RolPermission)
                        .FirstOrDefaultAsync(r => r.RolId == pRol.RolId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene una lista de roles aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pRol">
        /// Objeto <see cref="Rol"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>Name</c>: filtra por coincidencia parcial en el nombre (null = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Rol"/> que cumplen los filtros indicados,
        /// ordenados por nombre de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Rol>> ObtenerTodosAsync(Rol pRol)
        {
            var result = new List<Rol>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Rol
                        .Include(r => r.RolPermission)
                        .Where(r =>
                            (pRol.Name == null || r.Name.Contains(pRol.Name))
                        )
                        .OrderBy(r => r.Name)
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