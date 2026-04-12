using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;

namespace SysGestionVentas.DAL
{
    public class ProductListDAL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Verifica si ya existe un producto con el mismo código de barras en la base de datos,
        /// excluyendo el propio registro en caso de modificación.
        /// </summary>
        /// <param name="pProduct">Objeto <see cref="ProductList"/> con el <c>Barcode</c> a validar.</param>
        /// <param name="pDBContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el código de barras ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteBarcode(ProductList pProduct, DbContexto pDBContexto)
        {
            return await pDBContexto.ProductList.AnyAsync(
                p => p.Barcode == pProduct.Barcode && p.ProductId != pProduct.ProductId);
        }

        /// <summary>
        /// Aplica los filtros de búsqueda contenidos en <see cref="PagedQuery{ProductList}"/>
        /// a una consulta <see cref="IQueryable{ProductList}"/> base.
        /// No aplica paginación; esta responsabilidad recae en <see cref="BuscarAsync"/>,
        /// lo que permite reutilizar este método para conteo sin Skip/Take.
        /// </summary>
        /// <param name="pQuery">Consulta base sin filtros aplicados.</param>
        /// <param name="pPagedQuery">Parámetros de filtro, rango de fechas y paginación.</param>
        /// <returns>
        /// <see cref="IQueryable{ProductList}"/> con todos los filtros y el ordenamiento aplicados.
        /// </returns>
        private static IQueryable<ProductList> QuerySelect(
            IQueryable<ProductList> pQuery,
            PagedQuery<ProductList> pPagedQuery)
        {
            var f = pPagedQuery.Filter;

            if (f.ProductId > 0)
                pQuery = pQuery.Where(p => p.ProductId == f.ProductId);

            if (!string.IsNullOrWhiteSpace(f.Name))
                pQuery = pQuery.Where(p => p.Name!.Contains(f.Name));

            if (!string.IsNullOrWhiteSpace(f.Description))
                pQuery = pQuery.Where(p => p.Description!.Contains(f.Description));

            if (!string.IsNullOrWhiteSpace(f.Barcode))
                pQuery = pQuery.Where(p => p.Barcode!.Contains(f.Barcode));

            if (f.StatusId > 0)
                pQuery = pQuery.Where(p => p.StatusId == f.StatusId);

            if (f.CategoryId > 0)
                pQuery = pQuery.Where(p => p.CategoryId == f.CategoryId);

            if (f.CreatedByUser > 0)
                pQuery = pQuery.Where(p => p.CreatedByUser == f.CreatedByUser);

            if (pPagedQuery.FromDate.HasValue)
                pQuery = pQuery.Where(p => p.CreatedAt >= pPagedQuery.FromDate.Value);

            if (pPagedQuery.ToDate.HasValue)
                pQuery = pQuery.Where(p => p.CreatedAt <= pPagedQuery.ToDate.Value);

            return pQuery.OrderBy(p => p.Name);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Registra un nuevo producto en la base de datos.
        /// Valida unicidad del <c>Barcode</c> antes de guardar.
        /// </summary>
        /// <param name="pProduct">Objeto <see cref="ProductList"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el código de barras ya existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> GuardarAsync(ProductList pProduct)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteBarcode(pProduct, dbContexto))
                        throw new Exception("El código de barras ya está registrado.");

                    pProduct.CreatedAt = DateTime.UtcNow;
                    dbContexto.Add(pProduct);
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
        /// Modifica los datos de un producto existente en la base de datos.
        /// Valida unicidad del <c>Barcode</c> antes de actualizar.
        /// El campo <c>CreatedByUser</c> no es modificable por ser un campo de auditoría de creación.
        /// </summary>
        /// <param name="pProduct">
        /// Objeto <see cref="ProductList"/> con el <c>ProductId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el producto no existe, si el código de barras está duplicado,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(ProductList pProduct)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteBarcode(pProduct, dbContexto))
                        throw new Exception("El código de barras ya está registrado.");

                    var product = await dbContexto.ProductList.FirstOrDefaultAsync(
                        p => p.ProductId == pProduct.ProductId);

                    if (product == null)
                        throw new Exception($"No se encontró el producto con ID {pProduct.ProductId}.");

                    product.Name = pProduct.Name;
                    product.Description = pProduct.Description;
                    product.Barcode = pProduct.Barcode;
                    product.ImageUrl = pProduct.ImageUrl;
                    product.CategoryId = pProduct.CategoryId;
                    product.StatusId = pProduct.StatusId;

                    dbContexto.Update(product);
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
        /// Realiza una eliminación lógica de un producto, cambiando su estado en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pProduct">
        /// Objeto <see cref="ProductList"/> con el <c>ProductId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el producto no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(ProductList pProduct)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var product = await dbContexto.ProductList.FirstOrDefaultAsync(
                        p => p.ProductId == pProduct.ProductId);

                    if (product == null)
                        throw new Exception($"No se encontró el producto con ID {pProduct.ProductId}.");

                    // Eliminación lógica: se cambia el estado del producto
                    // en lugar de eliminarlo físicamente de la base de datos.
                    product.StatusId = pProduct.StatusId;

                    dbContexto.Update(product);
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
        /// Obtiene un producto específico por su identificador, incluyendo sus relaciones
        /// con <see cref="Category"/>, <see cref="Status"/> y el <see cref="User"/> creador.
        /// </summary>
        /// <param name="pProduct">Objeto <see cref="ProductList"/> con el <c>ProductId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="ProductList"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<ProductList?> ObtenerPorIdAsync(ProductList pProduct)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.ProductList
                        .Include(p => p.Category)
                        .Include(p => p.Status)
                        .Include(p => p.CreatedBy)
                        .Include(p => p.Inventory)
                        .FirstOrDefaultAsync(p => p.ProductId == pProduct.ProductId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de productos aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pProduct">
        /// Objeto <see cref="ProductList"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>Name</c>: filtra por coincidencia parcial en el nombre (null = sin filtro).</description></item>
        ///   <item><description><c>CategoryId</c>: filtra por categoría (0 = sin filtro).</description></item>
        ///   <item><description><c>StatusId</c>: filtra por estado (0 = sin filtro, devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="ProductList"/> que cumplen los filtros indicados,
        /// ordenados por nombre de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<ProductList>> ObtenerTodosAsync(ProductList pProduct)
        {
            var result = new List<ProductList>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.ProductList
                        .Include(p => p.Category)
                        .Include(p => p.Status)
                        .Include(p => p.CreatedBy)
                        .Include(p => p.Inventory)
                        .Where(p =>
                            (pProduct.Name == null || p.Name!.Contains(pProduct.Name)) &&
                            (pProduct.CategoryId == 0 || p.CategoryId == pProduct.CategoryId) &&
                            (pProduct.StatusId == 0 || p.StatusId == pProduct.StatusId)
                        )
                        .OrderBy(p => p.Name)
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
        /// Realiza una búsqueda avanzada de productos con soporte para paginación
        /// según los criterios especificados en <paramref name="pPagedQuery"/>.
        /// Si <c>Top</c> es mayor a cero, devuelve únicamente los primeros <c>Top</c> registros
        /// ignorando los parámetros de paginación.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{ProductList}"/> que define los filtros, el tamaño de página,
        /// el número de página y otros parámetros de búsqueda. No puede ser <c>null</c>.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{ProductList}"/> con la lista de productos encontrados
        /// e información de paginación (total de registros, página actual, tamaño de página).
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre un error durante la ejecución de la consulta o el acceso a la base de datos.
        /// </exception>
        public static async Task<PagedResult<ProductList>> BuscarAsync(PagedQuery<ProductList> pPagedQuery)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var baseQuery = dbContexto.ProductList
                        .Include(p => p.Status)
                        .Include(p => p.Category)
                        .Include(p => p.CreatedBy)
                        .Include(p => p.Inventory)
                        .AsQueryable();

                    var filtered = QuerySelect(baseQuery, pPagedQuery);
                    int total = await filtered.CountAsync();

                    List<ProductList> items;

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

                    return new PagedResult<ProductList>
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