using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;

namespace SysGestionVentas.DAL
{
    public class StatusTypeDAL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Verifica si ya existe un tipo de estado con el mismo nombre en la base de datos,
        /// excluyendo el propio registro en caso de modificación.
        /// </summary>
        /// <param name="pStatusType">Objeto <see cref="StatusType"/> con el <c>Name</c> a validar.</param>
        /// <param name="pDbContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el nombre ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteNombre(StatusType pStatusType, DbContexto pDbContexto)
        {
            return await pDbContexto.StatusType.AnyAsync(
                st => st.Name == pStatusType.Name && st.StatusTypeId != pStatusType.StatusTypeId);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Registra un nuevo tipo de estado en la base de datos.
        /// Valida unicidad del <c>Name</c> antes de guardar.
        /// </summary>
        /// <param name="pStatusType">Objeto <see cref="StatusType"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el nombre ya existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> GuardarAsync(StatusType pStatusType)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteNombre(pStatusType, dbContexto))
                        throw new Exception("El nombre del tipo de estado ya existe.");

                    dbContexto.Add(pStatusType);
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
        /// Modifica los datos de un tipo de estado existente en la base de datos.
        /// Valida unicidad del <c>Name</c> antes de actualizar.
        /// </summary>
        /// <param name="pStatusType">
        /// Objeto <see cref="StatusType"/> con el <c>StatusTypeId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el tipo de estado no existe, si el nombre está duplicado,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(StatusType pStatusType)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteNombre(pStatusType, dbContexto))
                        throw new Exception("El nombre del tipo de estado ya existe.");

                    var statusType = await dbContexto.StatusType.FirstOrDefaultAsync(
                        st => st.StatusTypeId == pStatusType.StatusTypeId);

                    if (statusType == null)
                        throw new Exception($"No se encontró el tipo de estado con ID {pStatusType.StatusTypeId}.");

                    statusType.Name = pStatusType.Name;
                    statusType.Description = pStatusType.Description;
                    statusType.IsActive = pStatusType.IsActive;

                    dbContexto.Update(statusType);
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
        /// Realiza una eliminación lógica de un tipo de estado, desactivándolo en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pStatusType">
        /// Objeto <see cref="StatusType"/> con el <c>StatusTypeId</c> del registro a desactivar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se desactivó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el tipo de estado no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(StatusType pStatusType)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var statusType = await dbContexto.StatusType.FirstOrDefaultAsync(
                        st => st.StatusTypeId == pStatusType.StatusTypeId);

                    if (statusType == null)
                        throw new Exception($"No se encontró el tipo de estado con ID {pStatusType.StatusTypeId}.");

                    // Eliminación lógica: se desactiva el tipo de estado mediante la bandera IsActive
                    // en lugar de eliminarlo físicamente de la base de datos.
                    statusType.IsActive = false;

                    dbContexto.Update(statusType);
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
        /// Obtiene un tipo de estado específico por su identificador,
        /// incluyendo la colección de <see cref="Status"/> asociados.
        /// </summary>
        /// <param name="pStatusType">Objeto <see cref="StatusType"/> con el <c>StatusTypeId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="StatusType"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<StatusType?> ObtenerPorIdAsync(StatusType pStatusType)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.StatusType
                        .Include(st => st.Status)
                        .FirstOrDefaultAsync(st => st.StatusTypeId == pStatusType.StatusTypeId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de tipos de estado aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pStatusType">
        /// Objeto <see cref="StatusType"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>Name</c>: filtra por coincidencia parcial en el nombre (null = sin filtro).</description></item>
        /// </list>
        /// </param>
        /// <param name="pIsActive">
        /// Filtro de estado: <c>true</c> = solo activos, <c>false</c> = solo inactivos, <c>null</c> = todos.
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="StatusType"/> que cumplen los filtros indicados,
        /// ordenados por nombre de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<StatusType>> ObtenerTodosAsync(StatusType pStatusType, bool? pIsActive = null)
        {
            var result = new List<StatusType>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.StatusType
                        .Include(st => st.Status)
                        .Where(st =>
                            (pStatusType.Name == null || st.Name!.Contains(pStatusType.Name)) &&
                            (pIsActive == null || st.IsActive == pIsActive)
                        )
                        .OrderBy(st => st.Name)
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