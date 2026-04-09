using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="Department"/>.
    /// Actúa como intermediario entre la capa de presentación y la capa DAL,
    /// delegando las operaciones CRUD a <see cref="DepartmentDAL"/>.
    /// </summary>
    public class DepartmentBL
    {
        #region "CRUD"

        /// <summary>
        /// Guarda un nuevo departamento en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="pDepartment">
        /// Objeto <see cref="Department"/> con los datos del nuevo departamento.
        /// </param>
        /// <returns>
        /// Número de filas afectadas (1 si se guardó correctamente, 0 en caso contrario).
        /// </returns>
        public async Task<int> GuardarAsync(Department pDepartment)
        {
            return await DepartmentDAL.GuardarAsync(pDepartment);
        }

        /// <summary>
        /// Modifica un departamento existente de forma asíncrona.
        /// </summary>
        /// <param name="pDepartment">
        /// Objeto <see cref="Department"/> con los datos actualizados.
        /// </param>
        /// <returns>
        /// Número de filas afectadas.
        /// </returns>
        public async Task<int> ModificarAsync(Department pDepartment)
        {
            return await DepartmentDAL.ModificarAsync(pDepartment);
        }

        /// <summary>
        /// Elimina un departamento de la base de datos.
        /// </summary>
        /// <param name="pDepartment">
        /// Objeto <see cref="Department"/> con el Id del departamento.
        /// </param>
        /// <returns>
        /// Número de filas afectadas.
        /// </returns>
        public async Task<int> EliminarAsync(Department pDepartment)
        {
            return await DepartmentDAL.EliminarAsync(pDepartment);
        }

        /// <summary>
        /// Obtiene todos los departamentos registrados.
        /// </summary>
        /// <param name="pDepartment">
        /// Parámetro opcional (sin filtros en esta versión).
        /// </param>
        /// <returns>
        /// Lista de <see cref="Department"/>.
        /// </returns>
        public async Task<List<Department>> ObtenerTodosAsync(Department pDepartment)
        {
            return await DepartmentDAL.ObtenerTodosAsync(pDepartment);
        }

        /// <summary>
        /// Obtiene un departamento por su Id.
        /// </summary>
        /// <param name="pDepartment">
        /// Objeto con el Id del departamento.
        /// </param>
        /// <returns>
        /// Objeto <see cref="Department"/>.
        /// </returns>
        public async Task<Department> ObtenerPorIdAsync(Department pDepartment)
        {
            return await DepartmentDAL.ObtenerPorIdAsync(pDepartment);
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