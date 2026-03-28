using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.Security.Cryptography;
using System.Text;

namespace SysGestionVentas.DAL
{
    public class UsersDAL
    {
        #region "Métodos Privados"
        /// <summary>
        /// Encripta la contraseña del usuario utilizando SHA256.
        /// </summary>
        /// <param name="pUser">Objeto <see cref="User"/> con la contraseña en texto plano a encriptar.
        /// El campo <c>PasswordHash</c> será reemplazado por su representación hash.</param>
        private static string EncriptarSHA256(string pInput)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pInput));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        /// <summary>
        /// Genera un código temporal aleatorio compuesto por letras y dígitos.
        /// </summary>
        /// <returns>Una cadena de 10 caracteres que contiene letras mayúsculas, minúsculas y dígitos seleccionados
        /// aleatoriamente.</returns>
        private static string GenerarCodigoTemporal()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
                                 "abcdefghijklmnopqrstuvwxyz" +
                                 "0123456789";
            var random = new Random();
            var tempCode = new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return tempCode;
        }

        /// <summary>
        /// Verifica si existe un <c>UserName</c> duplicado en la base de datos,
        /// excluyendo el propio registro del usuario en caso de modificación.
        /// </summary>
        /// <param name="pUser">Objeto <see cref="User"/> con el <c>UserName</c> a validar.</param>
        /// <param name="dbContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el <c>UserName</c> ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteUserName(User pUser, DbContexto dbContexto)
        {
            return await dbContexto.User.AnyAsync(
                u => u.UserName == pUser.UserName && u.UserId != pUser.UserId);
        }

        /// <summary>
        /// Verifica si existe un <c>Email</c> duplicado en la base de datos,
        /// excluyendo el propio registro del usuario en caso de modificación.
        /// </summary>
        /// <param name="pUser">Objeto <see cref="User"/> con el <c>Email</c> a validar.</param>
        /// <param name="dbContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el <c>Email</c> ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteEmail(User pUser, DbContexto dbContexto)
        {
            return await dbContexto.User.AnyAsync(
                u => u.Email == pUser.Email && u.UserId != pUser.UserId);
        }

        /// <summary>
        /// Aplica los filtros de búsqueda contenidos en <see cref="PagedQuery{User}"/>
        /// a una consulta <see cref="IQueryable{User}"/> base.
        /// No aplica paginación; esta responsabilidad recae en <see cref="BuscarAsync"/>,
        /// lo que permite reutilizar este método para conteo sin Skip/Take.
        /// </summary>
        /// <param name="pQuery">Consulta base sin filtros aplicados.</param>
        /// <param name="pPagedQuery">Parámetros de filtro, rango de fechas y paginación.</param>
        /// <returns>
        /// <see cref="IQueryable{User}"/> con todos los filtros y el ordenamiento aplicados.
        /// </returns>
        private static IQueryable<User> QuerySelect(
            IQueryable<User> pQuery,
            PagedQuery<User> pPagedQuery)
        {
            var f = pPagedQuery.Filter;

            if (f.UserId > 0)
                pQuery = pQuery.Where(u => u.UserId == f.UserId);
            if (!string.IsNullOrWhiteSpace(f.UserName))
                pQuery = pQuery.Where(u => u.UserName!.Contains(f.UserName));
            if (!string.IsNullOrWhiteSpace(f.Email))
                pQuery = pQuery.Where(u => u.Email!.Contains(f.Email));
            if (f.RolId > 0)
                pQuery = pQuery.Where(u => u.RolId == f.RolId);
            if (f.StatusId > 0)
                pQuery = pQuery.Where(u => u.StatusId == f.StatusId);
            if (pPagedQuery.FromDate.HasValue)
                pQuery = pQuery.Where(u => u.CreatedAt >= pPagedQuery.FromDate.Value);
            if (pPagedQuery.ToDate.HasValue)
                pQuery = pQuery.Where(u => u.CreatedAt <= pPagedQuery.ToDate.Value);

            return pQuery.OrderBy(u => u.UserName);
        }
        #endregion

        #region "CRUD"

        /// <summary>
        /// Registra un nuevo usuario en la base de datos.
        /// Valida unicidad de <c>UserName</c> y <c>Email</c> antes de guardar.
        /// La contraseña es encriptada con SHA256 antes de persistirse.
        /// </summary>
        /// <param name="pUser">Objeto <see cref="User"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>UserName</c> o <c>Email</c> ya existen,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> GuardarAsync(User pUser)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteUserName(pUser, dbContexto))
                        throw new Exception("El nombre de usuario ya existe.");
                    if (await ExisteEmail(pUser, dbContexto))
                        throw new Exception("El correo electrónico ya está registrado.");

                    pUser.PasswordHash = EncriptarSHA256(pUser.PasswordHash!);
                    pUser.MustChangePassword = false;
                    pUser.CreatedAt = DateTime.UtcNow;

                    dbContexto.Add(pUser);
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
        /// Modifica los datos de un usuario existente en la base de datos.
        /// Valida unicidad de <c>UserName</c> y <c>Email</c> antes de actualizar.
        /// No permite modificar la contraseña desde este método.
        /// </summary>
        /// <param name="pUser">
        /// Objeto <see cref="User"/> con el <c>UserId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el usuario no existe, si el <c>UserName</c> o <c>Email</c> ya existen,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(User pUser)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteUserName(pUser, dbContexto))
                        throw new Exception("El nombre de usuario ya existe.");
                    if (await ExisteEmail(pUser, dbContexto))
                        throw new Exception("El correo electrónico ya está registrado.");

                    var user = await dbContexto.User.FirstOrDefaultAsync(
                        u => u.UserId == pUser.UserId);

                    if (user == null)
                        throw new Exception($"No se encontró el usuario con ID {pUser.UserId}.");

                    user.UserName = pUser.UserName;
                    user.Email = pUser.Email;
                    user.RolId = pUser.RolId;
                    user.PersonId = pUser.PersonId;
                    user.StatusId = pUser.StatusId;

                    dbContexto.Update(user);
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
        /// Realiza una eliminación lógica de un usuario, cambiando su estado en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pUser">
        /// Objeto <see cref="User"/> con el <c>UserId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el usuario no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(User pUser)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var user = await dbContexto.User.FirstOrDefaultAsync(
                        u => u.UserId == pUser.UserId);

                    if (user == null)
                        throw new Exception($"No se encontró el usuario con ID {pUser.UserId}.");

                    // Eliminación lógica: se cambia el estado del usuario
                    // en lugar de eliminarlo físicamente de la base de datos.
                    user.StatusId = pUser.StatusId;

                    dbContexto.Update(user);
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
        /// Obtiene una lista de usuarios aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pUser">
        /// Objeto <see cref="User"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>UserName</c>: filtra por coincidencia parcial en el nombre de usuario (null = sin filtro).</description></item>
        ///   <item><description><c>RolId</c>: filtra por rol asignado (0 = sin filtro).</description></item>
        ///   <item><description><c>StatusId</c>: filtra por estado (0 = sin filtro, devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="User"/> que cumplen los filtros indicados,
        /// ordenados por nombre de usuario de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<User>> ObtenerTodosAsync(User pUser)
        {
            var result = new List<User>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.User
                        .Include(u => u.Rol)
                        .Include(u => u.Person)
                        .Include(u => u.Status)
                        .Where(u =>
                            (pUser.UserName == null || u.UserName!.Contains(pUser.UserName)) &&
                            (pUser.RolId == 0 || u.RolId == pUser.RolId) &&
                            (pUser.StatusId == 0 || u.StatusId == pUser.StatusId)
                        )
                        .OrderBy(u => u.UserName)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene un usuario específico por su identificador, incluyendo
        /// sus relaciones con <see cref="Rol"/>, <see cref="Person"/> y <see cref="Status"/>.
        /// </summary>
        /// <param name="pUser">Objeto <see cref="User"/> con el <c>UserId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="User"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<User?> ObtenerPorIdAsync(User pUser)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.User
                        .Include(u => u.Rol)
                        .Include(u => u.Person)
                        .Include(u => u.Status)
                        .FirstOrDefaultAsync(u => u.UserId == pUser.UserId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de usuarios con soporte para paginación según los criterios especificados.
        /// </summary>
        /// <param name="pPagedQuery">La consulta de paginación que define los filtros, el tamaño de página, el número de página y otros
        /// parámetros de búsqueda. No puede ser null.</param>
        /// <returns>Una tarea que representa la operación asincrónica. El resultado contiene un objeto PagedResult<User> con la
        /// lista de usuarios encontrados y la información de paginación.</returns>
        /// <exception cref="Exception">Se produce si ocurre un error durante la ejecución de la consulta o el acceso a la base de datos.</exception>
        public static async Task<PagedResult<User>> BuscarAsync(PagedQuery<User> pPagedQuery)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var baseQuery = dbContexto.User
                        .Include(u => u.Rol)
                        .Include(u => u.Person)
                        .Include(u => u.Status)
                        .AsQueryable();

                    var filtered = QuerySelect(baseQuery, pPagedQuery);
                    int total = await filtered.CountAsync();

                    List<User> items;

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

                    return new PagedResult<User>
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

        #region "Gestión de Contraseñas Temporales y Acceso al Sistema"

        /// <summary>
        /// Genera una contraseña temporal de acceso de emergencia para un usuario,
        /// a solicitud exclusiva de un administrador del sistema.
        /// <para>
        /// La contraseña temporal tiene una vigencia de 1 hora y es de un solo uso:
        /// el sistema la invalida automáticamente tras el primer inicio de sesión exitoso
        /// mediante <see cref="LogingAsync"/>.
        /// </para>
        /// </summary>
        /// <param name="UserId">
        /// Identificador del usuario al que se le generará el acceso temporal.
        /// </param>
        /// <returns>
        /// La contraseña temporal en texto plano, para ser entregada al usuario
        /// por un canal seguro (presencial, correo eléctronico, etc.).
        /// Esta es la única vez que el valor en texto plano está disponible;
        /// en base de datos solo se persiste su hash SHA256.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el usuario no existe, si está inactivo,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<string> GenerarTempAsync(int UserId)
        {
            string tempPassword = GenerarCodigoTemporal();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var user = await dbContexto.User
                        .Include(u => u.Status)
                        .FirstOrDefaultAsync(u => u.UserId == UserId);
                    if (user == null)
                        throw new Exception($"No se encontró el usuario con ID {UserId}.");
                    if (user.Status == null || user.Status.Name != "Activo")
                        throw new Exception("No se puede genrear acceso temporal a un usuario inactivo.");
                    user.TempPasswordHash = EncriptarSHA256(tempPassword);
                    user.TempPasswordExpiry = DateTime.UtcNow.AddHours(1); // La contraseña temporal es válida por 1 hora
                    user.MustChangePassword = true; // Requiere cambio de contraseña en el próximo inicio de sesión

                    dbContexto.Update(user);
                    await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            // Para una futura implementación
            // la contraseña temporal se enviaría al usuario por correo electrónico.
            return tempPassword;
        }

        /// <summary>
        /// Valida de forma asíncrona las credenciales de un usuario utilizando su correo electrónico y contraseña, y
        /// devuelve el usuario autenticado si la autenticación es exitosa.
        /// </summary>
        /// <remarks>El método admite tanto contraseñas normales como contraseñas temporales válidas. Si
        /// existe una contraseña temporal activa, se prioriza su validación. El método no diferencia el motivo del
        /// fallo en la autenticación en el mensaje de excepción.</remarks>
        /// <param name="email">La dirección de correo electrónico del usuario que intenta iniciar sesión. No puede ser nula ni vacía.</param>
        /// <param name="password">La contraseña proporcionada para la autenticación. No puede ser nula ni vacía.</param>
        /// <returns>Una instancia de <see cref="User"/> correspondiente al usuario autenticado si las credenciales son válidas.</returns>
        /// <exception cref="Exception">Se produce si no se encuentra un usuario con el correo electrónico especificado o si la contraseña es
        /// incorrecta.</exception>
        public static async Task<User> LogingAsync(string pEmail, string pPassword)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var user = await dbContexto.User
                        .Include(u => u.Rol)
                        .Include(u => u.Person)
                        .Include(u => u.Status)
                        .FirstOrDefaultAsync(u => u.Email == pEmail && u.Status.Name != "Activo");
                    if (user == null)
                        throw new Exception("No se encontró un usuario con ese correo electrónico.");
                    string hash = EncriptarSHA256(pPassword);

                    bool tempPasswordValid = user.TempPasswordHash != null &&
                                         user.TempPasswordExpiry.HasValue &&
                                         user.TempPasswordExpiry.Value > DateTime.UtcNow;
                    if (tempPasswordValid && hash == user.TempPasswordHash)
                    {
                        // Invalida la contraseña temporal después del primer inicio de sesión exitoso
                        user.TempPasswordHash = null;
                        user.TempPasswordExpiry = null;

                        dbContexto.Update(user);
                        await dbContexto.SaveChangesAsync();
                        return user;
                    }

                    if (hash == user.PasswordHash)
                    {
                        return user;
                    }
                    else
                    {
                        throw new Exception("Contraseña incorrecta.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Cambia la contraseña de un usuario de forma asíncrona.
        /// </summary>
        /// <remarks>La contraseña se almacena como un hash seguro. Asegúrese de que la nueva contraseña
        /// cumpla con los requisitos de seguridad de la aplicación.</remarks>
        /// <param name="pUserId">El identificador único del usuario cuya contraseña se va a cambiar.</param>
        /// <param name="pNewPassword">La nueva contraseña que se asignará al usuario. No puede ser null.</param>
        /// <returns>Un valor entero que indica el número de registros afectados en la base de datos. Devuelve 0 si no se realizó
        /// ningún cambio.</returns>
        /// <exception cref="Exception">Se produce si no se encuentra el usuario especificado o si ocurre un error durante la actualización de la
        /// contraseña.</exception>
        public static async Task<int> ChangePasswordAsync(int pUserId, string pNewPassword)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var user = await dbContexto.User.FirstOrDefaultAsync(
                        u => u.UserId == pUserId);
                    if (user == null)
                        throw new Exception($"No se encontró el usuario con ID {pUserId}.");
                    if (!user.MustChangePassword)
                        throw new Exception(
                            "El usuario no tiene cambio de contraseña obligatorio.");

                    user.PasswordHash = EncriptarSHA256(pNewPassword);
                    user.MustChangePassword = false; // El usuario ha cambiado su contraseña temporal, ya no es obligatorio cambiarla en el próximo inicio de sesión


                    dbContexto.Update(user);
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

        #endregion
    }
}