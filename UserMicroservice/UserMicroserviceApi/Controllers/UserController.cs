using System.Linq;
using System.Security.Claims;
using UserMicroservice.Application.Services;
using UserMicroservice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserMicroservice.Api.Services;

namespace UserMicroservice.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;
        private readonly IEmailClient _emailClient;

        public UserController(UserService service, IEmailClient emailClient)
        {
            _service = service;
            _emailClient = emailClient;
        }

        private string? GetEmailFromClaims()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            // Debug: Si no encuentra el email con ClaimTypes.Email, intenta con el nombre literal
            if (string.IsNullOrEmpty(email))
            {
                email = User.FindFirst("email")?.Value;
            }

            // Debug: Log de todos los claims
            var allClaims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            System.Diagnostics.Debug.WriteLine($"All claims: {string.Join(", ", allClaims)}");
            System.Diagnostics.Debug.WriteLine($"Email extracted: {email}");

            return email;
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetById(id);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAll();

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            if (user == null)
                return BadRequest(new { error = "El usuario no puede ser vacío" });

            var email = GetEmailFromClaims();
            var result = await _service.Create(user, email);

            if (!result.IsSuccess)
            {
                string errorMessage = result.Error;
                return BadRequest(new { error = errorMessage });
            }
            // Si la creación fue exitosa, enviar correo de credenciales vía gRPC (no bloquear fallo de negocio)
            try
            {
                var fullName = string.Join(" ", new[] { user.Name, user.FirstLastname, user.SecondLastname }.Where(s => !string.IsNullOrWhiteSpace(s)));
                await _emailClient.SendCredentialEmailAsync(
                    user.Email ?? string.Empty,
                    fullName,
                    "Registro de usuario",
                    "Por favor no comparta sus credenciales y cambie la contraseña al iniciar sesión.",
                    user.Email ?? string.Empty,
                    user.Password ?? string.Empty
                );
            }
            catch
            {
                // Ignorar errores de envío de correo para no romper el flujo de creación.
            }

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] User user)
        {
            if (user == null)
                return BadRequest(new { error = "El usuario no puede ser nulo" });

            var email = GetEmailFromClaims();
            var result = await _service.Update(user, email);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "Usuario actualizado exitosamente" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var email = GetEmailFromClaims();
            var result = await _service.Delete(id, email);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return NoContent();
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var result = await _service.GetByEmail(email);
            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPost("change-password/{id}")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordRequest request)
        {
            if (request == null)
                return BadRequest(new { error = "Datos de contraseña requeridos" });

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest(new { error = "Nueva contraseña requerida" });

            var email = GetEmailFromClaims();
            var result = await _service.UpdatePassword(id, request.NewPassword, email);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "Contraseña actualizada exitosamente" });
        }
    }

    public class ChangePasswordRequest
    {
        public string? NewPassword { get; set; }
    }
}