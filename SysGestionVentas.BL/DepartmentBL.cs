using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class DepartmentBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Department"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pDepartment">Objeto <see cref="Department"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(Department pDepartment)
        {
            var contexto = new ValidationContext(pDepartment);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pDepartment, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo departamento en el sistema.
        /// Verifica unicidad del nombre en la capa DAL.
        /// </summary>
        /// <param name="pDepartment">Objeto <see cref="Department"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el nombre ya existe o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> GuardarAsync(Department pDepartment)
        {
            ValidarEntidad(pDepartment);
            return await DepartmentDAL.GuardarAsync(pDepartment);
        }

        /// <summary>
        /// Valida y modifica los datos de un departamento existente en el sistema.
        /// Verifica unicidad del nombre en la capa DAL.
        /// </summary>
        /// <param name="pDepartment">
        /// Objeto <see cref="Department"/> con el <c>DepartmentId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el departamento no existe, si el nombre está duplicado,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> ModificarAsync(Department pDepartment)
        {
            if (pDepartment.DepartmentId <= 0)
                throw new Exception("El ID de departamento no es válido.");

            ValidarEntidad(pDepartment);
            return await DepartmentDAL.ModificarAsync(pDepartment);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un departamento cambiando su estado en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pDepartment">
        /// Objeto <see cref="Department"/> con el <c>DepartmentId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el ID no es válido, si el departamento no existe,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> EliminarAsync(Department pDepartment)
        {
            if (pDepartment.DepartmentId <= 0)
                throw new Exception("El ID de departamento no es válido.");

            if (pDepartment.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await DepartmentDAL.EliminarAsync(pDepartment);
        }

        /// <summary>
        /// Obtiene un departamento específico por su identificador,
        /// incluyendo su relación con <see cref="Status"/>.
        /// </summary>
        /// <param name="pDepartment">Objeto <see cref="Department"/> con el <c>DepartmentId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Department"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Department?> ObtenerPorIdAsync(Department pDepartment)
        {
            if (pDepartment.DepartmentId <= 0)
                throw new Exception("El ID de departamento no es válido.");

            return await DepartmentDAL.ObtenerPorIdAsync(pDepartment);
        }

        /// <summary>
        /// Obtiene una lista de departamentos aplicando filtros opcionales.
        /// </summary>
        /// <param name="pDepartment">Objeto <see cref="Department"/> usado como filtro de búsqueda.</param>
        /// <returns>Lista de objetos <see cref="Department"/> ordenados por nombre de forma ascendente.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Department>> ObtenerTodosAsync(Department pDepartment)
        {
            return await DepartmentDAL.ObtenerTodosAsync(pDepartment);
        }

        #endregion
    }
}