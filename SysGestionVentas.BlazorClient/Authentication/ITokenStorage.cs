namespace SysGestionVentas.BlazorClient.Authentication;

public interface ITokenStorage
{
    Task SaveAsync(string token);
    Task<string?> GetAsync();
    Task RemoveAsync();
}