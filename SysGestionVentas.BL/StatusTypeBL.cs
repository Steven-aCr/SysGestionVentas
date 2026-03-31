using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.BL
{
    public class StatusTypeBL
    {
        #region "Métodos Privados"

        /// <summary>
        /// Valida las propiedades de un objeto <see cref="StatusType"/> utilizando los
        /// <see cref="ValidationAttribute"/> definidos en la entidad (DataAnnotations).
        /// </summary>
        /// <param name="pStatusType">Objeto <see cref="StatusType"/> a validar.</param>
        /// <exception cref="ValidationException">
        /// Se lanza si alguna propiedad no cumple con las anotaciones de validación.
        /// El mensaje contiene la descripción del primer error encontrado.
        /// </exception>
        private static void ValidarEntidad(StatusType pStatusType)
        {
            var contexto = new ValidationContext(pStatusType);
            var resultados = new List<ValidationResult>();

            bool esValido = Validator.TryValidateObject(pStatusType, contexto, resultados, validateAllProperties: true);

            if (!esValido)
                throw new ValidationException(resultados[0].ErrorMessage);
        }

        #endregion

        #region "CRUD"

        /// <summary>
        /// Valida y registra un nuevo tipo de estado en el sistema.
        /// Requiere un contexto de base de datos activo para participar en transacciones
        /// coordinadas desde capas superiores.
        /// </summary>
        /// <param name="pStatusType">Objeto <see cref="StatusType"/> con los datos a guardar.</param>
        /// <param name="dbContexto">Contexto de base de datos activo.</param>
        /// <returns>Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente.</returns>
        /// <exception cref="ValidationException">Se lanza si los datos no pasan la validación de la entidad.</exception>
        /// <exception cref="Exception">Se lanza si ocurre un error en base de datos.</exception>
        public static async Task<int> GuardarAsync(StatusType pStatusType, DbContexto dbContexto)
        {
            ValidarEntidad(pStatusType);
            return await StatusTypeDAL.GuardarAsync(pStatusType);
        }

        #endregion
    }
}