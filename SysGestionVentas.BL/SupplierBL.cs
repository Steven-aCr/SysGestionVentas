using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="Supplier"/>.
    /// Actúa como intermediario entre la capa de presentación y la capa DAL,
    /// delegando las operaciones CRUD a <see cref="SupplierDAL"/>.
    /// Un proveedor se identifica fiscalmente mediante los campos <c>Nit</c>
    /// y <c>Nrc</c>, los cuales deben ser únicos en el sistema.
    /// </summary>
    public class SupplierBL
    {
        #region "CRUD"

        /// <summary>
        /// Guarda un nuevo proveedor en la base de datos de forma asíncrona,
        /// invocando la lógica de validación definida en <see cref="SupplierDAL.GuardarAsync"/>.
        /// </summary>
        /// <param name="pSupplier">
        /// Objeto <see cref="Supplier"/> con los datos del nuevo proveedor.
        /// Los campos requeridos son: <c>Nit</c>, <c>Nrc</c>, <c>PersonId</c>
        /// y <c>StatusId</c>. Los campos <c>CompanyName</c> y <c>Description</c>
        /// son opcionales.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si el proveedor fue guardado correctamente,
        /// <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>Nit</c> o el <c>Nrc</c> ya están registrados,
        /// o si ocurre cualquier error durante la operación.
        /// </exception>
        public async Task<int> GuardarAsync(Supplier pSupplier)
        {
            return await SupplierDAL.GuardarAsync(pSupplier);
        }

        /// <summary>
        /// Modifica los datos de un proveedor existente de forma asíncrona,
        /// invocando la lógica de actualización definida en <see cref="SupplierDAL.ModificarAsync"/>.
        /// </summary>
        /// <param name="pSupplier">
        /// Objeto <see cref="Supplier"/> con los datos actualizados del proveedor.
        /// El campo <c>SupplierId</c> es requerido para identificar el registro a modificar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la modificación fue exitosa, <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>Nit</c> o el <c>Nrc</c> ya están en uso por otro proveedor,
        /// o si ocurre cualquier error durante la operación.
        /// </exception>
        public async Task<int> ModificarAsync(Supplier pSupplier)
        {
            return await SupplierDAL.ModificarAsync(pSupplier);
        }

        /// <summary>
        /// Elimina un proveedor de la base de datos de forma asíncrona,
        /// invocando la lógica de eliminación definida en <see cref="SupplierDAL.EliminarAsync"/>.
        /// </summary>
        /// <param name="pSupplier">
        /// Objeto <see cref="Supplier"/> que debe contener el <c>SupplierId</c>
        /// del proveedor a eliminar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la eliminación fue exitosa, <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la operación de eliminación.
        /// </exception>
        public async Task<int> EliminarAsync(Supplier pSupplier)
        {
            return await SupplierDAL.EliminarAsync(pSupplier);
        }

        /// <summary>
        /// Obtiene la lista completa de proveedores registrados en la base de datos
        /// de forma asíncrona, incluyendo los datos relacionados de
        /// <see cref="Person"/> y <see cref="SysStatus"/>.
        /// </summary>
        /// <param name="pSupplier">
        /// Objeto <see cref="Supplier"/> utilizado como parámetro de entrada.
        /// En esta versión no se aplican filtros; se retornan todos los registros.
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Supplier"/> con sus relaciones cargadas.
        /// Retorna una lista vacía si no hay registros o si ocurre un error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la consulta.
        /// </exception>
        public async Task<List<Supplier>> ObtenerTodosAsync(Supplier pSupplier)
        {
            return await SupplierDAL.ObtenerTodosAsync(pSupplier);
        }

        /// <summary>
        /// Obtiene un proveedor específico de la base de datos de forma asíncrona,
        /// buscándolo por su <c>SupplierId</c>, incluyendo los datos relacionados
        /// de <see cref="Person"/> y <see cref="SysStatus"/>.
        /// </summary>
        /// <param name="pSupplier">
        /// Objeto <see cref="Supplier"/> que debe contener el <c>SupplierId</c>
        /// del proveedor a buscar.
        /// </param>
        /// <returns>
        /// Objeto <see cref="Supplier"/> con sus relaciones cargadas si fue encontrado;
        /// un objeto vacío si no existe el registro.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la consulta.
        /// </exception>
        public async Task<Supplier> ObtenerPorIdAsync(Supplier pSupplier)
        {
            return await SupplierDAL.ObtenerPorIdAsync(pSupplier);
        }

        public async Task EliminarAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> ObtenerPorIdAsync(int value)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> ObtenerTodosAsync()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}