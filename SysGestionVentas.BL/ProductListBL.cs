using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="ProductList"/>.
    /// Actúa como intermediario entre la capa de presentación y la capa DAL,
    /// delegando las operaciones CRUD a <see cref="ProductListDAL"/>.
    /// Cada producto pertenece a una <see cref="Category"/> y se identifica
    /// de forma única mediante su <c>Barcode</c>.
    /// </summary>
    public class ProductListBL
    {
        #region "CRUD"

        /// <summary>
        /// Guarda un nuevo producto en la base de datos de forma asíncrona,
        /// invocando la lógica de validación definida en <see cref="ProductListDAL.GuardarAsync"/>.
        /// </summary>
        /// <param name="pProduct">
        /// Objeto <see cref="ProductList"/> con los datos del nuevo producto.
        /// Los campos requeridos son: <c>Name</c>, <c>Barcode</c>, <c>CategoryId</c>,
        /// <c>StatusId</c> y <c>CreatedByUser</c>. El campo <c>Description</c>
        /// es opcional.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si el producto fue guardado correctamente,
        /// <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>Barcode</c> ya está registrado, o si ocurre
        /// cualquier error durante la operación.
        /// </exception>
        public async Task<int> GuardarAsync(ProductList pProduct)
        {
            return await ProductListDAL.GuardarAsync(pProduct);
        }

        /// <summary>
        /// Modifica los datos de un producto existente de forma asíncrona,
        /// invocando la lógica de actualización definida en <see cref="ProductListDAL.ModificarAsync"/>.
        /// </summary>
        /// <param name="pProduct">
        /// Objeto <see cref="ProductList"/> con los datos actualizados del producto.
        /// El campo <c>ProductId</c> es requerido para identificar el registro a modificar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la modificación fue exitosa, <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>Barcode</c> ya está en uso por otro producto, o si
        /// ocurre cualquier error durante la operación.
        /// </exception>
        public async Task<int> ModificarAsync(ProductList pProduct)
        {
            return await ProductListDAL.ModificarAsync(pProduct);
        }

        /// <summary>
        /// Elimina un producto de la base de datos de forma asíncrona,
        /// invocando la lógica de eliminación definida en <see cref="ProductListDAL.EliminarAsync"/>.
        /// </summary>
        /// <param name="pProduct">
        /// Objeto <see cref="ProductList"/> que debe contener el <c>ProductId</c>
        /// del producto a eliminar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la eliminación fue exitosa, <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la operación de eliminación.
        /// Tener en cuenta que si el producto tiene registros relacionados en
        /// <c>Inventory</c>, <c>DocumentDetail</c> o <c>ProductDiscount</c>,
        /// la base de datos rechazará la eliminación por integridad referencial.
        /// </exception>
        public async Task<int> EliminarAsync(ProductList pProduct)
        {
            return await ProductListDAL.EliminarAsync(pProduct);
        }

        /// <summary>
        /// Obtiene la lista completa de productos registrados en la base de datos
        /// de forma asíncrona, incluyendo los datos relacionados de
        /// <see cref="Category"/> y <see cref="SysStatus"/>.
        /// </summary>
        /// <param name="pProduct">
        /// Objeto <see cref="ProductList"/> utilizado como parámetro de entrada.
        /// En esta versión no se aplican filtros; se retornan todos los registros.
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="ProductList"/> con sus relaciones cargadas.
        /// Retorna una lista vacía si no hay registros o si ocurre un error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la consulta.
        /// </exception>
        public async Task<List<ProductList>> ObtenerTodosAsync(ProductList pProduct)
        {
            return await ProductListDAL.ObtenerTodosAsync(pProduct);
        }

        /// <summary>
        /// Obtiene un producto específico de la base de datos de forma asíncrona,
        /// buscándolo por su <c>ProductId</c>, incluyendo los datos relacionados
        /// de <see cref="Category"/> y <see cref="SysStatus"/>.
        /// </summary>
        /// <param name="pProduct">
        /// Objeto <see cref="ProductList"/> que debe contener el <c>ProductId</c>
        /// del producto a buscar.
        /// </param>
        /// <returns>
        /// Objeto <see cref="ProductList"/> con sus relaciones cargadas si fue encontrado;
        /// un objeto vacío si no existe el registro.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la consulta.
        /// </exception>
        public async Task<ProductList> ObtenerPorIdAsync(ProductList pProduct)
        {
            return await ProductListDAL.ObtenerPorIdAsync(pProduct);
        }

        public async Task<string?> ObtenerTodosAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string?> ObtenerPorIdAsync(int value)
        {
            throw new NotImplementedException();
        }

        public async Task EliminarAsync(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}