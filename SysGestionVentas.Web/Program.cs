using BDGestionVentas.BL;
using Microsoft.EntityFrameworkCore;
using SysGestionVentas.BL;
using SysGestionVentas.DAL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbContexto>();
builder.Services.AddScoped<CategoryBL>();
builder.Services.AddScoped<ClientBL>();
builder.Services.AddScoped<SupplierBL>();
builder.Services.AddScoped<EmployeeBL>();
builder.Services.AddScoped<DepartmentBL>();
builder.Services.AddScoped<MovementTypeBL>();
builder.Services.AddScoped<DocumentTypeBL>();
builder.Services.AddScoped<ProductListBL>();


// ─── 2. Servicios MVC ────
builder.Services.AddControllersWithViews();

// ─── 3. Sesiones ───
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ─── 4. Pipeline ────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();