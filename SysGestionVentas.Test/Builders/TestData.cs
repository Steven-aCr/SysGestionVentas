using SysGestionVentas.EN;
using System.Security.Cryptography;
using System.Text;

namespace SysGestionVentas.Test.Builders
{
    public static class TestData
    {
        /// ─── Constantes para IDs y datos de prueba ────────────────────────────────

        public const int STATUS_ACTIVO_ID = 1;
        public const int STATUS_INACTIVO_ID = 2;
        public const int STATUS_SUSPENDIDO_ID = 3;

        //  Constantes para IDs de roles
        public const int ROL_ADMIN_ID = 1;
        public const int ROL_VENDEDOR_ID = 2;

        public const int PERSON_ID = 1;

        public const string PASSWORD_PLANO = "Password123";

        public const string EMAIL_ACTIVO = "activo@test.com";

        public const string USERNAME_ACTIVO = "usuario.activo";

        // ─── Método de utilidad para hashear contraseñas ─────────────────────────
        public static string Sha256(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        // ─── Entidades de soporte ────────────────────────────────────────────────

        public static List<Status> CrearStatuses() => new()
        {
            new Status { StatusId = STATUS_ACTIVO_ID,     Name = "Activo",     IsActive = true,  StatusTypeId = 1 },
            new Status { StatusId = STATUS_INACTIVO_ID,   Name = "Inactivo",   IsActive = true,  StatusTypeId = 1 },
            new Status { StatusId = STATUS_SUSPENDIDO_ID, Name = "Suspendido", IsActive = true,  StatusTypeId = 2 },
        };

        public static List<StatusType> CrearStatusTypes() => new()
        {
            new StatusType { StatusTypeId = 1, Name = "Estado General",    IsActive = true },
            new StatusType { StatusTypeId = 2, Name = "Estado de Usuario", IsActive = true },
        };

        public static Person CrearPersona(int personId = PERSON_ID) => new()
        {
            PersonId = personId,
            FirstName = "Usuario",
            LastName = "Prueba",
            Adress = "Dirección de prueba 123",
            PhoneNumber = $"7000-000{personId}",
            StatusId = STATUS_ACTIVO_ID,
            CreatedAt = DateTime.UtcNow,
        };

        public static List<Rol> CrearRoles() => new()
        {
            new Rol { RolId = ROL_ADMIN_ID,    Name = "Administrador", StatusId = STATUS_ACTIVO_ID,   CreatedAt = DateTime.UtcNow },
            new Rol { RolId = ROL_VENDEDOR_ID, Name = "Vendedor",      StatusId = STATUS_ACTIVO_ID,   CreatedAt = DateTime.UtcNow },
        };

        public static List<Permission> CrearPermisos() => new()
        {
            new Permission { PermissionId = 1, Name = "Crear Usuario",    IsActive = true, CreatedAt = DateTime.UtcNow },
            new Permission { PermissionId = 2, Name = "Editar Usuario",   IsActive = true, CreatedAt = DateTime.UtcNow },
            new Permission { PermissionId = 3, Name = "Eliminar Usuario", IsActive = true, CreatedAt = DateTime.UtcNow },
            new Permission { PermissionId = 4, Name = "Ver Reportes",     IsActive = true, CreatedAt = DateTime.UtcNow },
        };

        // ─── Usuario semilla activo ──────────────────────────────────────────────

        public static User CrearUsuarioActivo(
            int userId = 1,
            string userName = USERNAME_ACTIVO,
            string email = EMAIL_ACTIVO,
            int personId = PERSON_ID) => new()
            {
                UserId = userId,
                UserName = userName,
                Email = email,
                PasswordHash = Sha256(PASSWORD_PLANO),
                RolId = ROL_ADMIN_ID,
                PersonId = personId,
                StatusId = STATUS_ACTIVO_ID,
                MustChangePassword = false,
                CreatedAt = DateTime.UtcNow,
            };
        public static User CrearUsuarioSuspendido() => new()
        {
            UserId = 90,
            UserName = "usuario.suspendido",
            Email = "suspendido@test.com",
            PasswordHash = Sha256(PASSWORD_PLANO),
            RolId = ROL_VENDEDOR_ID,
            PersonId = PERSON_ID,
            StatusId = STATUS_SUSPENDIDO_ID,
            MustChangePassword = false,
            CreatedAt = DateTime.UtcNow,
        };

        public static User CrearUsuarioConPasswordTemporal(string tempPassword = "TempPass99") => new()
        {
            UserId = 91,
            UserName = "usuario.temp",
            Email = "temp@test.com",
            PasswordHash = Sha256(PASSWORD_PLANO),
            RolId = ROL_VENDEDOR_ID,
            PersonId = PERSON_ID,
            StatusId = STATUS_ACTIVO_ID,
            TempPasswordHash = Sha256(tempPassword),
            TempPasswordExpiry = DateTime.UtcNow.AddHours(1),
            MustChangePassword = true,
            CreatedAt = DateTime.UtcNow,
        };

        public static User CrearUsuarioConPasswordTemporalVencida() => new()
        {
            UserId = 92,
            UserName = "usuario.tempvencida",
            Email = "tempvencida@test.com",
            PasswordHash = Sha256(PASSWORD_PLANO),
            RolId = ROL_VENDEDOR_ID,
            PersonId = PERSON_ID,
            StatusId = STATUS_ACTIVO_ID,
            TempPasswordHash = Sha256("TempExpirada"),
            TempPasswordExpiry = DateTime.UtcNow.AddHours(-2),
            MustChangePassword = true,
            CreatedAt = DateTime.UtcNow,
        };

        public static User CrearUsuarioNuevo(
            string userName = "nuevo.usuario",
            string email = "nuevo@test.com") => new()
            {
                UserId = 0,
                UserName = userName,
                Email = email,
                PasswordHash = PASSWORD_PLANO,
                RolId = ROL_VENDEDOR_ID,
                PersonId = PERSON_ID,
                StatusId = STATUS_ACTIVO_ID,
                CreatedAt = DateTime.UtcNow,
            };
    }
}