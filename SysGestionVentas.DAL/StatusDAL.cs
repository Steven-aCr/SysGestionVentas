using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;

namespace SysGestionVentas.DAL
{
    public class StatusDAL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Verifica si ya existe un estado con el mismo nombre dentro del mismo tipo de estado
        /// en la base de datos, excluyendo el propio registro en caso de modificación.
        /// La unicidad del nombre se valida dentro del mismo <c>StatusTypeId</c> para
        /// permitir nombres iguales en distintos tipos (ej: "Activo" en Estado General
        /// y "Activo" en Estado de Inventario).
        /// </summary>
        /// <param name="pStatus">Objeto <see cref="Status"/> con el <c>Name</c> y <c>StatusTypeId</c> a validar.</param>
        /// <param name="pDbContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el nombre ya existe dentro del mismo tipo, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteNombre(Status pStatus, DbContexto pDbContexto)
        {
            return await pDbContexto.Status.AnyAsync(
                s => s.Name == pStatus.Name
                  && s.StatusTypeId == pStatus.StatusTypeId
                  && s.StatusId != pStatus.StatusId);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Registra un nuevo estado en la base de datos.
        /// Valida unicidad del <c>Name</c> dentro del mismo <c>StatusTypeId</c> antes de guardar.
        /// </summary>
        /// <param name="pStatus">Objeto <see cref="Status"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el nombre ya existe en el mismo tipo o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> GuardarAsync(Status pStatus)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteNombre(pStatus, dbContexto))
                        throw new Exception("Ya existe un estado con ese nombre en el mismo tipo de estado.");

                    dbContexto.Add(pStatus);
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
        /// Modifica los datos de un estado existente en la base de datos.
        /// Valida unicidad del <c>Name</c> dentro del mismo <c>StatusTypeId</c> antes de actualizar.
        /// </summary>
        /// <param name="pStatus">
        /// Objeto <see cref="Status"/> con el <c>StatusId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el estado no existe, si el nombre está duplicado dentro del tipo,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Status pStatus)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteNombre(pStatus, dbContexto))
                        throw new Exception("Ya existe un estado con ese nombre en el mismo tipo de estado.");

                    var status = await dbContexto.Status.FirstOrDefaultAsync(
                        s => s.StatusId == pStatus.StatusId);

                    if (status == null)
                        throw new Exception($"No se encontró el estado con ID {pStatus.StatusId}.");

                    status.Name = pStatus.Name;
                    status.Description = pStatus.Description;
                    status.IsActive = pStatus.IsActive;
                    status.StatusTypeId = pStatus.StatusTypeId;

                    dbContexto.Update(status);
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
        /// Realiza una eliminación lógica de un estado, desactivándolo en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pStatus">
        /// Objeto <see cref="Status"/> con el <c>StatusId</c> del registro a desactivar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se desactivó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el estado no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Status pStatus)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var status = await dbContexto.Status.FirstOrDefaultAsync(
                        s => s.StatusId == pStatus.StatusId);

                    if (status == null)
                        throw new Exception($"No se encontró el estado con ID {pStatus.StatusId}.");

                    // Eliminación lógica: se desactiva el estado mediante la bandera IsActive
                    // en lugar de eliminarlo físicamente de la base de datos.
                    status.IsActive = false;

                    dbContexto.Update(status);
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
        /// Obtiene un estado específico por su identificador, incluyendo
        /// su relación con <see cref="StatusType"/>.
        /// </summary>
        /// <param name="pStatus">Objeto <see cref="Status"/> con el <c>StatusId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Status"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Status?> ObtenerPorIdAsync(Status pStatus)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Status
                        .Include(s => s.StatusType)
                        .FirstOrDefaultAsync(s => s.StatusId == pStatus.StatusId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de estados aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pStatus">
        /// Objeto <see cref="Status"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>Name</c>: filtra por coincidencia parcial en el nombre (null = sin filtro).</description></item>
        ///   <item><description><c>StatusTypeId</c>: filtra por tipo de estado (0 = sin filtro, devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <param name="pIsActive">
        /// Filtro de estado: <c>true</c> = solo activos, <c>false</c> = solo inactivos, <c>null</c> = todos.
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Status"/> que cumplen los filtros indicados,
        /// ordenados por tipo de estado y luego por nombre de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Status>> ObtenerTodosAsync(Status pStatus, bool? pIsActive = null)
        {
            var result = new List<Status>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Status
                        .Include(s => s.StatusType)
                        .Where(s =>
                            (pStatus.Name == null || s.Name!.Contains(pStatus.Name)) &&
                            (pStatus.StatusTypeId == 0 || s.StatusTypeId == pStatus.StatusTypeId) &&
                            (pIsActive == null || s.IsActive == pIsActive)
                        )
                        .OrderBy(s => s.StatusType!.Name)
                            .ThenBy(s => s.Name)
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