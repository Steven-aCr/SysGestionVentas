using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SysGestionVentas.BL;
using SysGestionVentas.EN.ViewModels;
using System.Security.Claims;

namespace SysGestionVentas.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]

    /// <summary>
    /// Controlador responsable de la autenticación de usuarios.
    /// Gestiona inicio de sesión, cierre de sesión y redirección
    /// post-login según el rol del usuario autenticado.
    /// </summary>
    public class AuthController : Controller
    {
        // ── GET: Auth/Login ───────────────────────────────────────

        /// <summary>
        /// Muestra el formulario de inicio de sesión.
        /// Redirige al dashboard correspondiente si el usuario
        /// ya tiene una sesión activa.
        /// </summary>
        /// <param name="returnUrl">URL opcional a la que redirigir tras autenticación exitosa.</param>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToRoleDashboard();

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginModel());
        }

        // ── POST: Auth/Login ──────────────────────────────────────

        /// <summary>
        /// Procesa las credenciales enviadas desde el formulario de inicio de sesión.
        /// Si la autenticación es exitosa, emite la cookie de sesión con los claims
        /// del usuario y redirige al dashboard correspondiente a su rol.
        /// </summary>
        /// <param name="pModel">ViewModel con email, contraseña y opción "recordarme".</param>
        /// <param name="returnUrl">URL opcional a la que redirigir tras autenticación exitosa.</param>
        /// <returns>
        /// Redirección al dashboard del rol en caso de éxito,
        /// o la vista de login con errores de validación en caso de fallo.
        /// </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel pModel, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(pModel);

            try
            {
                var user = await UserBL.LoginAsync(pModel.Email!, pModel.Password);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name,           user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.Email,          user.Email    ?? string.Empty),
                    new Claim(ClaimTypes.Role,           user.Rol?.Name ?? string.Empty),
                    new Claim("FullName",
                        user.Person?.FullName ?? user.UserName ?? string.Empty),
                    // PersonId en claim para que los controladores de Cliente
                    // puedan filtrar sus propios registros sin ir a BD.
                    new Claim("PersonId", user.PersonId.ToString())
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var authProps = new AuthenticationProperties
                {
                    IsPersistent = pModel.RememberMe,
                    ExpiresUtc = pModel.RememberMe
                        ? DateTimeOffset.UtcNow.AddDays(7)
                        : DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProps);

                // ReturnUrl tiene prioridad sobre el dashboard por rol.
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToRoleDashboard();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(pModel);
            }
        }

        // ── POST: Auth/Logout ─────────────────────────────────────

        /// <summary>
        /// Cierra la sesión activa del usuario autenticado
        /// y redirige al formulario de Login.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Sesión cerrada correctamente.";
            return RedirectToAction(nameof(Login));
        }

        // ── GET: Auth/AccesoDenegado ──────────────────────────────

        /// <summary>
        /// Muestra la página de acceso denegado cuando un usuario autenticado
        /// intenta acceder a un recurso para el que no tiene permisos.
        /// </summary>
        [HttpGet]
        public IActionResult AccesoDenegado() => View("Home");

        // ── Métodos privados ──────────────────────────────────────

        /// <summary>
        /// Redirige al usuario al dashboard correspondiente a su rol activo.
        /// Si el rol no coincide con ninguno registrado, redirige al Home genérico.
        /// </summary>
        /// <returns>Resultado de redirección según el claim de rol.</returns>
        private IActionResult RedirectToRoleDashboard()
        {
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;
            return rol switch
            {
                "Administrador" => RedirectToAction("Index", "Home"),
                "Vendedor" => RedirectToAction("Dashboard", "Ventas"),
                "Cliente" => RedirectToAction("Portal", "ClientePortal"),
                _ => RedirectToAction("Index", "Home")
            };
        }
    }
}