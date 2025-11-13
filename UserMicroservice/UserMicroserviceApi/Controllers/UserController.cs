using UserMicroservice.Application.Services;
using UserMicroservice.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserMicroservice.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly UserService service;

        public ClienteController(UserService service)
        {
            this.service = service;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Insert([FromBody]User t)
        { 
           var res=await service.Create(t);
           return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> Select()
        { 
          var res = await service.GetAll();
            return Ok(res);
        }
    }
}