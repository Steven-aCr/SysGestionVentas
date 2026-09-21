using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.DAL;

// Clases necesarias para consumir la API REST desarrollada en Java.
using SysGestionVentas.ApiClient.Config;
using SysGestionVentas.ApiClient.Services.Implementaciones;
using SysGestionVentas.ApiClient.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);


// ============================================================
// AUTENTICACIÓN
// Configura la autenticación mediante cookies.
// Se establecen las rutas utilizadas para iniciar sesión,
// cerrar sesión y manejar accesos no autorizados.
// ============================================================

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccesoDenegado";
    });


// ============================================================
// AUTORIZACIÓN
// Define las políticas de acceso según los roles existentes
// dentro del sistema.
// ============================================================

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        "SoloAdministrador",
        policy => policy.RequireRole("Administrador"));

    options.AddPolicy(
        "AdministradorOVendedor",
        policy => policy.RequireRole("Administrador", "Vendedor"));

    options.AddPolicy(
        "SoloCliente",
        policy => policy.RequireRole("Cliente"));
});


// ============================================================
// MVC
// Registra los controladores y las vistas utilizadas por
// la aplicación web.
// ============================================================

builder.Services.AddControllersWithViews();


// ============================================================
// SESIÓN
// Mantiene información temporal del usuario durante 30 minutos.
// La cookie se configura como HttpOnly para evitar que pueda
// ser accedida directamente mediante JavaScript.
// ============================================================

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// Permite acceder al contexto HTTP desde otros servicios.
builder.Services.AddHttpContextAccessor();


// ============================================================
// CONEXIÓN DIRECTA A MYSQL
// Configuración utilizada por los módulos existentes que todavía
// trabajan mediante Entity Framework y la capa DAL.
//
// Durante la migración hacia la API Java esta conexión seguirá
// disponible para los módulos que aún no hayan sido migrados.
// ============================================================

// Obtiene la cadena de conexión definida en appsettings.json.
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

// Registra DbContexto utilizando MySQL mediante Pomelo.
builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    )
);


// ============================================================
// CONFIGURACIÓN DE LA API REST EN JAVA
// A partir de esta sección se configura la comunicación entre
// el sistema C# y la API de inventario desarrollada en Java.
//
// La dirección base de la API se obtiene desde appsettings.json
// mediante la sección "ApiConfig".
// ============================================================

builder.Services.Configure<ApiConfig>(
    builder.Configuration.GetSection("ApiConfig"));

var apiConfig = builder.Configuration
    .GetSection("ApiConfig")
    .Get<ApiConfig>();


// Se valida la configuración antes de iniciar la aplicación.
// Esto permite detectar inmediatamente si BaseUrl no fue definida.
if (apiConfig == null || string.IsNullOrWhiteSpace(apiConfig.BaseUrl))
{
    throw new InvalidOperationException(
        "No se encontró la configuración de la API.");
}


// ============================================================
// SERVICIO DE MOVIMIENTOS DE INVENTARIO
// Registra el HttpClient encargado de comunicarse con los
// endpoints de MovimientoInventario de la API Java.
//
// La lógica de actualización del stock permanece en Java.
// C# únicamente envía las solicitudes y procesa las respuestas.
// ============================================================

builder.Services.AddHttpClient<
    IMovimientoInventarioApiService,
    MovimientoInventarioApiService>(client =>
    {
        client.BaseAddress = new Uri(apiConfig.BaseUrl);
    });


// ============================================================
// CONSTRUCCIÓN DE LA APLICACIÓN
// A partir de este punto se configura el pipeline HTTP.
// ============================================================

var app = builder.Build();


// ============================================================
// MANEJO DE ERRORES EN PRODUCCIÓN
// En ambientes diferentes a Development se utiliza una página
// centralizada para errores y se habilita HSTS.
// ============================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// ============================================================
// MIDDLEWARES
// El orden es importante, especialmente para Session,
// Authentication y Authorization.
// ============================================================

// Redirige las solicitudes HTTP hacia HTTPS.
app.UseHttpsRedirection();

// Permite servir archivos estáticos como CSS, JS e imágenes.
app.UseStaticFiles();

// Habilita el sistema de enrutamiento.
app.UseRouting();

// Habilita el manejo de sesiones.
app.UseSession();

// Identifica al usuario autenticado.
app.UseAuthentication();

// Comprueba los permisos y roles del usuario.
app.UseAuthorization();


// ============================================================
// RUTA MVC PREDETERMINADA
// Al iniciar la aplicación se dirige al Login del AuthController.
// ============================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");


// ============================================================
// INICIO DE LA APLICACIÓN
// ============================================================

app.Run();