using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="Client"/>.
    /// Actúa como intermediario entre la capa de presentación y la capa DAL,
    /// delegando las operaciones CRUD a <see cref="ClientDAL"/>.
    /// Un cliente está vinculado obligatoriamente a una <see cref="Person"/>
    /// ya registrada en el sistema.
    /// </summary>
    public class ClientBL
    {
        #region "CRUD"

        /// <summary>
        /// Guarda un nuevo cliente en la base de datos de forma asíncrona,
        /// invocando la lógica de validación definida en <see cref="ClientDAL.GuardarAsync"/>.
        /// </summary>
        /// <param name="pClient">
        /// Objeto <see cref="Client"/> con los datos del nuevo cliente.
        /// El campo requerido es <c>PersonId</c>, que debe corresponder
        /// a una persona existente en la tabla Person.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si el cliente fue guardado correctamente,
        /// <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>PersonId</c> no existe o si ocurre
        /// cualquier error durante la operación.
        /// </exception>
        public async Task<int> GuardarAsync(Client pClient)
        {
            return await ClientDAL.GuardarAsync(pClient);
        }

        /// <summary>
        /// Modifica los datos de un cliente existente de forma asíncrona,
        /// invocando la lógica de actualización definida en <see cref="ClientDAL.ModificarAsync"/>.
        /// </summary>
        /// <param name="pClient">
        /// Objeto <see cref="Client"/> con los datos actualizados.
        /// El campo <c>ClientId</c> es requerido para identificar el registro a modificar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la modificación fue exitosa, <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la operación.
        /// </exception>
        public async Task<int> ModificarAsync(Client pClient)
        {
            return await ClientDAL.ModificarAsync(pClient);
        }

        /// <summary>
        /// Elimina un cliente de la base de datos de forma asíncrona,
        /// invocando la lógica de eliminación definida en <see cref="ClientDAL.EliminarAsync"/>.
        /// </summary>
        /// <param name="pClient">
        /// Objeto <see cref="Client"/> que debe contener el <c>ClientId</c>
        /// del cliente a eliminar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la eliminación fue exitosa, <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la operación de eliminación.
        /// </exception>
        public async Task<int> EliminarAsync(Client pClient)
        {
            return await ClientDAL.EliminarAsync(pClient);
        }

        /// <summary>
        /// Obtiene la lista completa de clientes registrados en la base de datos
        /// de forma asíncrona, incluyendo los datos relacionados de <see cref="Person"/>.
        /// </summary>
        /// <param name="pClient">
        /// Objeto <see cref="Client"/> utilizado como parámetro de entrada.
        /// En esta versión no se aplican filtros; se retornan todos los registros.
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Client"/> con sus relaciones cargadas.
        /// Retorna una lista vacía si no hay registros o si ocurre un error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la consulta.
        /// </exception>
        public async Task<List<Client>> ObtenerTodosAsync(Client pClient)
        {
            return await ClientDAL.ObtenerTodosAsync(pClient);
        }

        /// <summary>
        /// Obtiene un cliente específico de la base de datos de forma asíncrona,
        /// buscándolo por su <c>ClientId</c>, incluyendo los datos relacionados
        /// de <see cref="Person"/>.
        /// </summary>
        /// <param name="pClient">
        /// Objeto <see cref="Client"/> que debe contener el <c>ClientId</c>
        /// del cliente a buscar.
        /// </param>
        /// <returns>
        /// Objeto <see cref="Client"/> con sus relaciones cargadas si fue encontrado;
        /// un objeto vacío si no existe el registro.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la consulta.
        /// </exception>
        public async Task<Client> ObtenerPorIdAsync(Client pClient)
        {
            return await ClientDAL.ObtenerPorIdAsync(pClient);
        }

        public async Task<string?> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<string?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Client obj)
        {
            throw new NotImplementedException();
        }

        public async Task CreateAsync(Client obj)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> ObtenerTodosAsync()
        {
            throw new NotImplementedException();
        }

        public async Task EliminarAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<string?> ObtenerPorIdAsync(int value)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}