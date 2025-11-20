using LoginMicroservice.Application.Interfaces;
using LoginMicroservice.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace LoginMicroservice.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (request == null)
                return BadRequest(new { error = "Credentials are required." });

            var result = await _loginService.AuthenticateAsync(request);

            if (!result.IsSuccess)
                return Unauthorized(new { error = result.Error });

            return Ok(result.Value);
        }
    }
}
