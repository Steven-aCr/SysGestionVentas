using System;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN.ViewModels
{
    /// <summary>
    /// VewModel utilizado para capturar credenciales de Usuarios
    /// durante el proceso de autenticación. 
    /// </summary>
    public class LoginModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress (ErrorMessage = "Ingrese un correo electrónico válido.")]
        [Display(Name = "Correo Electrónico")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; } = false;
    }
}
