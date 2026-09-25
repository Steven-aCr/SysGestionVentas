using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using SysGestionVentas.ApiClient.Config;
using SysGestionVentas.ApiClient.Services.Implementaciones;
using SysGestionVentas.ApiClient.Services.Interfaces;
using SysGestionVentas.BlazorClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiConfig = new ApiConfig();
builder.Configuration.GetSection("ApiConfig").Bind(apiConfig);

if (apiConfig == null || string.IsNullOrWhiteSpace(apiConfig.BaseUrl))
{
    throw new InvalidOperationException(
        "No se encontró la configuración de la API.");
}

builder.Services.AddHttpClient<IProductoApiService, ProductoApiService>(client =>
    client.BaseAddress = new Uri(apiConfig.BaseUrl));

builder.Services.AddHttpClient<IInventarioApiService, InventarioApiService>(client =>
    client.BaseAddress = new Uri(apiConfig.BaseUrl));

builder.Services.AddHttpClient<ICategoriaApiService, CategoriaApiService>(client =>
    client.BaseAddress = new Uri(apiConfig.BaseUrl));

builder.Services.AddHttpClient<IMovimientoInventarioApiService, MovimientoInventarioApiService>(client =>
    client.BaseAddress = new Uri(apiConfig.BaseUrl));

await builder.Build().RunAsync();