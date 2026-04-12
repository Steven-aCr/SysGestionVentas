using System;
using System.ComponentModel.DataAnnotations;

namespace SysGestionVentas.EN.ViewModels
{
    /// <summary>
    ///  VewModel utilizado para capturar la información necesaria
    ///  para el proceso de cambio de contraseña de un usuario.
    /// </summary>
    public class ChangePasswordModel
    {
        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [StringLength(255, MinimumLength = 8, 
            ErrorMessage = "La contraseña debe tener un mínimo de 8 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword),
            ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
