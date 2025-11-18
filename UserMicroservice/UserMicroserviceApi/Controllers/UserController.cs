using UserMicroservice.Application.Services;
using UserMicroservice.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace UserMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService service;

        public UserController(UserService service)
        {
            this.service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> SelectById(int id)
        {
            var res = await service.GetById(id);
            
            if (!res.IsSuccess)
                return NotFound(new { error = res.Error });
            
            return Ok(res.Value);
        }

        [HttpGet]
        public async Task<IActionResult> Select()
        {
            var res = await service.GetAll();
            
            if (!res.IsSuccess)
                return BadRequest(new { error = res.Error });
            
            return Ok(res.Value);
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Insert([FromBody] User usr)
        {
            if (usr == null)
                return BadRequest(new { error = "El usuario no puede ser vacio" });

            var res = await service.Create(usr);
            
            if (!res.IsSuccess)
                return BadRequest(new { error = res.Error });
            
            return CreatedAtAction(nameof(SelectById), new { id = res.Value.Id }, res.Value);
        }

        [HttpPut("Modificar")]
        public async Task<IActionResult> Update([FromBody] User usr)
        {
            if (usr == null)
                return BadRequest(new { error = "El usuario no puede ser nulo" });

            var res = await service.Update(usr);
            
            if (!res.IsSuccess)
                return BadRequest(new { error = res.Error });
            
            return Ok(res.Value);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await service.Delete(id);
            
            if (!res.IsSuccess)
                return NotFound(new { error = res.Error });
            
            return NoContent(); // 204 No Content
        }
    }
}