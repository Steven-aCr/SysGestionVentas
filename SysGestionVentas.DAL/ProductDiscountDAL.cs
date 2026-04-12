using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;

namespace SysGestionVentas.DAL
{
    public class ProductDiscountDAL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Verifica si ya existe una asignación activa entre el producto y el descuento indicados.
        /// </summary>
        /// <param name="pProductDiscount">Entidad con <c>ProductId</c> y <c>DiscountId</c> a verificar.</param>
        /// <param name="pDbContexto">Instancia activa del contexto de base de datos.</param>
        /// <returns>
        /// <c>true</c> si la asignación ya existe y está activa; <c>false</c> en caso contrario.
        /// </returns>
        private static async Task<bool> ExisteAsignacionAsync(ProductDiscount pProductDiscount, DbContexto pDbContexto)
        {
            return await pDbContexto.ProductDiscount
                .AnyAsync(pd => pd.ProductId == pProductDiscount.ProductId
                             && pd.DiscountId == pProductDiscount.DiscountId
                             && pd.IsActive);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Registra una nueva asignación de descuento a un producto.
        /// </summary>
        /// <param name="pProductDiscount">
        /// Entidad <see cref="ProductDiscount"/> con <c>ProductId</c>, <c>DiscountId</c>
        /// y <c>AssignedByUser</c> requeridos.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente,
        /// <c>0</c> si ocurrió un error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si la asignación ya existe o si ocurre un error en la base de datos.
        /// </exception>
        public static async Task<int> GuardarAsync(ProductDiscount pProductDiscount)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    bool existeAsignacion = await ExisteAsignacionAsync(pProductDiscount, dbContexto);
                    if (existeAsignacion)
                        throw new Exception("El descuento ya está asignado a este producto.");

                    pProductDiscount.AssignedAt = DateTime.UtcNow;
                    pProductDiscount.IsActive = true;
                    dbContexto.ProductDiscount.Add(pProductDiscount);
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
        /// Realiza la eliminación lógica de una asignación de descuento a un producto,
        /// marcando <c>IsActive = false</c> sin eliminar el registro físicamente.
        /// </summary>
        /// <param name="pProductDiscount">
        /// Entidad con <c>ProductId</c> y <c>DiscountId</c> de la asignación a desactivar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se desactivó correctamente,
        /// <c>0</c> si ocurrió un error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si la asignación no existe o si ocurre un error en la base de datos.
        /// </exception>
        public static async Task<int> EliminarAsync(ProductDiscount pProductDiscount)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var asignacion = await dbContexto.ProductDiscount
                        .FirstOrDefaultAsync(pd => pd.ProductId == pProductDiscount.ProductId
                                                && pd.DiscountId == pProductDiscount.DiscountId
                                                && pd.IsActive);

                    if (asignacion == null)
                        throw new Exception("La asignación no existe o ya fue desactivada.");

                    asignacion.IsActive = false;
                    dbContexto.ProductDiscount.Update(asignacion);
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
        /// Obtiene la lista de descuentos activos asignados a un producto específico.
        /// </summary>
        /// <param name="pProductId">Identificador del producto a consultar.</param>
        /// <returns>
        /// Lista de <see cref="ProductDiscount"/> con las propiedades de navegación
        /// <c>Product</c> y <c>Discount</c> cargadas. Retorna una lista vacía si no hay resultados.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre un error en la base de datos.
        /// </exception>
        public static async Task<List<ProductDiscount>> ObtenerPorProductoAsync(int pProductId)
        {
            var result = new List<ProductDiscount>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.ProductDiscount
                        .Include(pd => pd.Product)
                        .Include(pd => pd.Discount)
                        .Where(pd => pd.ProductId == pProductId && pd.IsActive)
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
        /// Obtiene todos los registros activos de asignaciones entre productos y descuentos.
        /// </summary>
        /// <returns>
        /// Lista completa de <see cref="ProductDiscount"/> activos con propiedades
        /// de navegación <c>Product</c> y <c>Discount</c> cargadas.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre un error en la base de datos.
        /// </exception>
        public static async Task<List<ProductDiscount>> ObtenerTodosAsync()
        {
            var result = new List<ProductDiscount>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.ProductDiscount
                        .Include(pd => pd.Product)
                        .Include(pd => pd.Discount)
                        .Where(pd => pd.IsActive)
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