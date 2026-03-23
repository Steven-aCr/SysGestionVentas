using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;

namespace SysGestionVentas.DAL
{
    public class CategoryDAL
    {
        /// <summary>
        /// Registra una nueva categoría en la base de datos.
        /// </summary>
        /// <param name="pCategory">Objeto <see cref="Category"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la operación.</exception>
        public static async Task<int> GuardarAsync(Category pCategory)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    pCategory.CreatedAt = DateTime.UtcNow;
                    dbContexto.Add(pCategory);
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
        /// Modifica los datos de una categoría existente en la base de datos.
        /// </summary>
        /// <param name="pCategory">
        /// Objeto <see cref="Category"/> con el <c>CategoryId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si la categoría no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Category pCategory)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var category = await dbContexto.Category.FirstOrDefaultAsync(
                        c => c.CategoryId == pCategory.CategoryId);

                    if (category == null)
                        throw new Exception($"No se encontró la categoría con ID {pCategory.CategoryId}.");

                    category.Name = pCategory.Name;
                    category.Description = pCategory.Description;
                    category.StatusId = pCategory.StatusId;

                    dbContexto.Update(category);
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
        /// Realiza una eliminación lógica de una categoría, cambiando su estado en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pCategory">
        /// Objeto <see cref="Category"/> con el <c>CategoryId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si la categoría no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Category pCategory)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var category = await dbContexto.Category.FirstOrDefaultAsync(
                        c => c.CategoryId == pCategory.CategoryId);

                    if (category == null)
                        throw new Exception($"No se encontró la categoría con ID {pCategory.CategoryId}.");

                    // Eliminación lógica: se cambia el estado de la categoría
                    // en lugar de eliminarla físicamente de la base de datos.
                    category.StatusId = pCategory.StatusId;

                    dbContexto.Update(category);
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
        /// Obtiene una categoría específica por su identificador, incluyendo
        /// sus relaciones con <see cref="Status"/> y <see cref="User"/> creador.
        /// </summary>
        /// <param name="pCategory">Objeto <see cref="Category"/> con el <c>CategoryId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Category"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Category?> ObtenerPorIdAsync(Category pCategory)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Category
                        .Include(c => c.Status)
                        .Include(c => c.CreatedBy)
                        .FirstOrDefaultAsync(c => c.CategoryId == pCategory.CategoryId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de categorías aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pCategory">
        /// Objeto <see cref="Category"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>Name</c>: filtra por coincidencia parcial en el nombre (null = sin filtro).</description></item>
        ///   <item><description><c>StatusId</c>: filtra por estado (0 = sin filtro, devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Category"/> que cumplen los filtros indicados,
        /// ordenados por nombre de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Category>> ObtenerTodosAsync(Category pCategory)
        {
            var result = new List<Category>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Category
                        .Include(c => c.Status)
                        .Include(c => c.CreatedBy)
                        .Where(c =>
                            (pCategory.Name == null || c.Name!.Contains(pCategory.Name)) &&
                            (pCategory.StatusId == 0 || c.StatusId == pCategory.StatusId)
                        )
                        .OrderBy(c => c.Name)
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