using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;

namespace SysSeguridad2G05.DAL
{
    public class ProductListDAL
    {
        /// <summary>
        /// Verifica si ya existe un producto con el mismo código de barras.
        /// </summary>
        /// <param name="pProduct">Objeto <see cref="ProductList"/> a validar.</param>
        /// <param name="pDBContexto">Contexto de base de datos.</param>
        /// <returns>
        /// <c>true</c> si el código de barras ya existe, <c>false</c> en caso contrario.
        /// </returns>
        private static async Task<bool> ExisteBarcode(ProductList pProduct, DbContexto pDBContexto)
        {
            var productoExiste = await pDBContexto.ProductList.FirstOrDefaultAsync(
                p => p.Barcode == pProduct.Barcode && p.ProductId != pProduct.ProductId);

            return (productoExiste != null && productoExiste.ProductId > 0);
        }

        /// <summary>
        /// Registra un nuevo producto en la base de datos.
        /// </summary>
        /// <param name="pProduct">Objeto <see cref="ProductList"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el código de barras ya existe o si ocurre un error.
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
        /// Modifica los datos de un producto existente.
        /// </summary>
        /// <param name="pProduct">
        /// Objeto <see cref="ProductList"/> con el <c>ProductId</c> y los nuevos valores.
        /// </param>
        /// <returns>Número de filas afectadas.</returns>
        /// <exception cref="Exception">
        /// Se lanza si no existe el producto o si el código de barras está duplicado.
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
                    product.CategoryId = pProduct.CategoryId;
                    product.StatusId = pProduct.StatusId;
                    product.CreatedByUser = pProduct.CreatedByUser;

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
        /// Realiza una eliminación lógica del producto.
        /// </summary>
        /// <param name="pProduct">
        /// Objeto <see cref="ProductList"/> con el <c>ProductId</c> y el nuevo <c>StatusId</c>.
        /// </param>
        /// <returns>Número de filas afectadas.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el producto no existe.
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

                    // Eliminación lógica
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
        /// Obtiene un producto por su ID incluyendo sus relaciones.
        /// </summary>
        /// <param name="pProduct">Objeto <see cref="ProductList"/> con el ID a buscar.</param>
        /// <returns>Producto encontrado o <c>null</c>.</returns>
        public static async Task<ProductList> ObtenerPorIdAsync(ProductList pProduct)
        {
            var result = new ProductList();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.ProductList
                        .Include(p => p.Category)
                        .Include(p => p.Status)
                        .FirstOrDefaultAsync(p => p.ProductId == pProduct.ProductId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene una lista de productos aplicando filtros opcionales.
        /// </summary>
        /// <param name="pProduct">
        /// Filtros:
        /// <list type="bullet">
        /// <item><description><c>Name</c>: búsqueda por nombre.</description></item>
        /// <item><description><c>CategoryId</c>: filtro por categoría.</description></item>
        /// <item><description><c>StatusId</c>: filtro por estado.</description></item>
        /// </list>
        /// </param>
        /// <returns>Lista de productos.</returns>
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
                        .Where(p =>
                            (pProduct.Name == null || p.Name.Contains(pProduct.Name)) &&
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
    }
}