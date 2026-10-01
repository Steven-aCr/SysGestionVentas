using System.Security.Claims;
using System.Text.Json;

namespace SysGestionVentas.BlazorClient.Authentication
{
    public class JwtParser
    {
        // AJUSTA: nombres donde Spring puede enviar el rol. Confirmar con el JWT real
        private static readonly string[] RoleKeys =
            { "role", "roles", "rol", "authorities", ClaimTypes.Role };

        // AJUSTA: claims cadidatos para el nombre del usuario, en orden de prioridad.
        private static readonly string[] NameKeys =
        { "unique_name", "name", "username", "nombre", "sub", "email" };

        public static IEnumerable<Claim> ParseClaims(string jwt)
        {
            var parts = jwt.Split('.');
            if (parts.Length != 3) return Enumerable.Empty<Claim>();

            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var json = Convert.FromBase64String(payload);
            using var doc = JsonDocument.Parse(json);
            var claims = new List<Claim>();

            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                var type = RoleKeys.Contains(prop.Name) ? ClaimTypes.Role : prop.Name;

                if (prop.Value.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in prop.Value.EnumerateArray())
                        claims.Add(new Claim(type, item.ToString()));
                }
                else
                {
                    claims.Add(new Claim(type, prop.Value.ToString()));
                }
            }

            var nameKey = NameKeys.FirstOrDefault(k => claims.Any(c => c.Type == k));
            if (nameKey != null)
            {
                var nameValue = claims.First(c => c.Type == nameKey).Value;
                claims.Add(new Claim(ClaimTypes.Name, nameValue));
            }

            return claims;

        }
        public static DateTimeOffset? GetExpiration(IEnumerable<Claim> claims)
        {
            var exp = claims.FirstOrDefault(c => c.Type == "exp")?.Value;
            return long.TryParse(exp, out var seconds)
                ? DateTimeOffset.FromUnixTimeSeconds(seconds)
                : null;
        }
    }
}
