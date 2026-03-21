using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysGestionVentas.DAL
{
    public class SupplierDAL
    {
        /// <summary>
        /// Verifica si ya existe un proveedor con el mismo NIT.
        /// </summary>
        private static async Task<bool> ExisteNit(Supplier pSupplier, DbContexto pDBContexto)
        {
            var supplierExiste = await pDBContexto.Supplier.FirstOrDefaultAsync(
                s => s.Nit == pSupplier.Nit && s.SupplierId != pSupplier.SupplierId);

            return (supplierExiste != null && supplierExiste.SupplierId > 0);
        }

        /// <summary>
        /// Verifica si ya existe un proveedor con el mismo NRC.
        /// </summary>
        private static async Task<bool> ExisteNrc(Supplier pSupplier, DbContexto pDBContexto)
        {
            var supplierExiste = await pDBContexto.Supplier.FirstOrDefaultAsync(
                s => s.Nrc == pSupplier.Nrc && s.SupplierId != pSupplier.SupplierId);

            return (supplierExiste != null && supplierExiste.SupplierId > 0);
        }

        /// <summary>
        /// Registra un nuevo proveedor en la base de datos.
        /// </summary>
        /// <param name="pSupplier">Objeto <see cref="Supplier"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el NIT o NRC ya existen o si ocurre un error.
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
        /// Modifica un proveedor existente.
        /// </summary>
        /// <param name="pSupplier">Objeto <see cref="Supplier"/> con los datos actualizados.</param>
        /// <returns>Número de filas afectadas.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el proveedor no existe o si hay duplicados.
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
        /// Realiza una eliminación lógica del proveedor.
        /// </summary>
        /// <param name="pSupplier">
        /// Objeto <see cref="Supplier"/> con el <c>SupplierId</c> y el nuevo <c>StatusId</c>.
        /// </param>
        /// <returns>Número de filas afectadas.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el proveedor no existe.
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

                    // Eliminación lógica
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
        /// Obtiene un proveedor por su ID incluyendo sus relaciones.
        /// </summary>
        /// <param name="pSupplier">Objeto <see cref="Supplier"/> con el ID a buscar.</param>
        /// <returns>Proveedor encontrado o <c>null</c>.</returns>
        public static async Task<Supplier> ObtenerPorIdAsync(Supplier pSupplier)
        {
            var result = new Supplier();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Supplier
                        .Include(s => s.Person)
                        .Include(s => s.Status)
                        .FirstOrDefaultAsync(s => s.SupplierId == pSupplier.SupplierId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene una lista de proveedores aplicando filtros opcionales.
        /// </summary>
        /// <param name="pSupplier">
        /// Filtros:
        /// <list type="bullet">
        /// <item><description><c>CompanyName</c>: búsqueda por nombre.</description></item>
        /// <item><description><c>Nit</c>: búsqueda por NIT.</description></item>
        /// <item><description><c>StatusId</c>: filtro por estado.</description></item>
        /// </list>
        /// </param>
        /// <returns>Lista de proveedores.</returns>
        public static async Task<List<Supplier>> ObtenerTodosAsync(Supplier pSupplier)
        {
            var result = new List<Supplier>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Supplier
                        .Include(s => s.Person)
                        .Include(s => s.Status)
                        .Where(s =>
                            (pSupplier.CompanyName == null || s.CompanyName.Contains(pSupplier.CompanyName)) &&
                            (pSupplier.Nit == null || s.Nit.Contains(pSupplier.Nit)) &&
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