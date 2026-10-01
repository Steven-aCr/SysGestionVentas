using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components;

namespace SysGestionVentas.BlazorClient.Authentication;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly ITokenStorage _tokenStorage;
    private readonly JwtAuthenticationStateProvider _authProvider;
    private readonly NavigationManager _navigation;

    public JwtAuthorizationHandler(
        ITokenStorage tokenStorage,
        JwtAuthenticationStateProvider authProvider,
        NavigationManager navigation)
    {
        _tokenStorage = tokenStorage;
        _authProvider = authProvider;
        _navigation = navigation;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenStorage.GetAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrWhiteSpace(token))
        {
            await _authProvider.MarkUserAsLoggedOutAsync();
            var returnUrl = Uri.EscapeDataString(_navigation.ToBaseRelativePath(_navigation.Uri));
            // AJUSTA: usa la ruta real de tu Login.razor
            _navigation.NavigateTo($"login?returnUrl={returnUrl}");
        }

        return response;
    }
}