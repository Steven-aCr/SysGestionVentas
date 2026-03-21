using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.Security.Cryptography;
using System.Text;

namespace BDGestionVentas.DAL
{
    public class UsersDAL
    {
        /// <summary>
        /// Encripta la contraseña del usuario utilizando MD5
        /// </summary>
        /// <param name="pUser">Usuario con contraseña en texto plano</param>
        private static void EncriptarMD5(User pUser)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(pUser.PasswordHash));
                var strEncriptar = "";
                for (int i = 0; i < result.Length; i++)
                    strEncriptar += result[i].ToString("x2").ToLower();
                pUser.PasswordHash = strEncriptar;
            }
        }

        /// <summary>
        /// Verifica si existe un UserName duplicado
        /// </summary>
        /// <param name="pUser">Usuario a validar</param>
        /// <param name="pDBContexto">Contexto de base de datos</param>
        /// <returns>True si existe, False si no</returns>

        private static async Task<bool> ExisteUserName(User pUser, DbContexto pDBContexto)
        {
            bool result = false;
            var userExiste = await pDBContexto.User.FirstOrDefaultAsync(
                u => u.UserName == pUser.UserName && u.UserId != pUser.UserId);
            if (userExiste != null && userExiste.UserId > 0)
                result = true;
            return result;
        }

        /// <summary>
        /// Verifica si existe un Email duplicado
        /// </summary>
        /// <param name="pUser">Usuario a validar</param>
        /// <param name="pDBContexto">Contexto de base de datos</param>
        /// <returns>True si existe, False si no</returns>
        private static async Task<bool> ExisteEmail(User pUser, DbContexto pDBContexto)
        {
            bool result = false;
            var emailExiste = await pDBContexto.User.FirstOrDefaultAsync(
                u => u.Email == pUser.Email && u.UserId != pUser.UserId);
            if (emailExiste != null && emailExiste.UserId > 0)
                result = true;
            return result;
        }

        #region "CRUD"

        /// <summary>
        /// Guarda un nuevo usuario en la base de datos
        /// </summary>
        /// <param name="pUser">Usuario a guardar</param>
        /// <returns>Número de registros afectados</returns>

        public static async Task<int> GuardarAsync(User pUser)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    bool existeUserName = await ExisteUserName(pUser, dbContexto);
                    bool existeEmail = await ExisteEmail(pUser, dbContexto);

                    if (existeUserName)
                        throw new Exception("El UserName ya existe.");
                    if (existeEmail)
                        throw new Exception("El Email ya está registrado.");

                    EncriptarMD5(pUser);
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
        /// Modifica un usuario existente
        /// </summary>
        /// <param name="pUser">Usuario con datos actualizados</param>
        /// <returns>Número de registros afectados</returns>

        public static async Task<int> ModificarAsync(User pUser)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    bool existeUserName = await ExisteUserName(pUser, dbContexto);
                    bool existeEmail = await ExisteEmail(pUser, dbContexto);

                    if (existeUserName)
                        throw new Exception("El UserName ya existe.");
                    if (existeEmail)
                        throw new Exception("El Email ya está registrado.");

                    var user = await dbContexto.User.FirstOrDefaultAsync(
                        u => u.UserId == pUser.UserId);

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
        /// Elimina un usuario por su ID
        /// </summary>
        /// <param name="pUser">Usuario a eliminar</param>
        /// <returns>Número de registros afectados</returns>

        public static async Task<int> EliminarAsync(User pUser)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var user = await dbContexto.User.FirstOrDefaultAsync(
                        u => u.UserId == pUser.UserId);
                    dbContexto.Remove(user);
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
        /// Obtiene todos los usuarios con sus relaciones
        /// </summary>
        /// <param name="pUser">Parámetro opcional</param>
        /// <returns>Lista de usuarios</returns

        public static async Task<List<User>> ObtenerTodosAsync(User pUser)
        {
            var lista = new List<User>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    lista = await dbContexto.User
                        .Include(u => u.Rol)
                        .Include(u => u.Person)
                        .Include(u => u.Status)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return lista;
        }

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        /// <param name="pUser">Usuario con ID a buscar</param>
        /// <returns>Usuario encontrado</returns>
        public static async Task<User> ObtenerPorIdAsync(User pUser)
        {
            var user = new User();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    user = await dbContexto.User
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
            return user;
        }

        #endregion
    }
}
