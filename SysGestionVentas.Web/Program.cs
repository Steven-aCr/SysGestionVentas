using Microsoft.AspNetCore.Authentication.Cookies;
using SysGestionVentas.DAL;

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

builder.Services.AddDbContext<DbContexto>();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

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