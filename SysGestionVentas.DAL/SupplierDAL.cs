using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;

namespace SysGestionVentas.DAL
{
    public class SupplierDAL
    {
        /// <summary>
        /// Verifica si ya existe un proveedor con el mismo NIT en la base de datos,
        /// excluyendo el propio registro en caso de modificación.
        /// </summary>
        /// <param name="pSupplier">Objeto <see cref="Supplier"/> con el <c>Nit</c> a validar.</param>
        /// <param name="pDBContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el NIT ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteNit(Supplier pSupplier, DbContexto pDBContexto)
        {
            return await pDBContexto.Supplier.AnyAsync(
                s => s.Nit == pSupplier.Nit && s.SupplierId != pSupplier.SupplierId);
        }

        /// <summary>
        /// Verifica si ya existe un proveedor con el mismo NRC en la base de datos,
        /// excluyendo el propio registro en caso de modificación.
        /// </summary>
        /// <param name="pSupplier">Objeto <see cref="Supplier"/> con el <c>Nrc</c> a validar.</param>
        /// <param name="pDBContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el NRC ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteNrc(Supplier pSupplier, DbContexto pDBContexto)
        {
            return await pDBContexto.Supplier.AnyAsync(
                s => s.Nrc == pSupplier.Nrc && s.SupplierId != pSupplier.SupplierId);
        }

        /// <summary>
        /// Registra un nuevo proveedor en la base de datos.
        /// Valida unicidad de <c>Nit</c> y <c>Nrc</c> antes de guardar.
        /// </summary>
        /// <param name="pSupplier">Objeto <see cref="Supplier"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el NIT o NRC ya existen, o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> GuardarAsync(Supplier pSupplier)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteNit(pSupplier, dbContexto))
                        throw new Exception("El NIT ya está registrado.");

                    if (await ExisteNrc(pSupplier, dbContexto))
                        throw new Exception("El NRC ya está registrado.");

                    dbContexto.Add(pSupplier);
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
        /// Modifica los datos de un proveedor existente en la base de datos.
        /// Valida unicidad de <c>Nit</c> y <c>Nrc</c> antes de actualizar.
        /// </summary>
        /// <param name="pSupplier">
        /// Objeto <see cref="Supplier"/> con el <c>SupplierId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el proveedor no existe, si hay duplicados de NIT o NRC,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Supplier pSupplier)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteNit(pSupplier, dbContexto))
                        throw new Exception("El NIT ya está registrado.");

                    if (await ExisteNrc(pSupplier, dbContexto))
                        throw new Exception("El NRC ya está registrado.");

                    var supplier = await dbContexto.Supplier.FirstOrDefaultAsync(
                        s => s.SupplierId == pSupplier.SupplierId);

                    if (supplier == null)
                        throw new Exception($"No se encontró el proveedor con ID {pSupplier.SupplierId}.");

                    supplier.CompanyName = pSupplier.CompanyName;
                    supplier.Nit = pSupplier.Nit;
                    supplier.Nrc = pSupplier.Nrc;
                    supplier.Description = pSupplier.Description;
                    supplier.PersonId = pSupplier.PersonId;
                    supplier.StatusId = pSupplier.StatusId;

                    dbContexto.Update(supplier);
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
        /// Realiza una eliminación lógica de un proveedor, cambiando su estado en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pSupplier">
        /// Objeto <see cref="Supplier"/> con el <c>SupplierId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el proveedor no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Supplier pSupplier)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var supplier = await dbContexto.Supplier.FirstOrDefaultAsync(
                        s => s.SupplierId == pSupplier.SupplierId);

                    if (supplier == null)
                        throw new Exception($"No se encontró el proveedor con ID {pSupplier.SupplierId}.");

                    // Eliminación lógica: se cambia el estado del proveedor
                    // en lugar de eliminarlo físicamente de la base de datos.
                    supplier.StatusId = pSupplier.StatusId;

                    dbContexto.Update(supplier);
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
        /// Obtiene un proveedor específico por su identificador, incluyendo
        /// sus relaciones con <see cref="Person"/> y <see cref="Status"/>.
        /// </summary>
        /// <param name="pSupplier">Objeto <see cref="Supplier"/> con el <c>SupplierId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Supplier"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Supplier?> ObtenerPorIdAsync(Supplier pSupplier)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Supplier
                        .Include(s => s.Person)
                            .ThenInclude(p => p!.Status)
                        .Include(s => s.Status)
                        .FirstOrDefaultAsync(s => s.SupplierId == pSupplier.SupplierId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de proveedores aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pSupplier">
        /// Objeto <see cref="Supplier"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>CompanyName</c>: filtra por coincidencia parcial en el nombre de empresa (null = sin filtro).</description></item>
        ///   <item><description><c>Nit</c>: filtra por coincidencia parcial en el NIT (null = sin filtro).</description></item>
        ///   <item><description><c>StatusId</c>: filtra por estado (0 = sin filtro, devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Supplier"/> que cumplen los filtros indicados,
        /// ordenados por nombre de empresa de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Supplier>> ObtenerTodosAsync(Supplier pSupplier)
        {
            var result = new List<Supplier>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Supplier
                        .Include(s => s.Person)
                            .ThenInclude(p => p!.Status)
                        .Include(s => s.Status)
                        .Where(s =>
                            (pSupplier.CompanyName == null || s.CompanyName!.Contains(pSupplier.CompanyName)) &&
                            (pSupplier.Nit == null || s.Nit!.Contains(pSupplier.Nit)) &&
                            (pSupplier.StatusId == 0 || s.StatusId == pSupplier.StatusId)
                        )
                        .OrderBy(s => s.CompanyName)
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