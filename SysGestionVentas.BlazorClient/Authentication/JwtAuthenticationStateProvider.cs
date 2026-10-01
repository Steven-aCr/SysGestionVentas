using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace SysGestionVentas.BlazorClient.Authentication;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStorage _tokenStorage;

    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    public JwtAuthenticationStateProvider(ITokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenStorage.GetAsync();
        if (string.IsNullOrWhiteSpace(token))
            return Anonymous;

        try
        {
            var claims = JwtParser.ParseClaims(token).ToList();

            var exp = JwtParser.GetExpiration(claims);
            if (exp is not null && exp <= DateTimeOffset.UtcNow)
            {
                await _tokenStorage.RemoveAsync(); // token vencido
                return Anonymous;
            }

            var identity = new ClaimsIdentity(claims, "jwt", ClaimTypes.Name, ClaimTypes.Role);
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            await _tokenStorage.RemoveAsync(); // token malformado
            return Anonymous;
        }
    }

    public async Task MarkUserAsAuthenticatedAsync(string token)
    {
        await _tokenStorage.SaveAsync(token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await _tokenStorage.RemoveAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }
}