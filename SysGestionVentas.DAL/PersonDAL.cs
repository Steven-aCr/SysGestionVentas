using SysGestionVentas.EN;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SysGestionVentas.DAL
{
    public class PersonDAL
    {
        /// <summary>
        /// Verifica si ya existe una persona con el mismo DUI.
        /// </summary>
        private static async Task<bool> ExisteDui(Person pPerson, DbContexto pDBContexto)
        {
            var personExiste = await pDBContexto.Person.FirstOrDefaultAsync(
                p => p.Dui == pPerson.Dui && p.PersonId != pPerson.PersonId);

            return (personExiste != null && personExiste.PersonId > 0);
        }

        /// <summary>
        /// Verifica si ya existe una persona con el mismo número de teléfono.
        /// </summary>
        private static async Task<bool> ExistePhone(Person pPerson, DbContexto pDBContexto)
        {
            var personExiste = await pDBContexto.Person.FirstOrDefaultAsync(
                p => p.PhoneNumber == pPerson.PhoneNumber && p.PersonId != pPerson.PersonId);

            return (personExiste != null && personExiste.PersonId > 0);
        }

        /// <summary>
        /// Registra una nueva persona en la base de datos.
        /// </summary>
        /// <param name="pPerson">Objeto <see cref="Person"/> con los datos a guardar.</param>
        /// <returns>Número de filas afectadas.</returns>
        /// <exception cref="Exception">
        /// Se lanza si el DUI o teléfono ya existen o si ocurre un error.
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
        /// Modifica los datos de una persona existente.
        /// </summary>
        /// <param name="pPerson">Objeto <see cref="Person"/> con los datos actualizados.</param>
        /// <returns>Número de filas afectadas.</returns>
        /// <exception cref="Exception">
        /// Se lanza si la persona no existe o si hay duplicados.
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

                    person.FirstName = pPerson.FirstName;
                    person.LastName = pPerson.LastName;
                    person.Adress = pPerson.Adress;
                    person.PhoneNumber = pPerson.PhoneNumber;
                    person.Dui = pPerson.Dui;
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
        /// Realiza una eliminación lógica de una persona.
        /// </summary>
        /// <param name="pPerson">
        /// Objeto <see cref="Person"/> con el <c>PersonId</c> y el nuevo <c>StatusId</c>.
        /// </param>
        /// <returns>Número de filas afectadas.</returns>
        /// <exception cref="Exception">
        /// Se lanza si la persona no existe.
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

                    // Eliminación lógica
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
        /// Obtiene una persona por su ID incluyendo su estado.
        /// </summary>
        /// <param name="pPerson">Objeto <see cref="Person"/> con el ID a buscar.</param>
        /// <returns>Persona encontrada o <c>null</c>.</returns>
        public static async Task<Person> ObtenerPorIdAsync(Person pPerson)
        {
            var result = new Person();
            try
            {
                using (var dbContexto = new DbContexto())
                {
                    result = await dbContexto.Person
                        .Include(p => p.Status)
                        .FirstOrDefaultAsync(p => p.PersonId == pPerson.PersonId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Obtiene una lista de personas aplicando filtros opcionales.
        /// </summary>
        /// <param name="pPerson">
        /// Filtros:
        /// <list type="bullet">
        /// <item><description><c>FirstName</c>: búsqueda por nombre.</description></item>
        /// <item><description><c>LastName</c>: búsqueda por apellido.</description></item>
        /// <item><description><c>Dui</c>: búsqueda por DUI.</description></item>
        /// <item><description><c>StatusId</c>: filtro por estado.</description></item>
        /// </list>
        /// </param>
        /// <returns>Lista de personas.</returns>
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
                            (pPerson.FirstName == null || p.FirstName.Contains(pPerson.FirstName)) &&
                            (pPerson.LastName == null || p.LastName.Contains(pPerson.LastName)) &&
                            (pPerson.Dui == null || p.Dui.Contains(pPerson.Dui)) &&
                            (pPerson.StatusId == 0 || p.StatusId == pPerson.StatusId)
                        )
                        .OrderBy(p => p.FirstName)
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