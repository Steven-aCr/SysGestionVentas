using Microsoft.Identity.Client;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using SysGestionVentas.EN.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class UserBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="User"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pUser">Objeto <see cref="User"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación definidas en la entidad.
        /// El mensaje de la excepción contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(User pUser)
        {
            var contexto = new ValidationContext(pUser);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pUser, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Crea de forma atómica una <see cref="Person"/>, su <see cref="User"/> asociado
        /// y, cuando el rol corresponde a Vendedor (<c>RolId == CreateUserModel.RolVendedorId</c>),
        /// también el registro de <see cref="Employee"/> vinculado.
        /// Si cualquiera de las operaciones falla, la transacción completa se revierte
        /// garantizando la integridad de los datos.
        /// </summary>
        /// <param name="pModel">
        /// ViewModel con los datos combinados capturados desde el formulario de registro.
        /// Los campos de empleado (<c>EmployeeCode</c>, <c>HireDate</c>, <c>Salary</c>)
        /// son obligatorios cuando <c>RolId == CreateUserModel.RolVendedorId</c>.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>2</c> para usuario + persona,
        /// o <c>3</c> cuando además se crea el registro de empleado.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre un error durante la transacción o si hay duplicados
        /// de DUI, teléfono, nombre de usuario, correo electrónico o código de empleado.
        /// </exception>
        public static async Task<int> CrearConPersonaAsync(CreateUserModel pModel)
        {
            // ── Validaciones comunes ──────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(pModel.UserName))
                throw new Exception("El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(pModel.Email))
                throw new Exception("El correo electrónico es obligatorio.");

            if (string.IsNullOrWhiteSpace(pModel.Password))
                throw new Exception("La contraseña es obligatoria.");

            if (pModel.Password.Length < 8)
                throw new Exception("La contraseña debe tener al menos 8 caracteres.");

            // ── Validaciones adicionales para rol Vendedor ────────────────────
            bool esVendedor = pModel.RolId == CreateUserModel.RolVendedorId;

            // ── Determinar si el rol requiere registro de empleado ────────────
            bool esEmpleado = pModel.RolId == CreateUserModel.RolAdministradorId
                           || pModel.RolId == CreateUserModel.RolVendedorId;

            // ── Validaciones adicionales cuando aplica datos laborales ────────
            if (esEmpleado)
            {
                if (string.IsNullOrWhiteSpace(pModel.EmployeeCode))
                    throw new Exception("El código de empleado es obligatorio para este rol.");

                if (pModel.HireDate == null)
                    throw new Exception("La fecha de contratación es obligatoria para este rol.");

                if (pModel.HireDate.Value.Date > DateTime.UtcNow.Date)
                    throw new Exception("La fecha de contratación no puede ser una fecha futura.");

                if (pModel.Salary == null || pModel.Salary <= 0)
                    throw new Exception("El salario debe ser mayor a $0.00 para este rol.");
            }

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

                    // 2 — Crear el Usuario relacionado con la Persona
                    var user = new User
                    {
                        UserName = pModel.UserName,
                        Email = pModel.Email,
                        PasswordHash = pModel.Password, // se encripta dentro del DAL
                        RolId = pModel.RolId,
                        StatusId = pModel.StatusId,
                        PersonId = person.PersonId
                    };

                    await UserDAL.GuardarEnTransaccionAsync(user, dbContexto);
                    await dbContexto.SaveChangesAsync(); // genera el UserId

                    // 3 — Si el rol requiere empleado, crear también el registro de Employee
                    if (esEmpleado)
                    {
                        var employee = new Employee
                        {
                            EmployeeCode = pModel.EmployeeCode!,
                            HireDate = pModel.HireDate!.Value,
                            Salary = pModel.Salary!.Value,
                            DepartmentId = pModel.DepartmentId,
                            UserId = user.UserId,
                            PersonId = person.PersonId,
                            StatusId = pModel.StatusId
                        };

                        if (await EmployeeDAL.ExisteEmployeeCode(employee, dbContexto))
                            throw new Exception("El código de empleado ya existe.");

                        dbContexto.Employee.Add(employee);
                    }

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

        /// <summary>
        /// Valida y registra un nuevo usuario en el sistema.
        /// Aplica las validaciones de estructura definidas en la entidad antes de persistir.
        /// La contraseña es encriptada en la capa DAL antes de almacenarse.
        /// </summary>
        /// <param name="pUser">
        /// Objeto <see cref="User"/> con los datos del usuario a registrar.
        /// Los campos <c>UserName</c>, <c>Email</c>, <c>PasswordHash</c>, <c>RolId</c>,
        /// <c>PersonId</c> y <c>StatusId</c> son obligatorios.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación en base de datos.</exception>
        public static async Task<int> GuardarAsync(User pUser)
        {
            ValidarEntidad(pUser);
            return await UserDAL.GuardarAsync(pUser);
        }

        /// <summary>
        /// Valida y modifica los datos de un usuario existente en el sistema.
        /// No permite modificar la contraseña desde este método; para ello debe usarse
        /// <see cref="CambiarContrasenaAsync"/>.
        /// </summary>
        /// <param name="pUser">
        /// Objeto <see cref="User"/> con el <c>UserId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si el usuario no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> ModificarAsync(User pUser)
        {
            ValidarEntidad(pUser);
            return await UserDAL.ModificarAsync(pUser);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un usuario, cambiando su estado en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pUser">
        /// Objeto <see cref="User"/> con el <c>UserId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si el usuario no existe o si ocurre un error en base de datos.</exception>
        public static async Task<int> EliminarAsync(User pUser)
        {
            if (pUser.UserId <= 0)
                throw new Exception("El ID de usuario no es válido.");

            if (pUser.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await UserDAL.EliminarAsync(pUser);
        }

        /// <summary>
        /// Obtiene un usuario específico por su identificador, incluyendo sus relaciones
        /// con <see cref="Rol"/>, <see cref="Person"/> y <see cref="Status"/>.
        /// </summary>
        /// <param name="pUser">Objeto <see cref="User"/> con el <c>UserId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="User"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<User?> ObtenerPorIdAsync(User pUser)
        {
            if (pUser.UserId <= 0)
                throw new Exception("El ID de usuario no es válido.");

            return await UserDAL.ObtenerPorIdAsync(pUser);
        }

        /// <summary>
        /// Obtiene una lista de usuarios aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pUser">
        /// Objeto <see cref="User"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>UserName</c>: filtra por coincidencia parcial (null = sin filtro).</description></item>
        ///   <item><description><c>RolId</c>: filtra por rol asignado (0 = sin filtro).</description></item>
        ///   <item><description><c>StatusId</c>: filtra por estado (0 = sin filtro, devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="User"/> que cumplen los filtros indicados,
        /// ordenados por nombre de usuario de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<User>> ObtenerTodosAsync(User pUser)
        {
            return await UserDAL.ObtenerTodosAsync(pUser);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de usuarios con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{User}"/> que define los filtros, el tamaño de página
        /// y el número de página. No puede ser <c>null</c>.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{User}"/> con la lista de usuarios encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.</exception>
        public static async Task<PagedResult<User>> BuscarAsync(PagedQuery<User> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            return await UserDAL.BuscarAsync(pPagedQuery);
        }

        #endregion

        #region "Autenticación y Gestión de Contraseñas"

        /// <summary>
        /// Autentica a un usuario en el sistema mediante su correo electrónico y contraseña.
        /// Solo los usuarios con estado activo pueden iniciar sesión.
        /// </summary>
        /// <param name="pEmail">Correo electrónico del usuario. No puede ser nulo ni vacío.</param>
        /// <param name="pPassword">Contraseña del usuario en texto plano. No puede ser nula ni vacía.</param>
        /// <returns>
        /// Objeto <see cref="User"/> autenticado con sus relaciones cargadas
        /// (<see cref="Rol"/>, <see cref="Person"/>, <see cref="Status"/>).
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el correo o contraseña son vacíos, si el usuario no existe,
        /// si está inactivo, o si la contraseña es incorrecta.
        /// </exception>
        public static async Task<User> LoginAsync(string pEmail, string pPassword)
        {
            if (string.IsNullOrWhiteSpace(pEmail))
                throw new Exception("El correo electrónico es obligatorio.");

            if (string.IsNullOrWhiteSpace(pPassword))
                throw new Exception("La contraseña es obligatoria.");

            return await UserDAL.LogingAsync(pEmail, pPassword);
        }

        /// <summary>
        /// Cambia la contraseña de un usuario que tiene el cambio obligatorio activo.
        /// Valida que la nueva contraseña cumpla los requisitos mínimos de seguridad antes de persistir.
        /// </summary>
        /// <param name="pUserId">Identificador del usuario al que se le cambiará la contraseña.</param>
        /// <param name="pNewPassword">Nueva contraseña en texto plano. Debe tener al menos 8 caracteres.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el ID no es válido, si la contraseña no cumple los requisitos mínimos,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> ChangePasswordAsync(int pUserId, string pNewPassword)
        {
            if (pUserId <= 0)
                throw new Exception("El ID de usuario no es válido.");

            if (string.IsNullOrWhiteSpace(pNewPassword))
                throw new Exception("La nueva contraseña es obligatoria.");

            if (pNewPassword.Length < 8)
                throw new Exception("La contraseña debe tener al menos 8 caracteres.");

            return await UserDAL.ChangePasswordAsync(pUserId, pNewPassword);
        }

        /// <summary>
        /// Genera una contraseña temporal de acceso de emergencia para un usuario,
        /// a solicitud exclusiva de un administrador del sistema.
        /// La contraseña temporal tiene vigencia de 1 hora y es de un solo uso.
        /// </summary>
        /// <param name="pUserId">Identificador del usuario al que se le generará el acceso temporal.</param>
        /// <returns>
        /// La contraseña temporal en texto plano, para ser entregada al usuario
        /// por un canal seguro. Esta es la única vez que el valor está disponible en texto plano.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el ID no es válido, si el usuario no existe, si está inactivo,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<string> GenerarTempAsync(int pUserId)
        {
            if (pUserId <= 0)
                throw new Exception("El ID de usuario no es válido.");

            return await UserDAL.GenerarTempAsync(pUserId);
        }
        #endregion

        #region "Métodos Específicos de Negocio"
        /// <summary>
        /// Actualiza de forma atómica los datos personales y de acceso del usuario autenticado.
        /// Si se proporcionan campos de contraseña, valida la actual antes de aplicar el cambio.
        /// <see cref="Person"/> y <see cref="User"/> se actualizan en una sola transacción.
        /// </summary>
        /// <param name="pModel">
        /// ViewModel con los datos del perfil. Si <c>NewPassword</c> está vacío,
        /// la contraseña actual se conserva sin modificaciones.
        /// </param>
        /// <returns>Número de filas afectadas.</returns>
        /// <exception cref="Exception">
        /// Se lanza si la contraseña actual es incorrecta, si hay duplicados de DUI,
        /// teléfono o correo, o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> ActualizarPerfilAsync(EditProfileModel pModel)
        {
            int result = 0;

            using var dbContexto = new DbContexto();
            using var transaction = await dbContexto.Database.BeginTransactionAsync();

            try
            {
                // 1 — Actualizar datos personales
                var person = new Person
                {
                    PersonId = pModel.PersonId,
                    FirstName = pModel.FirstName,
                    LastName = pModel.LastName,
                    Adress = pModel.Adress,
                    PhoneNumber = pModel.PhoneNumber,
                    Dui = pModel.Dui
                };
                await PersonDAL.ModificarEnTransaccionAsync(person, dbContexto);

                // 2 — Actualizar datos de acceso del usuario
                var user = await UserDAL.ObtenerEnTransaccionAsync(pModel.UserId, dbContexto);
                if (user == null)
                    throw new Exception("No se encontró el usuario.");

                user.Email = pModel.Email;

                // 3 — Cambio de contraseña opcional
                if (!string.IsNullOrWhiteSpace(pModel.NewPassword))
                {
                    await UserDAL.ValidarContrasenaActualAsync(
                        pModel.UserId, pModel.CurrentPassword!, dbContexto);

                    user.PasswordHash = UserDAL.EncriptarSHA256Publico(pModel.NewPassword);
                }

                UserDAL.ModificarEnTransaccion(user, dbContexto);
                result = await dbContexto.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }

            return result;
        }
        #endregion
    }
}