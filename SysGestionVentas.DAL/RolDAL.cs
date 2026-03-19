
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace BDGestionVentas.DAL
{
    public class RolDAL
    {
        private static async Task<bool> ExisteNombre(Rol pRol, DbContexto pDBContexto)
        {
            bool result = false;
            var existe = await pDBContexto.Rol.FirstOrDefaultAsync(
                r => r.Name == pRol.Name && r.RolId != pRol.RolId);
            if (existe != null && existe.RolId > 0)
                result = true;
            return result;
        }

        public static async Task<int> GuardarAsync(Rol pRol)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    bool existeNombre = await ExisteNombre(pRol, dbContexto);
                    if (existeNombre == false)
                    {
                        pRol.CreatedAt = DateTime.Now;
                        dbContexto.Add(pRol);
                        result = await dbContexto.SaveChangesAsync();
                    }
                    else
                        throw new Exception("El nombre del rol ya existe.");
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<int> ModificarAsync(Rol pRol)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    bool existeNombre = await ExisteNombre(pRol, dbContexto);
                    if (existeNombre == false)
                    {
                        var rol = await dbContexto.Rol.FirstOrDefaultAsync(
                            r => r.RolId == pRol.RolId);

                        rol.Name = pRol.Name;
                        rol.Description = pRol.Description;

                        dbContexto.Update(rol);
                        result = await dbContexto.SaveChangesAsync();
                    }
                    else
                        throw new Exception("El nombre del rol ya existe.");
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<int> EliminarAsync(Rol pRol)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var rol = await dbContexto.Rol.FirstOrDefaultAsync(
                        r => r.RolId == pRol.RolId);

                    dbContexto.Remove(rol);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<Rol> ObtenerPorIdAsync(Rol pRol)
        {
            var result = new Rol();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Rol
                        .FirstOrDefaultAsync(r => r.RolId == pRol.RolId);
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }

        public static async Task<List<Rol>> ObtenerTodosAsync(Rol pRol)
        {
            var result = new List<Rol>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Rol
                        .Where(r =>
                            (pRol.Name == null || r.Name.Contains(pRol.Name))
                        )
                        .OrderBy(r => r.Name)
                        .ToListAsync();
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message); }
            return result;
        }
    }
}