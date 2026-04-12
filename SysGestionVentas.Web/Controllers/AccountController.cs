using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.BL;
using SysGestionVentas.EN;
using SysGestionVentas.EN.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]

    /// <summary>
    /// Controlador responsable de la gestión del perfil del usuario autenticado.
    /// Permite editar datos personales, cambiar contraseña y cerrar sesión.
    /// Requiere autenticación en todas sus acciones.
    /// </summary>
    [Authorize]
    public class AccountController : Controller
    {
        // GET: Account/Profile
        /// <summary>
        /// Muestra el formulario de edición del perfil del usuario autenticado.
        /// Carga los datos actuales de <see cref="User"/> y su <see cref="Person"/> asociada.
        /// </summary>
        public async Task<IActionResult> Profile()
        {
            try
            {
                int userId = ObtenerUserIdActual();
                var user = await UserBL.ObtenerPorIdAsync(new User { UserId = userId });
                if (user == null)
                    return NotFound();

                var model = new EditProfileModel
                {
                    UserId = user.UserId,
                    PersonId = user.PersonId,
                    Email = user.Email,
                    FirstName = user.Person?.FirstName ?? string.Empty,
                    LastName = user.Person?.LastName ?? string.Empty,
                    Adress = user.Person?.Adress ?? string.Empty,
                    PhoneNumber = user.Person?.PhoneNumber ?? string.Empty,
                    Dui = user.Person?.Dui
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Account/Profile
        /// <summary>
        /// Procesa la actualización del perfil del usuario autenticado.
        /// Si <c>NewPassword</c> tiene valor, valida la contraseña actual antes de cambiarla.
        /// Si <c>NewPassword</c> está vacío, solo actualiza datos personales y correo.
        /// </summary>
        /// <param name="pModel">ViewModel con los datos capturados desde el formulario.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(EditProfileModel pModel)
        {
            // Si no se desea cambiar contraseña, ignorar esos campos en la validación
            if (string.IsNullOrWhiteSpace(pModel.NewPassword))
            {
                ModelState.Remove(nameof(EditProfileModel.CurrentPassword));
                ModelState.Remove(nameof(EditProfileModel.NewPassword));
                ModelState.Remove(nameof(EditProfileModel.ConfirmNewPassword));
            }
            else
            {
                if (string.IsNullOrWhiteSpace(pModel.CurrentPassword))
                    ModelState.AddModelError(
                        nameof(EditProfileModel.CurrentPassword),
                        "Debe ingresar su contraseña actual para cambiarla.");
            }

            if (!ModelState.IsValid)
                return View(pModel);

            try
            {
                await UserBL.ActualizarPerfilAsync(pModel);
                TempData["Success"] = "Perfil actualizado correctamente.";
                return RedirectToAction(nameof(Profile));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(pModel);
            }
        }

        // POST: Account/Logout
        /// <summary>
        /// Cierra la sesión activa del usuario autenticado y redirige al Login.
        /// Se expone aquí para que el botón de cerrar sesión del perfil
        /// tenga un endpoint propio sin depender del <c>AuthController</c>.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Success"] = "Sesión cerrada correctamente.";
            return RedirectToAction("Login", "Auth");
        }

        // ── Métodos Privados ──────────────────────────────────────────────────────

        /// <summary>
        /// Obtiene el <c>UserId</c> del usuario autenticado desde los claims de la sesión.
        /// </summary>
        /// <returns>Identificador del usuario autenticado.</returns>
        /// <exception cref="Exception">Se lanza si el claim no existe o no es válido.</exception>
        private int ObtenerUserIdActual()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out int userId) || userId <= 0)
                throw new Exception("No se pudo identificar al usuario autenticado.");
            return userId;
        }
    }
}