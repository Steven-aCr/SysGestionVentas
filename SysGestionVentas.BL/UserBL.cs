using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
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
            return await UsersDAL.GuardarAsync(pUser);
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
            return await UsersDAL.ModificarAsync(pUser);
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

            return await UsersDAL.EliminarAsync(pUser);
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

            return await UsersDAL.ObtenerPorIdAsync(pUser);
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
            return await UsersDAL.ObtenerTodosAsync(pUser);
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

            return await UsersDAL.BuscarAsync(pPagedQuery);
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

            return await UsersDAL.LogingAsync(pEmail, pPassword);
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

            return await UsersDAL.ChangePasswordAsync(pUserId, pNewPassword);
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

            return await UsersDAL.GenerarTempAsync(pUserId);
        }

        #endregion
    }
}