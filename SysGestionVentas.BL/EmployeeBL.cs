using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDGestionVentas.BL
{
    /// <summary>
    /// Capa de lógica de negocio para la entidad <see cref="Employee"/>.
    /// Actúa como intermediario entre la capa de presentación y la capa DAL,
    /// delegando las operaciones CRUD a <see cref="EmployeeDAL"/>.
    /// Un empleado está vinculado obligatoriamente a una <see cref="Person"/>
    /// registrada en el sistema y posee un código único de identificación.
    /// </summary>
    public class EmployeeBL
    {
        #region "CRUD"

        /// <summary>
        /// Guarda un nuevo empleado en la base de datos de forma asíncrona,
        /// invocando la lógica de validación definida en <see cref="EmployeeDAL.GuardarAsync"/>.
        /// </summary>
        /// <param name="pEmployee">
        /// Objeto <see cref="Employee"/> con los datos del nuevo empleado.
        /// Los campos requeridos son: <c>EmployeeCode</c>, <c>HireDate</c>,
        /// <c>Salary</c>, <c>PersonId</c> y <c>StatusId</c>.
        /// El campo <c>EmployeeCode</c> debe ser único en la tabla.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si el empleado fue guardado correctamente,
        /// <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>EmployeeCode</c> ya existe, si el <c>PersonId</c>
        /// no es válido, o si ocurre cualquier error durante la operación.
        /// </exception>
        public async Task<int> GuardarAsync(Employee pEmployee)
        {
            return await EmployeeDAL.GuardarAsync(pEmployee);
        }

        /// <summary>
        /// Modifica los datos de un empleado existente de forma asíncrona,
        /// invocando la lógica de actualización definida en <see cref="EmployeeDAL.ModificarAsync"/>.
        /// </summary>
        /// <param name="pEmployee">
        /// Objeto <see cref="Employee"/> con los datos actualizados.
        /// El campo <c>EmployeeId</c> es requerido para identificar el registro a modificar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la modificación fue exitosa, <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el <c>EmployeeCode</c> ya está en uso por otro empleado,
        /// o si ocurre cualquier error durante la operación.
        /// </exception>
        public async Task<int> ModificarAsync(Employee pEmployee)
        {
            return await EmployeeDAL.ModificarAsync(pEmployee);
        }

        /// <summary>
        /// Elimina un empleado de la base de datos de forma asíncrona,
        /// invocando la lógica de eliminación definida en <see cref="EmployeeDAL.EliminarAsync"/>.
        /// </summary>
        /// <param name="pEmployee">
        /// Objeto <see cref="Employee"/> que debe contener el <c>EmployeeId</c>
        /// del empleado a eliminar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas en la base de datos.
        /// Retorna <c>1</c> si la eliminación fue exitosa, <c>0</c> si ocurrió algún error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la operación de eliminación.
        /// </exception>
        public async Task<int> EliminarAsync(Employee pEmployee)
        {
            return await EmployeeDAL.EliminarAsync(pEmployee);
        }

        /// <summary>
        /// Obtiene la lista completa de empleados registrados en la base de datos
        /// de forma asíncrona, incluyendo los datos relacionados de
        /// <see cref="Person"/> y <see cref="SysStatus"/>.
        /// </summary>
        /// <param name="pEmployee">
        /// Objeto <see cref="Employee"/> utilizado como parámetro de entrada.
        /// En esta versión no se aplican filtros; se retornan todos los registros.
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Employee"/> con sus relaciones cargadas.
        /// Retorna una lista vacía si no hay registros o si ocurre un error.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la consulta.
        /// </exception>
        public async Task<List<Employee>> ObtenerTodosAsync(Employee pEmployee)
        {
            return await EmployeeDAL.ObtenerTodosAsync(pEmployee);
        }

        /// <summary>
        /// Obtiene un empleado específico de la base de datos de forma asíncrona,
        /// buscándolo por su <c>EmployeeId</c>, incluyendo los datos relacionados
        /// de <see cref="Person"/> y <see cref="SysStatus"/>.
        /// </summary>
        /// <param name="pEmployee">
        /// Objeto <see cref="Employee"/> que debe contener el <c>EmployeeId</c>
        /// del empleado a buscar.
        /// </param>
        /// <returns>
        /// Objeto <see cref="Employee"/> con sus relaciones cargadas si fue encontrado;
        /// un objeto vacío si no existe el registro.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre cualquier error durante la consulta.
        /// </exception>
        public async Task<Employee> ObtenerPorIdAsync(Employee pEmployee)
        {
            return await EmployeeDAL.ObtenerPorIdAsync(pEmployee);
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