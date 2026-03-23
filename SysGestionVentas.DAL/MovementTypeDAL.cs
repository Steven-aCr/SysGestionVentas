using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;

namespace SysGestionVentas.DAL
{
    public class MovementTypeDAL
    {
        /// <summary>
        /// Verifica si ya existe un tipo de movimiento con el mismo nombre en la base de datos,
        /// excluyendo el propio registro en caso de modificación.
        /// </summary>
        /// <param name="pMovementType">Objeto <see cref="MovementType"/> con el <c>Name</c> a validar.</param>
        /// <param name="pDBContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el nombre ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteNombre(MovementType pMovementType, DbContexto pDBContexto)
        {
            return await pDBContexto.MovementType.AnyAsync(
                m => m.Name == pMovementType.Name && m.MovementTypeId != pMovementType.MovementTypeId);
        }

        #region "CRUD"

        /// <summary>
        /// Registra un nuevo tipo de movimiento en la base de datos.
        /// Valida unicidad del <c>Name</c> antes de guardar.
        /// </summary>
        /// <param name="pMovementType">Objeto <see cref="MovementType"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el nombre ya existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> GuardarAsync(MovementType pMovementType)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteNombre(pMovementType, dbContexto))
                        throw new Exception("El tipo de movimiento ya existe.");

                    dbContexto.Add(pMovementType);
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
        /// Modifica los datos de un tipo de movimiento existente en la base de datos.
        /// Valida unicidad del <c>Name</c> antes de actualizar.
        /// </summary>
        /// <param name="pMovementType">
        /// Objeto <see cref="MovementType"/> con el <c>MovementTypeId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el tipo de movimiento no existe, si el nombre está duplicado,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(MovementType pMovementType)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteNombre(pMovementType, dbContexto))
                        throw new Exception("El tipo de movimiento ya existe.");

                    var movementType = await dbContexto.MovementType.FirstOrDefaultAsync(
                        m => m.MovementTypeId == pMovementType.MovementTypeId);

                    if (movementType == null)
                        throw new Exception($"No se encontró el tipo de movimiento con ID {pMovementType.MovementTypeId}.");

                    movementType.Name = pMovementType.Name;
                    movementType.Description = pMovementType.Description;
                    movementType.IsActive = pMovementType.IsActive;

                    dbContexto.Update(movementType);
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
        /// Realiza una eliminación lógica de un tipo de movimiento, desactivándolo en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pMovementType">
        /// Objeto <see cref="MovementType"/> con el <c>MovementTypeId</c> del registro a desactivar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se desactivó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el tipo de movimiento no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(MovementType pMovementType)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var movementType = await dbContexto.MovementType.FirstOrDefaultAsync(
                        m => m.MovementTypeId == pMovementType.MovementTypeId);

                    if (movementType == null)
                        throw new Exception($"No se encontró el tipo de movimiento con ID {pMovementType.MovementTypeId}.");

                    // Eliminación lógica: se desactiva el tipo de movimiento
                    // en lugar de eliminarlo físicamente de la base de datos.
                    movementType.IsActive = false;

                    dbContexto.Update(movementType);
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
        /// Obtiene un tipo de movimiento específico por su identificador.
        /// </summary>
        /// <param name="pMovementType">Objeto <see cref="MovementType"/> con el <c>MovementTypeId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="MovementType"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<MovementType?> ObtenerPorIdAsync(MovementType pMovementType)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.MovementType
                        .FirstOrDefaultAsync(m => m.MovementTypeId == pMovementType.MovementTypeId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de tipos de movimiento aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pMovementType">
        /// Objeto <see cref="MovementType"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>Name</c>: filtra por coincidencia parcial en el nombre (null = sin filtro).</description></item>
        ///   <item><description><c>IsActive</c>: se gestiona mediante el parámetro <c>pIsActive</c>.</description></item>
        /// </list>
        /// </param>
        /// <param name="pIsActive">
        /// Filtro de estado: <c>true</c> = solo activos, <c>false</c> = solo inactivos, <c>null</c> = todos.
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="MovementType"/> que cumplen los filtros indicados,
        /// ordenados por nombre de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<MovementType>> ObtenerTodosAsync(MovementType pMovementType, bool? pIsActive = null)
        {
            var result = new List<MovementType>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.MovementType
                        .Where(m =>
                            (pMovementType.Name == null || m.Name!.Contains(pMovementType.Name)) &&
                            (pIsActive == null || m.IsActive == pIsActive)
                        )
                        .OrderBy(m => m.Name)
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