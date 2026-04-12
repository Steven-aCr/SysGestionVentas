using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.ViewModels;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class EmployeeBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Employee"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pEmployee">Objeto <see cref="Employee"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(Employee pEmployee)
        {
            var contexto = new ValidationContext(pEmployee);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pEmployee, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        /// <summary>
        /// Valida las reglas de negocio aplicables a la fecha de contratación
        /// y al salario del empleado.
        /// </summary>
        /// <param name="pEmployee">Objeto <see cref="Employee"/> con los datos a validar.</param>
        /// <exception cref="Exception">
        /// Se lanza si la fecha de contratación es futura o si el salario es negativo o cero.
        /// </exception>
        private static void ValidarReglasnegocio(Employee pEmployee)
        {
            if (pEmployee.HireDate > DateTime.UtcNow.Date)
                throw new Exception("La fecha de contratación no puede ser una fecha futura.");

            if (pEmployee.Salary <= 0)
                throw new Exception("El salario debe ser mayor a $0.00.");
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo empleado en el sistema.
        /// Verifica unicidad del código de empleado en la capa DAL.
        /// </summary>
        /// <param name="pEmployee">Objeto <see cref="Employee"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el código de empleado ya existe, si las reglas de negocio no se cumplen,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> GuardarAsync(Employee pEmployee)
        {
            ValidarEntidad(pEmployee);
            ValidarReglasnegocio(pEmployee);
            return await EmployeeDAL.GuardarAsync(pEmployee);
        }

        /// <summary>
        /// Valida y modifica los datos de un empleado existente en el sistema.
        /// Verifica unicidad del código de empleado en la capa DAL.
        /// </summary>
        /// <param name="pEmployee">
        /// Objeto <see cref="Employee"/> con el <c>EmployeeId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el empleado no existe, si el código está duplicado, si las reglas
        /// de negocio no se cumplen, o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> ModificarAsync(Employee pEmployee)
        {
            if (pEmployee.EmployeeId <= 0)
                throw new Exception("El ID de empleado no es válido.");

            ValidarEntidad(pEmployee);
            ValidarReglasnegocio(pEmployee);
            return await EmployeeDAL.ModificarAsync(pEmployee);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un empleado cambiando su estado en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pEmployee">
        /// Objeto <see cref="Employee"/> con el <c>EmployeeId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el ID no es válido, si el empleado no existe,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> EliminarAsync(Employee pEmployee)
        {
            if (pEmployee.EmployeeId <= 0)
                throw new Exception("El ID de empleado no es válido.");

            if (pEmployee.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await EmployeeDAL.EliminarAsync(pEmployee);
        }

        /// <summary>
        /// Obtiene un empleado específico por su identificador, incluyendo sus relaciones
        /// con <see cref="Person"/>, <see cref="Department"/>, <see cref="User"/> y <see cref="Status"/>.
        /// </summary>
        /// <param name="pEmployee">Objeto <see cref="Employee"/> con el <c>EmployeeId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Employee"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Employee?> ObtenerPorIdAsync(Employee pEmployee)
        {
            if (pEmployee.EmployeeId <= 0)
                throw new Exception("El ID de empleado no es válido.");

            return await EmployeeDAL.ObtenerPorIdAsync(pEmployee);
        }

        /// <summary>
        /// Obtiene una lista de empleados aplicando filtros opcionales.
        /// </summary>
        /// <param name="pEmployee">Objeto <see cref="Employee"/> usado como filtro de búsqueda.</param>
        /// <returns>Lista de objetos <see cref="Employee"/> ordenados por código de empleado.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Employee>> ObtenerTodosAsync(Employee pEmployee)
        {
            return await EmployeeDAL.ObtenerTodosAsync(pEmployee);
        }

        /// <summary>
        /// Crea de forma atómica una <see cref="Person"/> y su <see cref="Employee"/> asociado
        /// en una única transacción de base de datos.
        /// Si cualquiera de las dos operaciones falla, se revierte la transacción completa
        /// garantizando la integridad de los datos.
        /// </summary>
        /// <param name="pModel">
        /// ViewModel con los datos combinados de <see cref="Person"/> y <see cref="Employee"/>
        /// capturados desde el formulario de registro.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>2</c> si ambos registros
        /// se guardaron correctamente.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre un error durante la transacción o si hay duplicados
        /// de DUI, teléfono o código de empleado.
        /// </exception>
        public static async Task<int> CrearConPersonaAsync(CreateEmployeeModel pModel)
        {
            if (string.IsNullOrWhiteSpace(pModel.EmployeeCode))
                throw new Exception("El código de empleado es obligatorio.");

            if (pModel.HireDate > DateTime.UtcNow.Date)
                throw new Exception("La fecha de contratación no puede ser una fecha futura.");

            if (pModel.Salary <= 0)
                throw new Exception("El salario debe ser mayor a $0.00.");

            int result = 0;

            using (var dbContexto = new DbContexto())
            using (var transaction = await dbContexto.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1 — Crear y persistir la Persona
                    var person = new Person
                    {
                        FirstName = pModel.FirstName,
                        LastName = pModel.LastName,
                        Adress = pModel.Adress,
                        PhoneNumber = pModel.PhoneNumber,
                        Dui = pModel.Dui,
                        StatusId = pModel.StatusId
                    };

                    await PersonDAL.GuardarEnTransaccionAsync(person, dbContexto);
                    await dbContexto.SaveChangesAsync(); // genera el PersonId

                    // 2 — Crear el Empleado relacionado con la Persona recién creada
                    var employee = new Employee
                    {
                        EmployeeCode = pModel.EmployeeCode,
                        HireDate = pModel.HireDate,
                        Salary = pModel.Salary,
                        DepartmentId = pModel.DepartmentId,
                        UserId = pModel.UserId,
                        StatusId = pModel.StatusId,
                        PersonId = person.PersonId   // FK con la persona recién creada
                    };

                    // Validar unicidad de código antes de agregar
                    if (await EmployeeDAL.ExisteEmployeeCode(employee, dbContexto))
                        throw new Exception("El código de empleado ya existe.");

                    dbContexto.Employee.Add(employee);
                    result = await dbContexto.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception(ex.Message);
                }
            }

            return result;
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de empleados con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Employee}"/> con los filtros y parámetros de paginación.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Employee}"/> con la lista de empleados encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">
        /// Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<PagedResult<Employee>> BuscarAsync(PagedQuery<Employee> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            return await EmployeeDAL.BuscarAsync(pPagedQuery);
        }

        #endregion
    }
}