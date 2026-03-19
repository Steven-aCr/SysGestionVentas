
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace BDGestionVentas.DAL
{
    public class PermissionDAL
    {
        private static async Task<bool> ExisteNombre(Permission pPermission,
            DbContexto pDBContexto)
        {
            bool result = false;
            var existe = await pDBContexto.Permission.FirstOrDefaultAsync(
                p => p.Name == pPermission.Name &&
                     p.PermissionId != pPermission.PermissionId);
            if (existe != null && existe.PermissionId > 0)
                result = true;
            return result;
        }

        public static async Task<int> GuardarAsync(Permission pPermission)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    bool existeNombre = await ExisteNombre(pPermission, dbContexto);
                    if (existeNombre == false)
                    {
                        pPermission.CreatedAt = DateTime.Now;
                        dbContexto.Add(pPermission);
                        result = await dbContexto.SaveChangesAsync();
                    }
                    else
                        throw new Exception("El permiso ya existe.");
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<int> ModificarAsync(Permission pPermission)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    bool existeNombre = await ExisteNombre(pPermission, dbContexto);
                    if (existeNombre == false)
                    {
                        var permission = await dbContexto.Permission.FirstOrDefaultAsync(
                            p => p.PermissionId == pPermission.PermissionId);

                        permission.Name = pPermission.Name;
                        permission.Description = pPermission.Description;

                        dbContexto.Update(permission);
                        result = await dbContexto.SaveChangesAsync();
                    }
                    else
                        throw new Exception("El permiso ya existe.");
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<int> EliminarAsync(Permission pPermission)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var permission = await dbContexto.Permission.FirstOrDefaultAsync(
                        p => p.PermissionId == pPermission.PermissionId);

                    dbContexto.Remove(permission);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<Permission> ObtenerPorIdAsync(Permission pPermission)
        {
            var result = new Permission();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Permission
                        .FirstOrDefaultAsync(p => p.PermissionId == pPermission.PermissionId);
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<List<Permission>> ObtenerTodosAsync(Permission pPermission)
        {
            var result = new List<Permission>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Permission
                        .Where(p =>
                            (pPermission.Name == null || p.Name.Contains(pPermission.Name))
                        )
                        .OrderBy(p => p.Name)
                        .ToListAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }
    }
}