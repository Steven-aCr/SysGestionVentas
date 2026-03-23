using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.Security.Cryptography;
using System.Text;

namespace SysGestionVentas.DAL
{
    public class UsersDAL
    {
        /// <summary>
        /// Encripta la contraseña del usuario utilizando SHA256.
        /// </summary>
        /// <param name="pUser">Objeto <see cref="User"/> con la contraseña en texto plano a encriptar.
        /// El campo <c>PasswordHash</c> será reemplazado por su representación hash.</param>
        private static void EncriptarSHA256(User pUser)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pUser.PasswordHash!));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                pUser.PasswordHash = sb.ToString();
            }
        }

        /// <summary>
        /// Verifica si existe un <c>UserName</c> duplicado en la base de datos,
        /// excluyendo el propio registro del usuario en caso de modificación.
        /// </summary>
        /// <param name="pUser">Objeto <see cref="User"/> con el <c>UserName</c> a validar.</param>
        /// <param name="pDBContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el <c>UserName</c> ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteUserName(User pUser, DbContexto pDBContexto)
        {
            return await pDBContexto.User.AnyAsync(
                u => u.UserName == pUser.UserName && u.UserId != pUser.UserId);
        }

        /// <summary>
        /// Verifica si existe un <c>Email</c> duplicado en la base de datos,
        /// excluyendo el propio registro del usuario en caso de modificación.
        /// </summary>
        /// <param name="pUser">Objeto <see cref="User"/> con el <c>Email</c> a validar.</param>
        /// <param name="pDBContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el <c>Email</c> ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteEmail(User pUser, DbContexto pDBContexto)
        {
            return await pDBContexto.User.AnyAsync(
                u => u.Email == pUser.Email && u.UserId != pUser.UserId);
        }

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

                    EncriptarSHA256(pUser);
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
        /// Modifica la contraseña de un usuario existente en la base de datos.
        /// La nueva contraseña es encriptada con SHA256 antes de persistirse.
        /// </summary>
        /// <param name="pUser">
        /// Objeto <see cref="User"/> con el <c>UserId</c> del registro
        /// y el nuevo valor de <c>PasswordHash</c> en texto plano.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el usuario no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarContrasenaAsync(User pUser)
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

                    EncriptarSHA256(pUser);
                    user.PasswordHash = pUser.PasswordHash;

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
    }
}