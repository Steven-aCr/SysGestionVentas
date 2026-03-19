
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace BDGestionVentas.DAL
{
    public class EmployeeDAL
    {
        private static async Task<bool> ExisteCode(Employee pEmployee,
            DbContexto pDBContexto)
        {
            bool result = false;
            var existe = await pDBContexto.Employee.FirstOrDefaultAsync(
                e => e.EmployeeCode == pEmployee.EmployeeCode &&
                     e.EmployeeId != pEmployee.EmployeeId);
            if (existe != null && existe.EmployeeId > 0)
                result = true;
            return result;
        }

        public static async Task<int> GuardarAsync(Employee pEmployee)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    bool existeCode = await ExisteCode(pEmployee, dbContexto);
                    if (existeCode == false)
                    {
                        dbContexto.Add(pEmployee);
                        result = await dbContexto.SaveChangesAsync();
                    }
                    else
                        throw new Exception("El código de empleado ya existe.");
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<int> ModificarAsync(Employee pEmployee)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    bool existeCode = await ExisteCode(pEmployee, dbContexto);
                    if (existeCode == false)
                    {
                        var employee = await dbContexto.Employee.FirstOrDefaultAsync(
                            e => e.EmployeeId == pEmployee.EmployeeId);

                        employee.EmployeeCode = pEmployee.EmployeeCode;
                        employee.HireDate = pEmployee.HireDate;
                        employee.Salary = pEmployee.Salary;
                        employee.PersonId = pEmployee.PersonId;
                        employee.StatusId = pEmployee.StatusId;

                        dbContexto.Update(employee);
                        result = await dbContexto.SaveChangesAsync();
                    }
                    else
                        throw new Exception("El código de empleado ya existe.");
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<int> EliminarAsync(Employee pEmployee)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var employee = await dbContexto.Employee.FirstOrDefaultAsync(
                        e => e.EmployeeId == pEmployee.EmployeeId);

                    dbContexto.Remove(employee);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<Employee> ObtenerPorIdAsync(Employee pEmployee)
        {
            var result = new Employee();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Employee
                        .Include(e => e.Person)
                        .Include(e => e.Status)
                        .FirstOrDefaultAsync(e => e.EmployeeId == pEmployee.EmployeeId);
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<List<Employee>> ObtenerTodosAsync(Employee pEmployee)
        {
            var result = new List<Employee>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Employee
                        .Include(e => e.Person)
                        .Include(e => e.Status)
                        .Where(e =>
                            (pEmployee.EmployeeCode == null || e.EmployeeCode.Contains(pEmployee.EmployeeCode)) &&
                            (pEmployee.StatusId == 0 || e.StatusId == pEmployee.StatusId)
                        )
                        .OrderBy(e => e.EmployeeCode)
                        .ToListAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }
    }
}