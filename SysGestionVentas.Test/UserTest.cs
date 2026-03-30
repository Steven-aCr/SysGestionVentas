using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.Pagination;
using SysGestionVentas.Test.Builders;
using Xunit;

namespace SysGestionVentas.Test
{
    /// <summary>
    /// Pruebas unitarias para <see cref="UsersDAL"/>.
    /// Usa EF Core InMemory inyectado via <see cref="DbContexto.TestOptions"/>
    /// para que el DAL opere sobre la BD de prueba sin tocar SQL Server.
    /// </summary>
    public class UserTest : IDisposable
    {
        private readonly DbContextOptions<DbContexto> _options;

        /// <summary>
        /// Inicializa la BD InMemory con nombre único por instancia,
        /// la inyecta en <see cref="DbContexto.TestOptions"/> para que
        /// todos los <c>new DbContexto()</c> internos del DAL la usen,
        /// y precarga los datos semilla.
        /// </summary>
        public UserTest()
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

        #region "GuardarAsync"

        [Fact]
        public async Task GuardarAsync_Usuario()
        {
            var nuevoUsuario = TestData.CrearUsuarioNuevo("juan.perez", "juan@test.com");
            int result = await UsersDAL.GuardarAsync(nuevoUsuario);
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task GuardarAsync_PasswordHash()
        {
            var nuevoUsuario = TestData.CrearUsuarioNuevo("maria.lopez", "maria@test.com");
            string passwordPlano = nuevoUsuario.PasswordHash!;

            await UsersDAL.GuardarAsync(nuevoUsuario);

            using var db = CrearContexto();
            var guardado = await db.User.FirstAsync(u => u.Email == "maria@test.com");
            Assert.NotEqual(passwordPlano, guardado.PasswordHash);
            Assert.Equal(TestData.Sha256(passwordPlano), guardado.PasswordHash);
        }

        [Fact]
        public async Task GuardarAsync_UserNameDuplicado()
        {
            var duplicado = TestData.CrearUsuarioNuevo(
                userName: TestData.USERNAME_ACTIVO,
                email: "diferente@test.com");

            var ex = await Assert.ThrowsAsync<Exception>(
                () => UsersDAL.GuardarAsync(duplicado));

            Assert.Contains("nombre de usuario", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GuardarAsync_EmailDuplicado()
        {
            var duplicado = TestData.CrearUsuarioNuevo(
                userName: "username.unico",
                email: TestData.EMAIL_ACTIVO);

            var ex = await Assert.ThrowsAsync<Exception>(
                () => UsersDAL.GuardarAsync(duplicado));

            Assert.Contains("correo", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GuardarAsync_MustChangePassword()
        {
            var nuevoUsuario = TestData.CrearUsuarioNuevo("ana.garcia", "ana@test.com");
            nuevoUsuario.MustChangePassword = true;

            await UsersDAL.GuardarAsync(nuevoUsuario);

            using var db = CrearContexto();
            var guardado = await db.User.FirstAsync(u => u.Email == "ana@test.com");
            Assert.False(guardado.MustChangePassword);
        }

        #endregion

        #region "ModificarAsync"

        [Fact]
        public async Task ModificarAsync()
        {
            var usuarioModificado = new User
            {
                UserId = 1,
                UserName = "usuario.modificado",
                Email = "modificado@test.com",
                RolId = TestData.ROL_VENDEDOR_ID,
                PersonId = TestData.PERSON_ID,
                StatusId = TestData.STATUS_ACTIVO_ID,
            };

            int result = await UsersDAL.ModificarAsync(usuarioModificado);

            Assert.Equal(1, result);
            using var db = CrearContexto();
            var actualizado = await db.User.FindAsync(1);
            Assert.Equal("usuario.modificado", actualizado!.UserName);
            Assert.Equal("modificado@test.com", actualizado.Email);
        }

        [Fact]
        public async Task ModificarAsync_UsuarioInexistente()
        {
            var usuarioInexistente = new User { UserId = 9999, UserName = "x", Email = "x@x.com" };

            var ex = await Assert.ThrowsAsync<Exception>(
                () => UsersDAL.ModificarAsync(usuarioInexistente));

            Assert.Contains("9999", ex.Message);
        }

        [Fact]
        public async Task ModificarAsync_UserNameDuplicado()
        {
            var conflicto = new User
            {
                UserId = 1,
                UserName = "usuario.suspendido",
                Email = "sinconflicto@test.com",
                RolId = TestData.ROL_ADMIN_ID,
                PersonId = TestData.PERSON_ID,
                StatusId = TestData.STATUS_ACTIVO_ID,
            };

            var ex = await Assert.ThrowsAsync<Exception>(
                () => UsersDAL.ModificarAsync(conflicto));

            Assert.Contains("nombre de usuario", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region "EliminarAsync"

        [Fact]
        public async Task EliminarAsync_Usuario()
        {
            var solicitud = new User
            {
                UserId = 1,
                StatusId = TestData.STATUS_INACTIVO_ID,
            };

            int result = await UsersDAL.EliminarAsync(solicitud);

            Assert.Equal(1, result);
            using var db = CrearContexto();
            var usuario = await db.User.FindAsync(1);
            Assert.NotNull(usuario);
            Assert.Equal(TestData.STATUS_INACTIVO_ID, usuario.StatusId);
        }

        [Fact]
        public async Task EliminarAsync_UsuarioInexistente()
        {
            var solicitud = new User { UserId = 9999, StatusId = TestData.STATUS_INACTIVO_ID };

            await Assert.ThrowsAsync<Exception>(() => UsersDAL.EliminarAsync(solicitud));
        }

        #endregion

        #region "ObtenerPorIdAsync"

        [Fact]
        public async Task ObtenerPorIdAsync_Usuario()
        {
            var solicitud = new User { UserId = 1 };
            var usuario = await UsersDAL.ObtenerPorIdAsync(solicitud);

            Assert.NotNull(usuario);
            Assert.Equal(1, usuario.UserId);
            Assert.NotNull(usuario.Rol);
            Assert.NotNull(usuario.Status);
        }

        [Fact]
        public async Task ObtenerPorIdAsync_UsuarioInexistente()
        {
            var solicitud = new User { UserId = 9999 };
            var usuario = await UsersDAL.ObtenerPorIdAsync(solicitud);

            Assert.Null(usuario);
        }

        #endregion

        #region "ObtenerTodosAsync"

        [Fact]
        public async Task ObtenerTodosAsync_Usuario()
        {
            var filtro = new User();
            var lista = await UsersDAL.ObtenerTodosAsync(filtro);

            Assert.Equal(4, lista.Count);
        }

        [Fact]
        public async Task ObtenerTodosAsync_StatusActivo()
        {
            var filtro = new User { StatusId = TestData.STATUS_ACTIVO_ID };
            var lista = await UsersDAL.ObtenerTodosAsync(filtro);

            Assert.All(lista, u => Assert.Equal(TestData.STATUS_ACTIVO_ID, u.StatusId));
            Assert.Equal(3, lista.Count);
        }

        [Fact]
        public async Task ObtenerTodosAsync_FiltroUserName()
        {
            var filtro = new User { UserName = "usuario" };
            var lista = await UsersDAL.ObtenerTodosAsync(filtro);

            Assert.True(lista.Count >= 1);
            Assert.All(lista, u => Assert.Contains("usuario", u.UserName!, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task ObtenerTodosAsync_OrdenPorUserName()
        {
            var filtro = new User();
            var lista = await UsersDAL.ObtenerTodosAsync(filtro);

            var nombres = lista.Select(u => u.UserName).ToList();
            var ordenados = nombres.OrderBy(n => n).ToList();
            Assert.Equal(ordenados, nombres);
        }

        #endregion

        #region "BuscarAsync - Paginación"

        [Fact]
        public async Task BuscarAsync_SinFiltros()
        {
            var query = new PagedQuery<User>
            {
                Filter = new User(),
                Page = 1,
                PageSize = 10,
            };

            var resultado = await UsersDAL.BuscarAsync(query);

            Assert.Equal(4, resultado.TotalCount);
            Assert.Equal(1, resultado.CurrentPage);
        }

        [Fact]
        public async Task BuscarAsync_PageSize2()
        {
            var query = new PagedQuery<User>
            {
                Filter = new User(),
                Page = 1,
                PageSize = 2,
            };

            var resultado = await UsersDAL.BuscarAsync(query);

            Assert.Equal(2, resultado.Items.Count);
            Assert.Equal(4, resultado.TotalCount);
        }

        [Fact]
        public async Task BuscarAsync_FiltroStatusSuspendido()
        {
            var query = new PagedQuery<User>
            {
                Filter = new User { StatusId = TestData.STATUS_SUSPENDIDO_ID },
                Page = 1,
                PageSize = 10,
            };

            var resultado = await UsersDAL.BuscarAsync(query);

            Assert.Equal(1, resultado.TotalCount);
            Assert.All(resultado.Items,
                u => Assert.Equal(TestData.STATUS_SUSPENDIDO_ID, u.StatusId));
        }

        [Fact]
        public async Task BuscarAsync_Top1()
        {
            var query = new PagedQuery<User>
            {
                Filter = new User(),
                Page = 1,
                PageSize = 10,
                Top = 1,
            };

            var resultado = await UsersDAL.BuscarAsync(query);

            Assert.Single(resultado.Items);
            Assert.Equal(4, resultado.TotalCount);
        }

        [Fact]
        public async Task BuscarAsync_FiltroFechas()
        {
            var query = new PagedQuery<User>
            {
                Filter = new User(),
                Page = 1,
                PageSize = 10,
                FromDate = DateTime.UtcNow.AddHours(-1),
                ToDate = DateTime.UtcNow.AddHours(1),
            };

            var resultado = await UsersDAL.BuscarAsync(query);

            Assert.Equal(4, resultado.TotalCount);
        }

        [Fact]
        public async Task BuscarAsync_PropiedadesPaginacion()
        {
            var query = new PagedQuery<User>
            {
                Filter = new User(),
                Page = 1,
                PageSize = 2,
            };

            var resultado = await UsersDAL.BuscarAsync(query);

            Assert.Equal(2, resultado.TotalPages);
            Assert.True(resultado.HasNextPage);
            Assert.False(resultado.HasPreviousPage);
            Assert.Equal(1, resultado.FirstItemIndex);
            Assert.Equal(2, resultado.LastItemIndex);
        }

        #endregion

        #region "LogingAsync"

        [Fact]
        public async Task LogingAsync_Usuario()
        {
            var usuario = await UsersDAL.LogingAsync(
                TestData.EMAIL_ACTIVO,
                TestData.PASSWORD_PLANO);

            Assert.NotNull(usuario);
            Assert.Equal(TestData.EMAIL_ACTIVO, usuario.Email);
        }

        [Fact]
        public async Task LogingAsync_PasswordIncorrecta()
        {
            var ex = await Assert.ThrowsAsync<Exception>(
                () => UsersDAL.LogingAsync(TestData.EMAIL_ACTIVO, "WrongPass999"));

            Assert.Contains("incorrecta", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task LogingAsync_EmailInexistente()
        {
            var ex = await Assert.ThrowsAsync<Exception>(
                () => UsersDAL.LogingAsync("noexiste@test.com", TestData.PASSWORD_PLANO));

            Assert.NotNull(ex);
        }

        [Fact]
        public async Task LogingAsync_PasswordTemporalVigente()
        {
            var usuario = await UsersDAL.LogingAsync("temp@test.com", "TempPass99");

            Assert.NotNull(usuario);
            using var db = CrearContexto();
            var actualizado = await db.User.FindAsync(91);
            Assert.Null(actualizado!.TempPasswordHash);
            Assert.Null(actualizado.TempPasswordExpiry);
        }

        [Fact]
        public async Task LogingAsync_PasswordTemporalVencida()
        {
            var usuario = await UsersDAL.LogingAsync(
                "tempvencida@test.com",
                TestData.PASSWORD_PLANO);

            Assert.NotNull(usuario);
        }

        #endregion

        #region "GenerarTempAsync"

        [Fact]
        public async Task GenerarTempAsync()
        {
            string tempPassword = await UsersDAL.GenerarTempAsync(1);

            Assert.False(string.IsNullOrWhiteSpace(tempPassword));
            using var db = CrearContexto();
            var usuario = await db.User.FindAsync(1);
            Assert.True(usuario!.MustChangePassword);
            Assert.NotNull(usuario.TempPasswordHash);
            Assert.Equal(TestData.Sha256(tempPassword), usuario.TempPasswordHash);
        }

        [Fact]
        public async Task GenerarTempAsync_Expiracion()
        {
            await UsersDAL.GenerarTempAsync(1);

            using var db = CrearContexto();
            var usuario = await db.User.FindAsync(1);
            Assert.NotNull(usuario!.TempPasswordExpiry);
            Assert.True(usuario.TempPasswordExpiry > DateTime.UtcNow.AddMinutes(55));
            Assert.True(usuario.TempPasswordExpiry < DateTime.UtcNow.AddMinutes(65));
        }

        [Fact]
        public async Task GenerarTempAsync_UsuarioInexistente()
        {
            await Assert.ThrowsAsync<Exception>(() => UsersDAL.GenerarTempAsync(9999));
        }

        [Fact]
        public async Task GenerarTempAsync_UsuarioSuspendido()
        {
            var ex = await Assert.ThrowsAsync<Exception>(
                () => UsersDAL.GenerarTempAsync(90));

            Assert.Contains("inactivo", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        #region "ChangePasswordAsync"

        [Fact]
        public async Task ChangePasswordAsync_MustChange()
        {
            string nuevaPassword = "NuevoPass456";
            int userId = 91;

            int result = await UsersDAL.ChangePasswordAsync(userId, nuevaPassword);

            Assert.Equal(1, result);
            using var db = CrearContexto();
            var usuario = await db.User.FindAsync(userId);
            Assert.False(usuario!.MustChangePassword);
            Assert.Equal(TestData.Sha256(nuevaPassword), usuario.PasswordHash);
        }

        [Fact]
        public async Task ChangePasswordAsync_SinMustChange()
        {
            var ex = await Assert.ThrowsAsync<Exception>(
                () => UsersDAL.ChangePasswordAsync(1, "NuevoPass456"));

            Assert.Contains("obligatorio", ex.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task ChangePasswordAsync_UsuarioInexistente()
        {
            await Assert.ThrowsAsync<Exception>(
                () => UsersDAL.ChangePasswordAsync(9999, "NuevoPass456"));
        }

        #endregion
    }
}