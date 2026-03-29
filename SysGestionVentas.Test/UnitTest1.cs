using SysGestionVentas.DAL;
using SysGestionVentas.EN;
using Xunit;

namespace SysGestionVentas.Test
{
    // =============================================
    //           PRUEBAS DE ROL
    // =============================================
    public class UnitTest1
    {
        private readonly Rol ObjRol = new Rol();

        [Fact]
        public async Task PruebaGuardarRol()
        {
            ObjRol.Name = "Administrador";
            ObjRol.Description = "Rol con acceso total";
            ObjRol.StatusId = 1;

            int Resultado = await RolDAL.GuardarAsync(ObjRol);
            Assert.Equal(1, Resultado);
        }
    }

}

//        [Fact]
//        public async Task PruebaObtenerTodosRoles()
//        {
//            var filtro = new Rol();
//            List<Rol> Resultado = await RolDAL.ObtenerTodosAsync(filtro);
//            Assert.NotNull(Resultado);
//            Assert.NotEqual(0, Resultado.Count);
//        }

//        [Fact]
//        public async Task PruebaObtenerRolPorId()
//        {
//            ObjRol.RolId = 1;
//            Rol? Resultado = await RolDAL.ObtenerPorIdAsync(ObjRol);
//            Assert.NotNull(Resultado);
//        }

//        [Fact]
//        public async Task PruebaModificarRol()
//        {
//            ObjRol.RolId = 1;
//            ObjRol.Name = "Administrador Actualizado";
//            ObjRol.Description = "Descripcion actualizada";
//            ObjRol.StatusId = 1;

//            int Resultado = await RolDAL.ModificarAsync(ObjRol);
//            Assert.Equal(1, Resultado);
//        }

//        [Fact]
//        public async Task PruebaEliminarRol()
//        {
//            ObjRol.RolId = 1;
//            ObjRol.StatusId = 2; // Estado inactivo

//            int Resultado = await RolDAL.EliminarAsync(ObjRol);
//            Assert.Equal(1, Resultado);
//        }
//    }

//    // =============================================
//    //           PRUEBAS DE PERMISSION
//    // =============================================
    
//        private readonly Permission ObjPermission = new Permission();

//        [Fact]
//        public async Task PruebaGuardarPermission()
//        {
//            ObjPermission.Name = "Ver Usuarios";
//            ObjPermission.Description = "Permiso para ver la lista de usuarios";
//            ObjPermission.IsActive = true;

//            int Resultado = await PermissionDAL.GuardarAsync(ObjPermission);
//            Assert.Equal(1, Resultado);
//        }

//        [Fact]
//        public async Task PruebaObtenerTodosPermissions()
//        {
//            var filtro = new Permission();
//            List<Permission> Resultado = await PermissionDAL.ObtenerTodosAsync(filtro);
//            Assert.NotNull(Resultado);
//            Assert.NotEqual(0, Resultado.Count);
//        }

//        [Fact]
//        public async Task PruebaObtenerPermissionPorId()
//        {
//            ObjPermission.PermissionId = 1;
//            Permission? Resultado = await PermissionDAL.ObtenerPorIdAsync(ObjPermission);
//            Assert.NotNull(Resultado);
//        }

//        [Fact]
//        public async Task PruebaModificarPermission()
//        {
//            ObjPermission.PermissionId = 1;
//            ObjPermission.Name = "Ver Usuarios Actualizado";
//            ObjPermission.Description = "Descripcion actualizada";
//            ObjPermission.IsActive = true;

//            int Resultado = await PermissionDAL.ModificarAsync(ObjPermission);
//            Assert.Equal(1, Resultado);
//        }

//        [Fact]
//        public async Task PruebaEliminarPermission()
//        {
//            ObjPermission.PermissionId = 1;

//            int Resultado = await PermissionDAL.EliminarAsync(ObjPermission);
//            Assert.Equal(1, Resultado);
//        }
    

//    // =============================================
//    //           PRUEBAS DE USER
//    // =============================================
//     class UsersTestpubli
//    {
//        private readonly User ObjUser = new User();

//        [Fact]
//        public async Task PruebaGuardarUsuario()
//        {
//            ObjUser.UserName = "prueba_test";
//            ObjUser.Email = "prueba@gmail.com";
//            ObjUser.PasswordHash = "123456";
//            ObjUser.RolId = 1;
//            ObjUser.StatusId = 1;

//            int Resultado = await UsersDAL.GuardarAsync(ObjUser);
//            Assert.Equal(1, Resultado);
//        }

//        [Fact]
//        public async Task PruebaObtenerTodosUsuarios()
//        {
//            var filtro = new User();
//            List<User> Resultado = await UsersDAL.ObtenerTodosAsync(filtro);
//            Assert.NotNull(Resultado);
//            Assert.NotEqual(0, Resultado.Count);
//        }

//        [Fact]
//        public async Task PruebaObtenerUsuarioPorId()
//        {
//            ObjUser.UserId = 1;
//            User? Resultado = await UsersDAL.ObtenerPorIdAsync(ObjUser);
//            Assert.NotNull(Resultado);
//        }

//        [Fact]
//        public async Task PruebaModificarUsuario()
//        {
//            ObjUser.UserId = 1;
//            ObjUser.UserName = "prueba_actualizado";
//            ObjUser.Email = "actualizado@gmail.com";
//            ObjUser.RolId = 1;
//            ObjUser.StatusId = 1;

//            int Resultado = await UsersDAL.ModificarAsync(ObjUser);
//            Assert.Equal(1, Resultado);
//        }

//        [Fact]
//        public async Task PruebaEliminarUsuario()
//        {
//            ObjUser.UserId = 1;
//            ObjUser.StatusId = 2; // Estado inactivo

//            int Resultado = await UsersDAL.EliminarAsync(ObjUser);
//            Assert.Equal(1, Resultado);
//        }
//    }
//}