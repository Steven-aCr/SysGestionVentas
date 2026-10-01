using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using SysGestionVentas.BlazorClient.Authentication;
using SysGestionVentas.ApiClient.Config;
using SysGestionVentas.ApiClient.Extensions;
using SysGestionVentas.BlazorClient;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// --- Autenticación (Fase 6) ---
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<ITokenStorage, TokenStorage>();
builder.Services.AddSingleton<JwtAuthenticationStateProvider>();
builder.Services.AddSingleton<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddTransient<JwtAuthorizationHandler>();

var apiConfig = new ApiConfig();
builder.Configuration.GetSection("ApiConfig").Bind(apiConfig);

builder.Services.AddApiClient(apiConfig);

await builder.Build().RunAsync();