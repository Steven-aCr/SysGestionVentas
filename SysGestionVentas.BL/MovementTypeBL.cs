using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="MovementType"/>.
    /// Actúa como intermediario entre la capa de presentación y la capa DAL,
    /// delegando las operaciones CRUD a <see cref="MovementTypeDAL"/>.
    /// </summary>
    public class MovementTypeBL
    {
        #region "CRUD"

        /// <summary>
        /// Guarda un nuevo tipo de movimiento en la base de datos.
        /// </summary>
        /// <param name="pMovementType">
        /// Objeto <see cref="MovementType"/> con los datos.
        /// </param>
        /// <returns>
        /// Número de filas afectadas.
        /// </returns>
        public async Task<int> GuardarAsync(MovementType pMovementType)
        {
            return await MovementTypeDAL.GuardarAsync(pMovementType);
        }

        /// <summary>
        /// Modifica un tipo de movimiento existente.
        /// </summary>
        /// <param name="pMovementType">
        /// Objeto con los datos actualizados.
        /// </param>
        /// <returns>
        /// Número de filas afectadas.
        /// </returns>
        public async Task<int> ModificarAsync(MovementType pMovementType)
        {
            return await MovementTypeDAL.ModificarAsync(pMovementType);
        }

        /// <summary>
        /// Elimina un tipo de movimiento.
        /// </summary>
        /// <param name="pMovementType">
        /// Objeto con el Id del registro.
        /// </param>
        /// <returns>
        /// Número de filas afectadas.
        /// </returns>
        public async Task<int> EliminarAsync(MovementType pMovementType)
        {
            return await MovementTypeDAL.EliminarAsync(pMovementType);
        }

        /// <summary>
        /// Obtiene todos los tipos de movimiento.
        /// </summary>
        /// <param name="pMovementType">
        /// Parámetro opcional.
        /// </param>
        /// <returns>
        /// Lista de <see cref="MovementType"/>.
        /// </returns>
        public async Task<List<MovementType>> ObtenerTodosAsync(MovementType pMovementType)
        {
            return await MovementTypeDAL.ObtenerTodosAsync(pMovementType);
        }

        /// <summary>
        /// Obtiene un tipo de movimiento por Id.
        /// </summary>
        /// <param name="pMovementType">
        /// Objeto con el Id.
        /// </param>
        /// <returns>
        /// Objeto <see cref="MovementType"/>.
        /// </returns>
        public async Task<MovementType> ObtenerPorIdAsync(MovementType pMovementType)
        {
            return await MovementTypeDAL.ObtenerPorIdAsync(pMovementType);
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