using Microsoft.Extensions.DependencyInjection;
using SysGestionVentas.ApiClient.Config;
using SysGestionVentas.ApiClient.Services.Implementaciones;
using SysGestionVentas.ApiClient.Services.Interfaces;

namespace SysGestionVentas.ApiClient.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiClient(
            this IServiceCollection services, ApiConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.BaseUrl))
                throw new InvalidOperationException(
                    "Falta ApiConfig:BaseUrl en wwwroot/appsettings.json.");

            var baseUri = new Uri(config.BaseUrl);

            services.AddHttpClient<IAuthApiService, AuthApiService>(c => c.BaseAddress = baseUri);
            services.AddHttpClient<IProductoApiService, ProductoApiService>(c => c.BaseAddress = baseUri);
            services.AddHttpClient<ICategoriaApiService, CategoriaApiService>(c => c.BaseAddress = baseUri);
            services.AddHttpClient<IInventarioApiService, InventarioApiService>(c => c.BaseAddress = baseUri);
            services.AddHttpClient<IMovimientoInventarioApiService, MovimientoInventarioApiService>(c => c.BaseAddress = baseUri);

            return services;
        }
    }
}