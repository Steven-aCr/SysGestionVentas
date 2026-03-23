using Microsoft.EntityFrameworkCore;
using SysGestionVentas.EN;
using System;


namespace SysGestionVentas.DAL
{
    public class DbContexto: DbContext
    {
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

        /// <summary>
        /// Configura el modelo de datos para el contexto actual utilizando las convenciones y las configuraciones
        /// aplicadas desde el ensamblado actual.
        /// </summary>
        /// <remarks>Este método se llama durante la inicialización del contexto para personalizar el
        /// modelo de Entity Framework. Utiliza todas las configuraciones de entidad definidas en el ensamblado del
        /// contexto.</remarks>
        /// <param name="modelBuilder">El generador de modelos utilizado para construir el modelo de datos para este contexto. No puede ser nulo.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContexto).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=BDGestionVentas.mssql.somee.com;Initial Catalog=BDGestionVentas;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");
        }

    }
}
