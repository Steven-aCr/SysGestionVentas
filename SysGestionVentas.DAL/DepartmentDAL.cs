using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;

namespace SysGestionVentas.DAL
{
    public class DepartmentDAL
    {
        /// <summary>
        /// Verifica si ya existe un departamento con el mismo nombre en la base de datos,
        /// excluyendo el propio registro en caso de modificación.
        /// </summary>
        /// <param name="pDepartment">Objeto <see cref="Department"/> con el <c>Name</c> a validar.</param>
        /// <param name="pDBContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el nombre ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteNombre(Department pDepartment, DbContexto pDBContexto)
        {
            return await pDBContexto.Department.AnyAsync(
                d => d.Name == pDepartment.Name && d.DepartmentId != pDepartment.DepartmentId);
        }

        #region "CRUD"
        /// <summary>
        /// Registra un nuevo departamento en la base de datos.
        /// Valida unicidad del <c>Name</c> antes de guardar.
        /// </summary>
        /// <param name="pDepartment">Objeto <see cref="Department"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el nombre ya existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> GuardarAsync(Department pDepartment)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteNombre(pDepartment, dbContexto))
                        throw new Exception("El nombre del departamento ya existe.");

                    pDepartment.CreatedAt = DateTime.UtcNow;
                    dbContexto.Add(pDepartment);
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
        /// Modifica los datos de un departamento existente en la base de datos.
        /// Valida unicidad del <c>Name</c> antes de actualizar.
        /// </summary>
        /// <param name="pDepartment">
        /// Objeto <see cref="Department"/> con el <c>DepartmentId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el departamento no existe, si el nombre está duplicado,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Department pDepartment)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteNombre(pDepartment, dbContexto))
                        throw new Exception("El nombre del departamento ya existe.");

                    var department = await dbContexto.Department.FirstOrDefaultAsync(
                        d => d.DepartmentId == pDepartment.DepartmentId);

                    if (department == null)
                        throw new Exception($"No se encontró el departamento con ID {pDepartment.DepartmentId}.");

                    department.Name = pDepartment.Name;
                    department.Description = pDepartment.Description;
                    department.StatusId = pDepartment.StatusId;

                    dbContexto.Update(department);
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
        /// Realiza una eliminación lógica de un departamento, cambiando su estado en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pDepartment">
        /// Objeto <see cref="Department"/> con el <c>DepartmentId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el departamento no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Department pDepartment)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var department = await dbContexto.Department.FirstOrDefaultAsync(
                        d => d.DepartmentId == pDepartment.DepartmentId);

                    if (department == null)
                        throw new Exception($"No se encontró el departamento con ID {pDepartment.DepartmentId}.");

                    // Eliminación lógica: se cambia el estado del departamento
                    // en lugar de eliminarlo físicamente de la base de datos.
                    department.StatusId = pDepartment.StatusId;

                    dbContexto.Update(department);
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
        /// Obtiene un departamento específico por su identificador, incluyendo
        /// su relación con <see cref="Status"/>.
        /// </summary>
        /// <param name="pDepartment">Objeto <see cref="Department"/> con el <c>DepartmentId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Department"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Department?> ObtenerPorIdAsync(Department pDepartment)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Department
                        .Include(d => d.Status)
                        .FirstOrDefaultAsync(d => d.DepartmentId == pDepartment.DepartmentId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de departamentos aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pDepartment">
        /// Objeto <see cref="Department"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>Name</c>: filtra por coincidencia parcial en el nombre (null = sin filtro).</description></item>
        ///   <item><description><c>StatusId</c>: filtra por estado (0 = sin filtro, devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Department"/> que cumplen los filtros indicados,
        /// ordenados por nombre de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Department>> ObtenerTodosAsync(Department pDepartment)
        {
            var result = new List<Department>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Department
                        .Include(d => d.Status)
                        .Where(d =>
                            (pDepartment.Name == null || d.Name!.Contains(pDepartment.Name)) &&
                            (pDepartment.StatusId == 0 || d.StatusId == pDepartment.StatusId)
                        )
                        .OrderBy(d => d.Name)
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