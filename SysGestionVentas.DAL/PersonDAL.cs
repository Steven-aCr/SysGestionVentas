using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using Microsoft.EntityFrameworkCore;

namespace SysGestionVentas.DAL
{
    public class PersonDAL
    {
        #region "Métodos Privados"

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
        /// Aplica los filtros de búsqueda contenidos en <see cref="PagedQuery{Person}"/>
        /// a una consulta <see cref="IQueryable{Person}"/> base.
        /// No aplica paginación; esta responsabilidad recae en <see cref="BuscarAsync"/>,
        /// lo que permite reutilizar este método para conteo sin Skip/Take.
        /// </summary>
        /// <param name="pQuery">Consulta base sin filtros aplicados.</param>
        /// <param name="pPagedQuery">Parámetros de filtro, rango de fechas y paginación.</param>
        /// <returns>
        /// <see cref="IQueryable{Person}"/> con todos los filtros y el ordenamiento aplicados,
        /// ordenado por apellido y luego por nombre de forma ascendente.
        /// </returns>
        private static IQueryable<Person> QuerySelect(
            IQueryable<Person> pQuery,
            PagedQuery<Person> pPagedQuery)
        {
            var f = pPagedQuery.Filter;

            if (f.PersonId > 0)
                pQuery = pQuery.Where(p => p.PersonId == f.PersonId);

            if (!string.IsNullOrWhiteSpace(f.FirstName))
                pQuery = pQuery.Where(p => p.FirstName!.Contains(f.FirstName));

            if (!string.IsNullOrWhiteSpace(f.LastName))
                pQuery = pQuery.Where(p => p.LastName!.Contains(f.LastName));

            if (!string.IsNullOrWhiteSpace(f.Adress))
                pQuery = pQuery.Where(p => p.Adress!.Contains(f.Adress));

            if (!string.IsNullOrWhiteSpace(f.PhoneNumber))
                pQuery = pQuery.Where(p => p.PhoneNumber!.Contains(f.PhoneNumber));

            if (!string.IsNullOrWhiteSpace(f.Dui))
                pQuery = pQuery.Where(p => p.Dui!.Contains(f.Dui));

            if (f.StatusId > 0)
                pQuery = pQuery.Where(p => p.StatusId == f.StatusId);

            if (pPagedQuery.FromDate.HasValue)
                pQuery = pQuery.Where(p => p.CreatedAt >= pPagedQuery.FromDate.Value);

            if (pPagedQuery.ToDate.HasValue)
                pQuery = pQuery.Where(p => p.CreatedAt <= pPagedQuery.ToDate.Value);

            return pQuery
                .OrderBy(p => p.LastName)
                    .ThenBy(p => p.FirstName);
        }

        #endregion

        #region "CRUD"

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

        #endregion

        #region "Búsqueda Avanzada con Paginación"

        /// <summary>
        /// Realiza una búsqueda avanzada de personas con soporte para paginación
        /// según los criterios especificados en <paramref name="pPagedQuery"/>.
        /// Si <c>Top</c> es mayor a cero, devuelve únicamente los primeros <c>Top</c> registros
        /// ignorando los parámetros de paginación.
        /// </summary>
        /// <param name="pPagedQuery">
        /// Objeto <see cref="PagedQuery{Person}"/> que define los filtros, el tamaño de página,
        /// el número de página y otros parámetros de búsqueda. No puede ser <c>null</c>.
        /// </param>
        /// <returns>
        /// Objeto <see cref="PagedResult{Person}"/> con la lista de personas encontradas
        /// e información de paginación (total de registros, página actual, tamaño de página).
        /// </returns>
        /// <exception cref="Exception">
        /// Se lanza si ocurre un error durante la ejecución de la consulta o el acceso a la base de datos.
        /// </exception>
        public static async Task<PagedResult<Person>> BuscarAsync(PagedQuery<Person> pPagedQuery)
        {
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    var baseQuery = dbContexto.Person
                        .Include(p => p.Status)
                        .AsQueryable();

                    var filtered = QuerySelect(baseQuery, pPagedQuery);
                    int total    = await filtered.CountAsync();

                    List<Person> items;

                    if (pPagedQuery.Top > 0)
                    {
                        items = await filtered
                            .Take(pPagedQuery.Top)
                            .ToListAsync();
                    }
                    else
                    {
                        items = await filtered
                            .Skip(pPagedQuery.Skip)
                            .Take(pPagedQuery.PageSize)
                            .ToListAsync();
                    }

                    return new PagedResult<Person>
                    {
                        Items       = items,
                        TotalCount  = total,
                        CurrentPage = pPagedQuery.Page,
                        PageSize    = pPagedQuery.PageSize
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}