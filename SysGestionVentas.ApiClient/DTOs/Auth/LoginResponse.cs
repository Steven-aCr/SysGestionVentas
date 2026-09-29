using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SysGestionVentas.ApiClient.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string TipoToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
