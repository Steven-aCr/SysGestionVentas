using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="Category"/>.
    /// Actúa como intermediario entre la capa de presentación y la capa DAL,
    /// delegando las operaciones CRUD a <see cref="CategoryDAL"/>.
    /// Las categorías permiten clasificar los productos del sistema y están
    /// asociadas a un estado y al usuario que las creó.
    /// </summary>
    public class CategoryBL
    {
        #region "CRUD"

        /// <summary>
        /// Guarda una nueva categoría en la base de datos de forma asíncrona,
        /// invocando la lógica de validación definida en <see cref="CategoryDAL.GuardarAsync"/>.
        /// </summary>
        /// <param name="pCategory">
        /// Objeto <see cref="Category"/> con los datos de la nueva categoría.
        /// Los campos requeridos son: <c>Name</c>, <c>StatusId</c>
        /// y <c>CreatedByUser</c>. El campo <c>Description</c> es opcional.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la categoría fue guardada correctamente,
        /// <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>Name</c> de la categoría ya existe, o si ocurre
        /// cualquier error durante la operación.
        /// </exception>
        public async Task<int> GuardarAsync(Category pCategory)
        {
            return await CategoryDAL.GuardarAsync(pCategory);
        }

        /// <summary>
        /// Modifica los datos de una categoría existente de forma asíncrona,
        /// invocando la lógica de actualización definida en <see cref="CategoryDAL.ModificarAsync"/>.
        /// </summary>
        /// <param name="pCategory">
        /// Objeto <see cref="Category"/> con los datos actualizados.
        /// El campo <c>CategoryId</c> es requerido para identificar el registro a modificar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la modificación fue exitosa, <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>Name</c> ya está en uso por otra categoría,
        /// o si ocurre cualquier error durante la operación.
        /// </exception>
        public async Task<int> ModificarAsync(Category pCategory)
        {
            return await CategoryDAL.ModificarAsync(pCategory);
        }

        /// <summary>
        /// Elimina una categoría de la base de datos de forma asíncrona,
        /// invocando la lógica de eliminación definida en <see cref="CategoryDAL.EliminarAsync"/>.
        /// </summary>
        /// <param name="pCategory">
        /// Objeto <see cref="Category"/> que debe contener el <c>CategoryId</c>
        /// de la categoría a eliminar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la eliminación fue exitosa, <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la operación de eliminación.
        /// Tener en cuenta que si la categoría tiene productos asociados en
        /// <c>ProductList</c>, la base de datos rechazará la eliminación
        /// por integridad referencial.
        /// </exception>
        public async Task<int> EliminarAsync(Category pCategory)
        {
            return await CategoryDAL.EliminarAsync(pCategory);
        }

        /// <summary>
        /// Obtiene la lista completa de categorías registradas en la base de datos
        /// de forma asíncrona, incluyendo los datos relacionados de <see cref="SysStatus"/>.
        /// </summary>
        /// <param name="pCategory">
        /// Objeto <see cref="Category"/> utilizado como parámetro de entrada.
        /// En esta versión no se aplican filtros; se retornan todos los registros.
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Category"/> con sus relaciones cargadas.
        /// Retorna una lista vacía si no hay registros o si ocurre un error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la consulta.
        /// </exception>
        public async Task<List<Category>> ObtenerTodosAsync(Category pCategory)
        {
            return await CategoryDAL.ObtenerTodosAsync(pCategory);
        }

        /// <summary>
        /// Obtiene una categoría específica de la base de datos de forma asíncrona,
        /// buscándola por su <c>CategoryId</c>, incluyendo los datos relacionados
        /// de <see cref="SysStatus"/>.
        /// </summary>
        /// <param name="pCategory">
        /// Objeto <see cref="Category"/> que debe contener el <c>CategoryId</c>
        /// de la categoría a buscar.
        /// </param>
        /// <returns>
        /// Objeto <see cref="Category"/> con sus relaciones cargadas si fue encontrado;
        /// un objeto vacío si no existe el registro.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la consulta.
        /// </exception>
        public async Task<Category> ObtenerPorIdAsync(Category pCategory)
        {
            return await CategoryDAL.ObtenerPorIdAsync(pCategory);
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