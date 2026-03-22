using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;

namespace SysGestionVentas.DAL
{
    public class EmployeeDAL
    {
        /// <summary>
        /// Registra un nuevo empleado en la base de datos.
        /// </summary>
        /// <param name="pEmployee">Objeto <see cref="Employee"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación.</exception>
        public static async Task<int> GuardarAsync(Employee pEmployee)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
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
        /// </summary>
        /// <param name="pEmployee">
        /// Objeto <see cref="Employee"/> con el <c>EmployeeId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el empleado no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Employee pEmployee)
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

                    employee.EmployeeCode = pEmployee.EmployeeCode;
                    employee.HireDate = pEmployee.HireDate;
                    employee.Salary = pEmployee.Salary;
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
        /// y el <c>StatusId</c> correspondiente al estado "inactivo/eliminado".
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
        /// sus relaciones con <see cref="Person"/> y <see cref="SysStatus"/>.
        /// </summary>
        /// <param name="pEmployee">Objeto <see cref="Employee"/> con el <c>EmployeeId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Employee"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Employee> ObtenerPorIdAsync(Employee pEmployee)
        {
            var result = new Employee();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Employee
                        .Include(e => e.Person)
                        .Include(e => e.SysStatus)
                        .FirstOrDefaultAsync(e => e.EmployeeId == pEmployee.EmployeeId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene una lista de empleados aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pEmployee">
        /// Objeto <see cref="Employee"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>EmployeeCode</c>: filtra por coincidencia parcial en el código.</description></item>
        ///   <item><description><c>StatusId</c>: filtra por estado (0 = sin filtro).</description></item>
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
                        .Include(e => e.SysStatus)
                        .Where(e =>
                            (pEmployee.EmployeeCode == null || e.EmployeeCode.Contains(pEmployee.EmployeeCode)) &&
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
    }
}