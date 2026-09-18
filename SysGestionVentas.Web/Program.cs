using Microsoft.AspNetCore.Authentication.Cookies;
using SysGestionVentas.DAL;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccesoDenegado";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloAdministrador", p => p.RequireRole("Administrador"));
    options.AddPolicy("AdministradorOVendedor", p => p.RequireRole("Administrador", "Vendedor"));
    options.AddPolicy("SoloCliente", p => p.RequireRole("Cliente"));
});

//builder.Services.AddDbContext<DbContexto>(); se repite, si da error, comentar la linea 37 y descomentar esta.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

// 1. Obtener la cadena de conexión desde appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registrar DbContacto o DbContexto con Pomelo MySql
builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString) // Detecta automáticamente la versión de tu servidor MySQL
    )
);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();