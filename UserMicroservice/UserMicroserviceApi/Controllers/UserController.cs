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
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> Select()
        {
            var res = await service.GetAll();
            return Ok(res);
        }

        [HttpPost("Crear")]
        public async Task<IActionResult> Insert([FromBody] User usr)
        {
            var res = await service.Create(usr);
            return Ok(res);
        }

        [HttpPut("Modificar")]
        public async Task<IActionResult> Update([FromBody] User usr)
        {
            var res = await service.Update(usr);
            return Ok(res);
        }

        [HttpDelete("Eliminar/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var res = await service.Delete(id);
            return Ok(res);
        }
    }
}