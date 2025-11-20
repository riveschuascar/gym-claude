using UserMicroservice.Application.Services;
using UserMicroservice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserMicroservice.Api.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _service;

        public UserController(UserService service)
        {
            _service = service;
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

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.Create(user);
            
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] User user)
        {
            if (user == null)
                return BadRequest(new { error = "El usuario no puede ser nulo" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.Update(user);
            
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });
            
            return Ok(new { message = "Usuario actualizado exitosamente" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.Delete(id);
            
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

            var result = await _service.UpdatePassword(id, request.NewPassword);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(new { message = "Contraseña actualizada exitosamente" });
        }
    }

    public class ChangePasswordRequest
    {
        public string NewPassword { get; set; }
    }
}
