using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebUI.Pages.Account
{
    [Authorize]
    public class ChangePasswordModel : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<ChangePasswordModel> _logger;

        public ChangePasswordModel(IHttpClientFactory httpFactory, ILogger<ChangePasswordModel> logger)
        {
            _httpFactory = httpFactory;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        public class InputModel
        {
            [Required(ErrorMessage = "La contraseña actual es requerida")]
            [DataType(DataType.Password)]
            public string? CurrentPassword { get; set; }

            [Required(ErrorMessage = "La nueva contraseña es requerida")]
            [StringLength(100, MinimumLength = 8, ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres")]
            [DataType(DataType.Password)]
            public string? NewPassword { get; set; }

            [Required(ErrorMessage = "Debe confirmar la nueva contraseña")]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
            public string? ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(idClaim))
            {
                _logger.LogWarning("No se encontró Claim NameIdentifier para el usuario");
                return Forbid();
            }

            var client = _httpFactory.CreateClient("Users");
            var payload = new { NewPassword = Input.NewPassword };

            try
            {
                var response = await client.PostAsJsonAsync($"/api/user/change-password/{idClaim}", payload);

                if (response.IsSuccessStatusCode)
                {
                    StatusMessage = "Contraseña actualizada exitosamente";
                    Input = new InputModel();
                    return RedirectToPage();
                }

                string errorText = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Error al cambiar contraseña: {Status} {Error}", response.StatusCode, errorText);
                ModelState.AddModelError(string.Empty, "No se pudo actualizar la contraseña: " + errorText);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al llamar al API de usuarios para cambiar contraseña");
                ModelState.AddModelError(string.Empty, "Ocurrió un error al cambiar la contraseña");
                return Page();
            }
        }
    }
}