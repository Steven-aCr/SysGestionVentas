using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;

namespace SysGestionVentas.DAL
{
    public class DiscountDAL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Verifica si ya existe un descuento con el mismo nombre en la base de datos,
        /// excluyendo el propio registro en caso de modificación.
        /// </summary>
        /// <param name="pDiscount">Objeto <see cref="Discount"/> con el <c>Name</c> a validar.</param>
        /// <param name="pDbContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el nombre ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteNombre(Discount pDiscount, DbContexto pDbContexto)
        {
            return await pDbContexto.Discount.AnyAsync(
                d => d.Name == pDiscount.Name && d.DiscountId != pDiscount.DiscountId);
        }

        /// <summary>
        /// Valida que el rango de fechas del descuento sea coherente:
        /// la fecha de fin debe ser posterior a la fecha de inicio.
        /// </summary>
        /// <param name="pDiscount">Objeto <see cref="Discount"/> con las fechas a validar.</param>
        /// <exception cref="Exception">
        /// Se lanza si <c>EndDate</c> es igual o anterior a <c>StartDate</c>.
        /// </exception>
        private static void ValidarFechas(Discount pDiscount)
        {
            if (pDiscount.EndDate <= pDiscount.StartDate)
                throw new Exception("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Registra un nuevo descuento en la base de datos.
        /// Valida unicidad del <c>Name</c> y coherencia del rango de fechas antes de guardar.
        /// </summary>
        /// <param name="pDiscount">Objeto <see cref="Discount"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el nombre ya existe, si las fechas son incoherentes,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> GuardarAsync(Discount pDiscount)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    ValidarFechas(pDiscount);

                    if (await ExisteNombre(pDiscount, dbContexto))
                        throw new Exception("El nombre del descuento ya existe.");

                    pDiscount.CreatedAt = DateTime.UtcNow;
                    dbContexto.Add(pDiscount);
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
        /// Modifica los datos de un descuento existente en la base de datos.
        /// Valida unicidad del <c>Name</c> y coherencia del rango de fechas antes de actualizar.
        /// </summary>
        /// <param name="pDiscount">
        /// Objeto <see cref="Discount"/> con el <c>DiscountId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el descuento no existe, si el nombre está duplicado,
        /// si las fechas son incoherentes, o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Discount pDiscount)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    ValidarFechas(pDiscount);

                    if (await ExisteNombre(pDiscount, dbContexto))
                        throw new Exception("El nombre del descuento ya existe.");

                    var discount = await dbContexto.Discount.FirstOrDefaultAsync(
                        d => d.DiscountId == pDiscount.DiscountId);

                    if (discount == null)
                        throw new Exception($"No se encontró el descuento con ID {pDiscount.DiscountId}.");

                    discount.Name = pDiscount.Name;
                    discount.Percentage = pDiscount.Percentage;
                    discount.StartDate = pDiscount.StartDate;
                    discount.EndDate = pDiscount.EndDate;
                    discount.StatusId = pDiscount.StatusId;

                    dbContexto.Update(discount);
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
        /// Realiza una eliminación lógica de un descuento, cambiando su estado en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pDiscount">
        /// Objeto <see cref="Discount"/> con el <c>DiscountId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el descuento no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Discount pDiscount)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var discount = await dbContexto.Discount.FirstOrDefaultAsync(
                        d => d.DiscountId == pDiscount.DiscountId);

                    if (discount == null)
                        throw new Exception($"No se encontró el descuento con ID {pDiscount.DiscountId}.");

                    // Eliminación lógica: se cambia el estado del descuento
                    // en lugar de eliminarlo físicamente de la base de datos.
                    discount.StatusId = pDiscount.StatusId;

                    dbContexto.Update(discount);
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
        /// Obtiene un descuento específico por su identificador, incluyendo
        /// su relación con <see cref="Status"/>.
        /// </summary>
        /// <param name="pDiscount">Objeto <see cref="Discount"/> con el <c>DiscountId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Discount"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Discount?> ObtenerPorIdAsync(Discount pDiscount)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Discount
                        .Include(d => d.Status)
                        .FirstOrDefaultAsync(d => d.DiscountId == pDiscount.DiscountId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de descuentos aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pDiscount">
        /// Objeto <see cref="Discount"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>Name</c>: filtra por coincidencia parcial en el nombre (null = sin filtro).</description></item>
        ///   <item><description><c>StatusId</c>: filtra por estado (0 = sin filtro, devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <param name="pFromDate">Filtra descuentos cuya <c>StartDate</c> sea mayor o igual a esta fecha (null = sin filtro).</param>
        /// <param name="pToDate">Filtra descuentos cuya <c>EndDate</c> sea menor o igual a esta fecha (null = sin filtro).</param>
        /// <returns>
        /// Lista de objetos <see cref="Discount"/> que cumplen los filtros indicados,
        /// ordenados por nombre de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Discount>> ObtenerTodosAsync(
            Discount pDiscount,
            DateTime? pFromDate = null,
            DateTime? pToDate = null)
        {
            var result = new List<Discount>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Discount
                        .Include(d => d.Status)
                        .Where(d =>
                            (pDiscount.Name == null || d.Name!.Contains(pDiscount.Name)) &&
                            (pDiscount.StatusId == 0 || d.StatusId == pDiscount.StatusId) &&
                            (pFromDate == null || d.StartDate >= pFromDate.Value) &&
                            (pToDate == null || d.EndDate <= pToDate.Value)
                        )
                        .OrderBy(d => d.Name)
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
        /// Obtiene la lista de descuentos vigentes a una fecha dada,
        /// considerando únicamente aquellos cuyo estado esté activo
        /// y cuyo rango de fechas incluya la fecha consultada.
        /// </summary>
        /// <param name="pDate">Fecha de referencia para evaluar la vigencia del descuento.</param>
        /// <param name="pActiveStatusId">
        /// Identificador del estado "Activo" en la tabla <see cref="Status"/>,
        /// usado para filtrar solo los descuentos operativos.
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Discount"/> vigentes a la fecha indicada,
        /// ordenados por porcentaje de descuento de forma descendente (mayor descuento primero).
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Discount>> ObtenerVigentesAsync(DateTime pDate, int pActiveStatusId)
        {
            var result = new List<Discount>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Discount
                        .Include(d => d.Status)
                        .Where(d =>
                            d.StatusId == pActiveStatusId &&
                            d.StartDate <= pDate &&
                            d.EndDate >= pDate
                        )
                        .OrderByDescending(d => d.Percentage)
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
    }
}