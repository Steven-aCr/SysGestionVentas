using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;

namespace SysGestionVentas.DAL
{
    public class DbContexto : DbContext
    {
        #region "Contexto principal de base de datos"

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

        #region "Soporte para pruebas unitarias"

        /// <summary>
        /// Opciones de contexto inyectadas desde pruebas unitarias.
        /// Cuando este campo no es <c>null</c>, el constructor sin parámetros
        /// lo utiliza en lugar de la cadena de conexión a SQL Server,
        /// permitiendo que el DAL opere sobre la BD InMemory del test
        /// sin modificar ningún método de negocio.
        /// <para>
        /// <b>Solo debe asignarse desde la capa de pruebas.</b>
        /// En producción permanece <c>null</c> y no tiene ningún efecto.
        /// </para>
        /// </summary>
        public static DbContextOptions<DbContexto>? TestOptions { get; set; }

        #endregion

        #region "Constructores"

        /// <summary>
        /// Constructor utilizado por el DAL en producción mediante <c>new DbContexto()</c>.
        /// Si <see cref="TestOptions"/> fue configurado desde las pruebas unitarias,
        /// lo usa directamente. De lo contrario, <see cref="OnConfiguring"/> aplica
        /// la cadena de conexión a SQL Server.
        /// </summary>
        public DbContexto() : base(GetOptions()) { }

        /// <summary>
        /// Constructor con opciones externas. Utilizado opcionalmente desde pruebas
        /// para crear instancias adicionales del contexto (seed, verificación).
        /// </summary>
        /// <param name="options">Opciones de configuración provistas externamente.</param>
        public DbContexto(DbContextOptions<DbContexto> options) : base(options) { }

        /// <summary>
        /// Retorna las opciones a usar en el constructor sin parámetros.
        /// Prioriza <see cref="TestOptions"/> si está configurado,
        /// caso contrario retorna opciones vacías para que <see cref="OnConfiguring"/>
        /// aplique la cadena SQL Server.
        /// </summary>
        private static DbContextOptions<DbContexto> GetOptions()
        {
            return TestOptions
                ?? new DbContextOptionsBuilder<DbContexto>().Options;
        }
        #endregion

        #region "Configuración de la cadena de conexión"

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
        /// Configura la cadena de conexión a SQL Server para producción.
        /// Solo actúa si el contexto no fue configurado previamente,
        /// lo que ocurre cuando <see cref="TestOptions"/> es <c>null</c>.
        /// </summary>
        /// <param name="optionsBuilder">Constructor de opciones del contexto.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Data Source=BDGestionVentas.mssql.somee.com;" +
                    "Initial Catalog=BDGestionVentas;" +
                    "Connect Timeout=30;Encrypt=False;" +
                    "Trust Server Certificate=False;" +
                    "Application Intent=ReadWrite;" +
                    "Multi Subnet Failover=False;" +
                    "Command Timeout=30");
            }
        }

        #endregion
    }
}