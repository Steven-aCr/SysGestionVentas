using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN.Pagination;

namespace SysGestionVentas.DAL
{
    public class EmployeeDAL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Verifica si ya existe un empleado con el mismo código en la base de datos,
        /// excluyendo el propio registro en caso de modificación.
        /// </summary>
        /// <param name="pEmployee">Objeto <see cref="Employee"/> con el <c>EmployeeCode</c> a validar.</param>
        /// <param name="dbContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el código ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteEmployeeCode(Employee pEmployee, DbContexto dbContexto)
        {
            return await dbContexto.Employee.AnyAsync(
                e => e.EmployeeCode == pEmployee.EmployeeCode
                  && e.EmployeeId != pEmployee.EmployeeId);
        }

        /// <summary>
        /// Aplica los filtros de búsqueda contenidos en <see cref="PagedQuery{Employee}"/>
        /// a una consulta <see cref="IQueryable{Employee}"/> base.
        /// No aplica paginación; esta responsabilidad recae en <see cref="BuscarAsync"/>,
        /// lo que permite reutilizar este método para conteo sin Skip/Take.
        /// </summary>
        /// <param name="pQuery">Consulta base sin filtros aplicados.</param>
        /// <param name="pPagedQuery">Parámetros de filtro, rango de fechas y paginación.</param>
        /// <returns>
        /// <see cref="IQueryable{Employee}"/> con todos los filtros y el ordenamiento aplicados.
        /// </returns>
        private static IQueryable<Employee> QuerySelect(
            IQueryable<Employee> pQuery,
            PagedQuery<Employee> pPagedQuery)
        {
            var f = pPagedQuery.Filter;

            if (f.EmployeeId > 0)
                pQuery = pQuery.Where(e => e.EmployeeId == f.EmployeeId);

            if (!string.IsNullOrWhiteSpace(f.EmployeeCode))
                pQuery = pQuery.Where(e => e.EmployeeCode!.Contains(f.EmployeeCode));

            if (pPagedQuery.FromDate.HasValue)
                pQuery = pQuery.Where(e => e.HireDate >= pPagedQuery.FromDate.Value);

            if (pPagedQuery.ToDate.HasValue)
                pQuery = pQuery.Where(e => e.HireDate <= pPagedQuery.ToDate.Value);

            if (f.DepartmentId.HasValue)
                pQuery = pQuery.Where(e => e.DepartmentId == f.DepartmentId.Value);

            if (f.UserId.HasValue)
                pQuery = pQuery.Where(e => e.UserId == f.UserId.Value);

            if (f.PersonId > 0)
                pQuery = pQuery.Where(e => e.PersonId == f.PersonId);

            if (f.StatusId > 0)
                pQuery = pQuery.Where(e => e.StatusId == f.StatusId);

            return pQuery.OrderBy(e => e.EmployeeCode);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Registra un nuevo empleado en la base de datos.
        /// Valida unicidad del <c>EmployeeCode</c> antes de guardar.
        /// </summary>
        /// <param name="pEmployee">Objeto <see cref="Employee"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el código de empleado ya existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> GuardarAsync(Employee pEmployee)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteEmployeeCode(pEmployee, dbContexto))
                        throw new Exception("El código de empleado ya existe.");

                    dbContexto.Add(pEmployee);
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
        /// Modifica los datos de un empleado existente en la base de datos.
        /// Valida unicidad del <c>EmployeeCode</c> antes de actualizar.
        /// </summary>
        /// <param name="pEmployee">
        /// Objeto <see cref="Employee"/> con el <c>EmployeeId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el empleado no existe, si el código está duplicado,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Employee pEmployee)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteEmployeeCode(pEmployee, dbContexto))
                        throw new Exception("El código de empleado ya existe.");

                    var employee = await dbContexto.Employee.FirstOrDefaultAsync(
                        e => e.EmployeeId == pEmployee.EmployeeId);

                    if (employee == null)
                        throw new Exception($"No se encontró el empleado con ID {pEmployee.EmployeeId}.");

                    employee.EmployeeCode = pEmployee.EmployeeCode;
                    employee.HireDate = pEmployee.HireDate;
                    employee.Salary = pEmployee.Salary;
                    employee.DepartmentId = pEmployee.DepartmentId;
                    employee.UserId = pEmployee.UserId;
                    employee.PersonId = pEmployee.PersonId;
                    employee.StatusId = pEmployee.StatusId;

                    dbContexto.Update(employee);
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
        /// Realiza una eliminación lógica de un empleado, cambiando su estado en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pEmployee">
        /// Objeto <see cref="Employee"/> con el <c>EmployeeId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el empleado no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Employee pEmployee)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var employee = await dbContexto.Employee.FirstOrDefaultAsync(
                        e => e.EmployeeId == pEmployee.EmployeeId);

                    if (employee == null)
                        throw new Exception($"No se encontró el empleado con ID {pEmployee.EmployeeId}.");

                    // Eliminación lógica: se cambia el estado del empleado
                    // en lugar de eliminarlo físicamente de la base de datos.
                    employee.StatusId = pEmployee.StatusId;

                    dbContexto.Update(employee);
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
        /// Obtiene un empleado específico por su identificador, incluyendo
        /// sus relaciones con <see cref="Person"/>, <see cref="Department"/>,
        /// <see cref="User"/> y <see cref="Status"/>.
        /// </summary>
        /// <param name="pEmployee">Objeto <see cref="Employee"/> con el <c>EmployeeId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Employee"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Employee?> ObtenerPorIdAsync(Employee pEmployee)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Employee
                        .Include(e => e.Person)
                        .Include(e => e.Department)
                        .Include(e => e.User)
                        .Include(e => e.Status)
                        .FirstOrDefaultAsync(e => e.EmployeeId == pEmployee.EmployeeId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de empleados aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pEmployee">
        /// Objeto <see cref="Employee"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>EmployeeCode</c>: filtra por coincidencia parcial en el código (null = sin filtro).</description></item>
        ///   <item><description><c>DepartmentId</c>: filtra por departamento (null = sin filtro).</description></item>
        ///   <item><description><c>StatusId</c>: filtra por estado (0 = sin filtro, devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Employee"/> que cumplen los filtros indicados,
        /// ordenados por código de empleado de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Employee>> ObtenerTodosAsync(Employee pEmployee)
        {
            var result = new List<Employee>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Employee
                        .Include(e => e.Person)
                        .Include(e => e.Department)
                        .Include(e => e.User)
                        .Include(e => e.Status)
                        .Where(e =>
                            (pEmployee.EmployeeCode == null || e.EmployeeCode!.Contains(pEmployee.EmployeeCode)) &&
                            (!pEmployee.DepartmentId.HasValue || e.DepartmentId == pEmployee.DepartmentId) &&
                            (pEmployee.StatusId == 0 || e.StatusId == pEmployee.StatusId)
                        )
                        .OrderBy(e => e.EmployeeCode)
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

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de empleados con soporte para paginación
        /// según los criterios especificados en <paramref name="pPagedQuery"/>.
        /// Si <c>Top</c> es mayor a cero, devuelve únicamente los primeros <c>Top</c> registros
        /// ignorando los parámetros de paginación.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Employee}"/> que define los filtros, el tamaño de página,
        /// el número de página y otros parámetros de búsqueda. No puede ser <c>null</c>.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Employee}"/> con la lista de empleados encontrados
        /// e información de paginación (total de registros, página actual, tamaño de página).
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre un error durante la ejecución de la consulta o el acceso a la base de datos.
        /// </exception>
        public static async Task<PagedResult<Employee>> BuscarAsync(PagedQuery<Employee> pPagedQuery)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var baseQuery = dbContexto.Employee
                        .Include(e => e.Department)
                        .Include(e => e.User)
                        .Include(e => e.Person)
                        .Include(e => e.Status)
                        .AsQueryable();

                    var filtered = QuerySelect(baseQuery, pPagedQuery);
                    int total = await filtered.CountAsync();

                    List<Employee> items;

                    if (pPagedQuery.Top > 0)
                    {
                        items = await filtered
                            .Take(pPagedQuery.Top)
                            .ToListAsync();
                    }
                    else
                    {
                        items = await filtered
                            .Skip(pPagedQuery.Skip)
                            .Take(pPagedQuery.PageSize)
                            .ToListAsync();
                    }

                    return new PagedResult<Employee>
                    {
                        Items = items,
                        TotalCount = total,
                        CurrentPage = pPagedQuery.Page,
                        PageSize = pPagedQuery.PageSize
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}