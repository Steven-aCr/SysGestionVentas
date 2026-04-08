using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.BL;
using SysGestionVentas.Test.Builders;
using Microsoft.EntityFrameworkCore.InMemory;

namespace SysGestionVentas.Test
{
    public class CreateUserTest : IDisposable
    {
        private readonly DbContextOptions<DbContexto> _options;

        /// <summary>
        /// Inicializa la BD InMemory con nombre único por instancia,
        /// la inyecta en <see cref="DbContexto.TestOptions"/> para que
        /// todos los <c>new DbContexto()</c> internos del DAL la usen,
        /// y precarga los datos semilla.
        /// </summary>
        public CreateUserTest()
        {
            _options = new DbContextOptionsBuilder<DbContexto>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            DbContexto.TestOptions = _options;

            SeedDatabase();
        }

        /// <summary>
        /// Libera la BD InMemory y limpia <see cref="DbContexto.TestOptions"/>
        /// al finalizar cada prueba para no afectar otras pruebas.
        /// </summary>
        public void Dispose()
        {
            using var db = new DbContexto(_options);
            db.Database.EnsureDeleted();

            // ─── Limpiar TestOptions para no contaminar otras clases de prueba ──
            DbContexto.TestOptions = null;
        }

        /// <summary>
        /// Carga datos semilla en la BD InMemory:
        /// StatusTypes, Statuses, Roles, Persona y cuatro usuarios de prueba.
        /// </summary>
        private void SeedDatabase()
        {
            using var db = new DbContexto(_options);

            db.StatusType.AddRange(TestData.CrearStatusTypes());
            db.Status.AddRange(TestData.CrearStatuses());
            db.Rol.AddRange(TestData.CrearRoles());
            db.Person.Add(TestData.CrearPersona());
            db.User.Add(TestData.CrearUsuarioActivo());
            db.User.Add(TestData.CrearUsuarioSuspendido());
            db.User.Add(TestData.CrearUsuarioConPasswordTemporal());
            db.User.Add(TestData.CrearUsuarioConPasswordTemporalVencida());

            db.SaveChanges();
        }

        private DbContexto CrearContexto() => new(_options);


        #region "CrearConPersonaAsync"

        [Fact]
        public async Task CrearConPersonaAsync_Exitoso()
        {
            var model = TestData.CrearModeloUsuarioNuevo();
            int result = await UserBL.CrearConPersonaAsync(model);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task CrearConPersonaAsync_PersonaCreada()
        {
            var model = TestData.CrearModeloUsuarioNuevo();
            await UserBL.CrearConPersonaAsync(model);

            using var db = CrearContexto();
            var persona = await db.Person.FirstOrDefaultAsync(
                p => p.PhoneNumber == model.PhoneNumber);

            Assert.NotNull(persona);
            Assert.Equal(model.FirstName, persona.FirstName);
            Assert.Equal(model.LastName, persona.LastName);
        }

        [Fact]
        public async Task CrearConPersonaAsync_UsuarioCreado()
        {
            var model = TestData.CrearModeloUsuarioNuevo();
            await UserBL.CrearConPersonaAsync(model);

            using var db = CrearContexto();
            var usuario = await db.User.FirstOrDefaultAsync(
                u => u.Email == model.Email);

            Assert.NotNull(usuario);
            Assert.Equal(model.UserName, usuario.UserName);
        }

        [Fact]
        public async Task CrearConPersonaAsync_UsuarioVinculadoAPersona()
        {
            var model = TestData.CrearModeloUsuarioNuevo();
            await UserBL.CrearConPersonaAsync(model);

            using var db = CrearContexto();
            var usuario = await db.User
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            Assert.NotNull(usuario);
            Assert.NotNull(usuario.Person);
            Assert.Equal(model.FirstName, usuario.Person.FirstName);
        }

        [Fact]
        public async Task CrearConPersonaAsync_PasswordEncriptada()
        {
            var model = TestData.CrearModeloUsuarioNuevo();
            string passwordPlano = model.Password;

            await UserBL.CrearConPersonaAsync(model);

            using var db = CrearContexto();
            var usuario = await db.User.FirstOrDefaultAsync(u => u.Email == model.Email);
            Assert.NotNull(usuario);
            Assert.NotEqual(passwordPlano, usuario.PasswordHash);
            Assert.Equal(TestData.Sha256(passwordPlano), usuario.PasswordHash);
        }

        [Fact]
        public async Task CrearConPersonaAsync_UserNameVacio()
        {
            var model = TestData.CrearModeloUsuarioNuevo();
            model.UserName = string.Empty;

            var ex = await Assert.ThrowsAsync<Exception>(
                () => UserBL.CrearConPersonaAsync(model));

            Assert.Contains("usuario", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CrearConPersonaAsync_EmailVacio()
        {
            var model = TestData.CrearModeloUsuarioNuevo();
            model.Email = string.Empty;

            var ex = await Assert.ThrowsAsync<Exception>(
                () => UserBL.CrearConPersonaAsync(model));

            Assert.Contains("correo", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CrearConPersonaAsync_PasswordCorta()
        {
            var model = TestData.CrearModeloUsuarioNuevo();
            model.Password = "Corta1";

            var ex = await Assert.ThrowsAsync<Exception>(
                () => UserBL.CrearConPersonaAsync(model));

            Assert.Contains("8 caracteres", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CrearConPersonaAsync_RollbackSiUsuarioFalla()
        {
            var model = TestData.CrearModeloUsuarioNuevo();
            model.UserName = TestData.USERNAME_ACTIVO; // username duplicado → falla en User

            await Assert.ThrowsAsync<Exception>(
                () => UserBL.CrearConPersonaAsync(model));

            // La Person NO debe haberse persistido por el rollback
            using var db = CrearContexto();
            var persona = await db.Person.FirstOrDefaultAsync(
                p => p.PhoneNumber == model.PhoneNumber);

            Assert.Null(persona);
        }

        #endregion
    }
}
