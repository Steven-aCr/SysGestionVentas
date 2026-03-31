using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SysGestionVentas.EN;

namespace SysGestionVentas.DAL
{
    public class DbContexto : DbContext
    {
        #region "DbSets"

        public DbSet<Status> Status { get; set; }
        public DbSet<StatusType> StatusType { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Permission> Permission { get; set; }
        public DbSet<RolPermission> RolPermission { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<ProductList> ProductList { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<InventoryMovement> InventoryMovement { get; set; }
        public DbSet<Discount> Discount { get; set; }
        public DbSet<ProductDiscount> ProductDiscount { get; set; }
        public DbSet<DocumentType> DocumentType { get; set; }
        public DbSet<Document> Document { get; set; }
        public DbSet<DocumentDetail> DocumentDetail { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<MovementType> MovementType { get; set; }

        #endregion

        #region "TestOptions (solo para pruebas unitarias)"

        /// <summary>
        /// Opciones de contexto inyectadas desde pruebas unitarias (xUnit).
        /// Cuando esta propiedad es distinta de <c>null</c>, <see cref="OnConfiguring"/>
        /// copia sus extensiones al builder actual, permitiendo que todos los
        /// <c>new DbContexto()</c> internos del DAL operen sobre la misma BD
        /// InMemory del test sin requerir referencia al paquete InMemory en el DAL.
        /// <para>
        /// <b>Importante:</b> debe limpiarse a <c>null</c> en el <c>Dispose</c>
        /// del test para no contaminar otras clases de prueba.
        /// </para>
        /// </summary>
        public static DbContextOptions<DbContexto>? TestOptions { get; set; }

        #endregion

        #region "Constructores"

        /// <summary>
        /// Constructor sin parámetros utilizado por el DAL en producción
        /// mediante <c>new DbContexto()</c>. Delega la configuración de la
        /// cadena de conexión a <see cref="OnConfiguring"/>.
        /// </summary>
        public DbContexto() { }

        /// <summary>
        /// Constructor con opciones externas. Utilizado desde pruebas unitarias
        /// para inyectar una base de datos InMemory u otro proveedor de prueba,
        /// evitando que <see cref="OnConfiguring"/> sobreescriba la configuración.
        /// </summary>
        /// <param name="options">Opciones de configuración provistas externamente.</param>
        public DbContexto(DbContextOptions<DbContexto> options) : base(options) { }

        #endregion

        #region "Configuración del modelo y conexión"

        /// <summary>
        /// Configura el modelo de datos aplicando todas las clases
        /// <see cref="IEntityTypeConfiguration{T}"/> del ensamblado.
        /// </summary>
        /// <param name="modelBuilder">Constructor del modelo. No puede ser nulo.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContexto).Assembly);
        }

        /// <summary>
        /// Configura la fuente de datos del contexto siguiendo esta prioridad:
        /// <list type="number">
        ///   <item>
        ///     Si el contexto ya fue configurado externamente (constructor con opciones),
        ///     no realiza ninguna acción.
        ///   </item>
        ///   <item>
        ///     Si <see cref="TestOptions"/> está definido (entorno de pruebas unitarias),
        ///     copia sus extensiones al builder actual mediante la interfaz de infraestructura
        ///     de EF Core, sin requerir referencia al proveedor InMemory en este ensamblado.
        ///   </item>
        ///   <item>
        ///     En caso contrario, aplica la cadena de conexión a SQL Server de producción.
        ///   </item>
        /// </list>
        /// </summary>
        /// <param name="optionsBuilder">Constructor de opciones del contexto.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            if (TestOptions != null)
            {
                var infrastructure = (IDbContextOptionsBuilderInfrastructure)optionsBuilder;
                foreach (var extension in TestOptions.Extensions)
                    infrastructure.AddOrUpdateExtension(extension);

                return;
            }

            optionsBuilder.UseSqlServer(
                "Data Source=BDGestionVentas.mssql.somee.com; Initial Catalog=BDGestionVentas;" +
                "User ID=DevCore_SQLLogin_1; Password=u7z7prei6q;" +
                "Connect Timeout=30;Encrypt=False;" +
                "Trust Server Certificate=False;" +
                "Application Intent=ReadWrite; Multi Subnet Failover=False;" +
                "Command Timeout=30");
        }

        #endregion
    }
}