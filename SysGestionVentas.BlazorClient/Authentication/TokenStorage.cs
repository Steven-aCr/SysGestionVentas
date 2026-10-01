using Microsoft.JSInterop;

namespace SysGestionVentas.BlazorClient.Authentication
{
    public class TokenStorage : ITokenStorage
    {
        private const  string Key = "authToken";
        private readonly IJSRuntime _js;

        public TokenStorage(IJSRuntime js)
        {
            _js = js;
        }

        public async Task SaveAsync(string token)
            => await _js.InvokeVoidAsync("localStorage.setItem", Key, token);

        public async Task<string> GetAsync( )
            => await _js.InvokeAsync<string?>("localStorage.getItem", Key);

        public async Task RemoveAsync( )
            => await _js.InvokeVoidAsync("localStorage.removeItem", Key);
    }
}
