using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using Microsoft.EntityFrameworkCore;

namespace SysGestionVentas.DAL
{
    public class SupplierDAL
    {
        #region "Métodos Privados"

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
        /// Aplica los filtros de búsqueda contenidos en <see cref="PagedQuery{Supplier}"/>
        /// a una consulta <see cref="IQueryable{Supplier}"/> base.
        /// No aplica paginación; esta responsabilidad recae en <see cref="BuscarAsync"/>,
        /// lo que permite reutilizar este método para conteo sin Skip/Take.
        /// </summary>
        /// <param name="pQuery">Consulta base sin filtros aplicados.</param>
        /// <param name="pPagedQuery">Parámetros de filtro, rango de fechas y paginación.</param>
        /// <returns>
        /// <see cref="IQueryable{Supplier}"/> con todos los filtros y el ordenamiento aplicados,
        /// ordenado por nombre de empresa de forma ascendente.
        /// </returns>
        private static IQueryable<Supplier> QuerySelect(
            IQueryable<Supplier> pQuery,
            PagedQuery<Supplier> pPagedQuery)
        {
            var f = pPagedQuery.Filter;

            if (f.SupplierId > 0)
                pQuery = pQuery.Where(s => s.SupplierId == f.SupplierId);

            if (f.PersonId > 0)
                pQuery = pQuery.Where(s => s.PersonId == f.PersonId);

            if (!string.IsNullOrWhiteSpace(f.CompanyName))
                pQuery = pQuery.Where(s => s.CompanyName!.Contains(f.CompanyName));

            if (!string.IsNullOrWhiteSpace(f.Nit))
                pQuery = pQuery.Where(s => s.Nit!.Contains(f.Nit));

            if (!string.IsNullOrWhiteSpace(f.Nrc))
                pQuery = pQuery.Where(s => s.Nrc!.Contains(f.Nrc));

            if (f.StatusId > 0)
                pQuery = pQuery.Where(s => s.StatusId == f.StatusId);

            return pQuery.OrderBy(s => s.CompanyName);
        }

        #endregion

        #region "CRUD"

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
        /// sus relaciones con <see cref="Person"/>, el <see cref="Status"/> de la persona
        /// y el <see cref="Status"/> propio del proveedor.
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

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de proveedores con soporte para paginación
        /// según los criterios especificados en <paramref name="pPagedQuery"/>.
        /// Si <c>Top</c> es mayor a cero, devuelve únicamente los primeros <c>Top</c> registros
        /// ignorando los parámetros de paginación.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Supplier}"/> que define los filtros, el tamaño de página,
        /// el número de página y otros parámetros de búsqueda. No puede ser <c>null</c>.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Supplier}"/> con la lista de proveedores encontrados
        /// e información de paginación (total de registros, página actual, tamaño de página).
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre un error durante la ejecución de la consulta o el acceso a la base de datos.
        /// </exception>
        public static async Task<PagedResult<Supplier>> BuscarAsync(PagedQuery<Supplier> pPagedQuery)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var baseQuery = dbContexto.Supplier
                        .Include(s => s.Person)
                            .ThenInclude(p => p!.Status)
                        .Include(s => s.Status)
                        .AsQueryable();

                    var filtered = QuerySelect(baseQuery, pPagedQuery);
                    int total = await filtered.CountAsync();

                    List<Supplier> items;

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

                    return new PagedResult<Supplier>
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
    }
}