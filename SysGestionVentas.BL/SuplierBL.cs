using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class SupplierBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="Supplier"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pSupplier">Objeto <see cref="Supplier"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(Supplier pSupplier)
        {
            var contexto = new ValidationContext(pSupplier);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pSupplier, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo proveedor en el sistema.
        /// Verifica unicidad de NIT y NRC en la capa DAL.
        /// </summary>
        /// <param name="pSupplier">Objeto <see cref="Supplier"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el NIT o NRC ya existen, o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> GuardarAsync(Supplier pSupplier)
        {
            ValidarEntidad(pSupplier);
            return await SupplierDAL.GuardarAsync(pSupplier);
        }

        /// <summary>
        /// Valida y modifica los datos de un proveedor existente en el sistema.
        /// Verifica unicidad de NIT y NRC en la capa DAL.
        /// </summary>
        /// <param name="pSupplier">
        /// Objeto <see cref="Supplier"/> con el <c>SupplierId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">
        /// Se lanza si el proveedor no existe, si hay duplicados de NIT o NRC,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> ModificarAsync(Supplier pSupplier)
        {
            if (pSupplier.SupplierId <= 0)
                throw new Exception("El ID de proveedor no es válido.");

            ValidarEntidad(pSupplier);
            return await SupplierDAL.ModificarAsync(pSupplier);
        }

        /// <summary>
        /// Realiza la eliminación lógica de un proveedor cambiando su estado en el sistema.
        /// No elimina el registro físicamente de la base de datos.
        /// </summary>
        /// <param name="pSupplier">
        /// Objeto <see cref="Supplier"/> con el <c>SupplierId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el ID no es válido, si el proveedor no existe,
        /// o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<int> EliminarAsync(Supplier pSupplier)
        {
            if (pSupplier.SupplierId <= 0)
                throw new Exception("El ID de proveedor no es válido.");

            if (pSupplier.StatusId <= 0)
                throw new Exception("Debe especificar un estado válido para la eliminación lógica.");

            return await SupplierDAL.EliminarAsync(pSupplier);
        }

        /// <summary>
        /// Obtiene un proveedor específico por su identificador, incluyendo sus relaciones
        /// con <see cref="Person"/>, el <see cref="Status"/> de la persona
        /// y el <see cref="Status"/> propio del proveedor.
        /// </summary>
        /// <param name="pSupplier">Objeto <see cref="Supplier"/> con el <c>SupplierId</c> a buscar.</param>
        /// <returns>El objeto <see cref="Supplier"/> encontrado, o <c>null</c> si no existe.</returns>
        /// <exception cref="Exception">Se lanza si el ID no es válido o si ocurre un error en base de datos.</exception>
        public static async Task<Supplier?> ObtenerPorIdAsync(Supplier pSupplier)
        {
            if (pSupplier.SupplierId <= 0)
                throw new Exception("El ID de proveedor no es válido.");

            return await SupplierDAL.ObtenerPorIdAsync(pSupplier);
        }

        /// <summary>
        /// Obtiene una lista de proveedores aplicando filtros opcionales.
        /// </summary>
        /// <param name="pSupplier">Objeto <see cref="Supplier"/> usado como filtro de búsqueda.</param>
        /// <returns>Lista de objetos <see cref="Supplier"/> ordenados por nombre de empresa.</returns>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<List<Supplier>> ObtenerTodosAsync(Supplier pSupplier)
        {
            return await SupplierDAL.ObtenerTodosAsync(pSupplier);
        }

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de proveedores con soporte para paginación.
        /// Valida que los parámetros de paginación sean coherentes antes de ejecutar la consulta.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Supplier}"/> con los filtros y parámetros de paginación.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Supplier}"/> con la lista de proveedores encontrados
        /// e información de paginación.
        /// </returns>
        /// <exception cref="ArgumentNullException">Se lanza si <paramref name="pPagedQuery"/> es <c>null</c>.</exception>
        /// <exception cref="Exception">
        /// Se lanza si los parámetros de paginación no son válidos o si ocurre un error en base de datos.
        /// </exception>
        public static async Task<PagedResult<Supplier>> BuscarAsync(PagedQuery<Supplier> pPagedQuery)
        {
            if (pPagedQuery == null)
                throw new ArgumentNullException(nameof(pPagedQuery), "Los parámetros de búsqueda no pueden ser nulos.");

            if (pPagedQuery.Page <= 0)
                throw new Exception("El número de página debe ser mayor a 0.");

            if (pPagedQuery.PageSize <= 0)
                throw new Exception("El tamaño de página debe ser mayor a 0.");

            return await SupplierDAL.BuscarAsync(pPagedQuery);
        }

        #endregion
    }
}