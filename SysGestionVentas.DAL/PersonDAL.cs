using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;

namespace SysGestionVentas.DAL
{
    public class PersonDAL
    {
        /// <summary>
        /// Verifica si ya existe una persona con el mismo DUI en la base de datos,
        /// excluyendo el propio registro en caso de modificación.
        /// Solo valida si el DUI proporcionado no es nulo o vacío.
        /// </summary>
        /// <param name="pPerson">Objeto <see cref="Person"/> con el <c>Dui</c> a validar.</param>
        /// <param name="pDBContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el DUI ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExisteDui(Person pPerson, DbContexto pDBContexto)
        {
            if (string.IsNullOrWhiteSpace(pPerson.Dui))
                return false;

            return await pDBContexto.Person.AnyAsync(
                p => p.Dui == pPerson.Dui && p.PersonId != pPerson.PersonId);
        }

        /// <summary>
        /// Verifica si ya existe una persona con el mismo número de teléfono en la base de datos,
        /// excluyendo el propio registro en caso de modificación.
        /// Solo valida si el número de teléfono proporcionado no es nulo o vacío.
        /// </summary>
        /// <param name="pPerson">Objeto <see cref="Person"/> con el <c>PhoneNumber</c> a validar.</param>
        /// <param name="pDBContexto">Contexto de base de datos activo.</param>
        /// <returns><c>true</c> si el teléfono ya existe, <c>false</c> en caso contrario.</returns>
        private static async Task<bool> ExistePhone(Person pPerson, DbContexto pDBContexto)
        {
            if (string.IsNullOrWhiteSpace(pPerson.PhoneNumber))
                return false;

            return await pDBContexto.Person.AnyAsync(
                p => p.PhoneNumber == pPerson.PhoneNumber && p.PersonId != pPerson.PersonId);
        }

        /// <summary>
        /// Registra una nueva persona en la base de datos.
        /// Valida unicidad de <c>Dui</c> y <c>PhoneNumber</c> antes de guardar.
        /// </summary>
        /// <param name="pPerson">Objeto <see cref="Person"/> con los datos a guardar.</param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se guardó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si el DUI o teléfono ya existen, o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> GuardarAsync(Person pPerson)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteDui(pPerson, dbContexto))
                        throw new Exception("El DUI ya está registrado.");

                    if (await ExistePhone(pPerson, dbContexto))
                        throw new Exception("El número de teléfono ya está registrado.");

                    pPerson.CreatedAt = DateTime.UtcNow;
                    dbContexto.Add(pPerson);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Modifica los datos de una persona existente en la base de datos.
        /// Valida unicidad de <c>Dui</c> y <c>PhoneNumber</c> antes de actualizar.
        /// </summary>
        /// <param name="pPerson">
        /// Objeto <see cref="Person"/> con el <c>PersonId</c> del registro a modificar
        /// y los nuevos valores a actualizar.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se modificó correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si la persona no existe, si hay duplicados de DUI o teléfono,
        /// o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> ModificarAsync(Person pPerson)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    if (await ExisteDui(pPerson, dbContexto))
                        throw new Exception("El DUI ya está registrado.");

                    if (await ExistePhone(pPerson, dbContexto))
                        throw new Exception("El número de teléfono ya está registrado.");

                    var person = await dbContexto.Person.FirstOrDefaultAsync(
                        p => p.PersonId == pPerson.PersonId);

                    if (person == null)
                        throw new Exception($"No se encontró la persona con ID {pPerson.PersonId}.");

                    person.FirstName   = pPerson.FirstName;
                    person.LastName    = pPerson.LastName;
                    person.Adress      = pPerson.Adress;
                    person.PhoneNumber = pPerson.PhoneNumber;
                    person.Dui         = pPerson.Dui;
                    person.StatusId    = pPerson.StatusId;

                    dbContexto.Update(person);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Realiza una eliminación lógica de una persona, cambiando su estado en la base de datos.
        /// No elimina el registro físicamente.
        /// </summary>
        /// <param name="pPerson">
        /// Objeto <see cref="Person"/> con el <c>PersonId</c> del registro
        /// y el <c>StatusId</c> correspondiente al estado inactivo.
        /// </param>
        /// <returns>
        /// Número de filas afectadas. Retorna <c>1</c> si se cambió el estado correctamente, <c>0</c> si falló.
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si la persona no existe o si ocurre un error durante la operación.
        /// </exception>
        public static async Task<int> EliminarAsync(Person pPerson)
        {
            int result = 0;
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var person = await dbContexto.Person.FirstOrDefaultAsync(
                        p => p.PersonId == pPerson.PersonId);

                    if (person == null)
                        throw new Exception($"No se encontró la persona con ID {pPerson.PersonId}.");

                    // Eliminación lógica: se cambia el estado de la persona
                    // en lugar de eliminarla físicamente de la base de datos.
                    person.StatusId = pPerson.StatusId;

                    dbContexto.Update(person);
                    result = await dbContexto.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                result = 0;
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene una persona específica por su identificador, incluyendo
        /// su relación con <see cref="Status"/>.
        /// </summary>
        /// <param name="pPerson">Objeto <see cref="Person"/> con el <c>PersonId</c> a buscar.</param>
        /// <returns>
        /// El objeto <see cref="Person"/> encontrado, o <c>null</c> si no existe.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<Person?> ObtenerPorIdAsync(Person pPerson)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    return await dbContexto.Person
                        .Include(p => p.Status)
                        .FirstOrDefaultAsync(p => p.PersonId == pPerson.PersonId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una lista de personas aplicando filtros opcionales.
        /// Los parámetros con valor <c>null</c> o <c>0</c> son ignorados en el filtro.
        /// </summary>
        /// <param name="pPerson">
        /// Objeto <see cref="Person"/> usado como filtro de búsqueda:
        /// <list type="bullet">
        ///   <item><description><c>FirstName</c>: filtra por coincidencia parcial en el nombre (null = sin filtro).</description></item>
        ///   <item><description><c>LastName</c>: filtra por coincidencia parcial en el apellido (null = sin filtro).</description></item>
        ///   <item><description><c>Dui</c>: filtra por coincidencia parcial en el DUI (null = sin filtro).</description></item>
        ///   <item><description><c>StatusId</c>: filtra por estado (0 = sin filtro, devuelve todos).</description></item>
        /// </list>
        /// </param>
        /// <returns>
        /// Lista de objetos <see cref="Person"/> que cumplen los filtros indicados,
        /// ordenados por apellido y luego por nombre de forma ascendente.
        /// </returns>
        /// <exception cref="Exception">Se lanza si ocurre un error durante la consulta.</exception>
        public static async Task<List<Person>> ObtenerTodosAsync(Person pPerson)
        {
            var result = new List<Person>();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Person
                        .Include(p => p.Status)
                        .Where(p =>
                            (pPerson.FirstName == null || p.FirstName!.Contains(pPerson.FirstName)) &&
                            (pPerson.LastName  == null || p.LastName!.Contains(pPerson.LastName))   &&
                            (pPerson.Dui       == null || p.Dui!.Contains(pPerson.Dui))             &&
                            (pPerson.StatusId  == 0    || p.StatusId == pPerson.StatusId)
                        )
                        .OrderBy(p => p.LastName)
                            .ThenBy(p => p.FirstName)
                        .ToListAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }
    }
}