using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebUI.DTO;

namespace WebUI.Pages.Login
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _loginClient;

        [BindProperty]
        public LoginInput Credentials { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _loginClient = httpClientFactory.CreateClient("LoginAPI");
        }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToPage("/Index");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Revisa los datos ingresados.";
                return Page();
            }

            try
            {
                var response = await _loginClient.PostAsJsonAsync("/api/login", new
                {
                    email = Credentials.Email,
                    password = Credentials.Password
                });

                if (!response.IsSuccessStatusCode)
                {
                    ErrorMessage = "Credenciales invalidas o usuario inactivo.";
                    return Page();
                }

                var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
                if (auth?.User == null || string.IsNullOrWhiteSpace(auth.Token))
                {
                    ErrorMessage = "No se pudo generar el token de acceso.";
                    return Page();
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, auth.User.Id.ToString()),
                    new Claim(ClaimTypes.Name, auth.User.Name),
                    new Claim(ClaimTypes.Email, auth.User.Email),
                    new Claim(ClaimTypes.Role, string.IsNullOrWhiteSpace(auth.User.Role) ? "User" : auth.User.Role),
                    new Claim("token", auth.Token)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                var expiresUtc = auth.ExpiresAt == default
                    ? DateTimeOffset.UtcNow.AddHours(1)
                    : new DateTimeOffset(DateTime.SpecifyKind(auth.ExpiresAt, DateTimeKind.Utc));

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = expiresUtc
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                return RedirectToPage("/Index");
            }
            catch (HttpRequestException)
            {
                ErrorMessage = "No se pudo conectar con el microservicio de login.";
                return Page();
            }
            catch (Exception)
            {
                ErrorMessage = "Ocurrio un error al iniciar sesion.";
                return Page();
            }
        }
    }

    public class LoginInput
    {
        [Required(ErrorMessage = "El email es requerido.")]
        [EmailAddress(ErrorMessage = "Formato de email invalido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contrasena es requerida.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
