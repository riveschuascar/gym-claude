using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesMicroserviceApplication.Interfaces;
using SalesMicroserviceDomain.Entities;
using System.Security.Claims;
using SalesMicroserviceDomain.Shared;

namespace SalesMicroserviceAPI.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Instructor")]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        private string? GetEmailFromClaims()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                email = User.FindFirst("email")?.Value;
            }

            var allClaims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            System.Diagnostics.Debug.WriteLine($"All claims: {string.Join(", ", allClaims)}");
            System.Diagnostics.Debug.WriteLine($"Email extracted: {email}");

            return email;
        }

        private OperationContext BuildOperationContext()
        {
            var correlationId = Request.Headers["X-Correlation-Id"].FirstOrDefault();
            var operationId = Request.Headers["X-Operation-Id"].FirstOrDefault();

            var context = new OperationContext(correlationId, operationId);

            if (string.IsNullOrWhiteSpace(correlationId))
                Request.Headers["X-Correlation-Id"] = context.CorrelationId;
            if (string.IsNullOrWhiteSpace(operationId))
                Request.Headers["X-Operation-Id"] = context.OperationId;

            Response.Headers["X-Correlation-Id"] = context.CorrelationId;
            Response.Headers["X-Operation-Id"] = context.OperationId;

            return context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _saleService.GetAll();
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _saleService.GetById(id);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Sale sale)
        {
            var email = GetEmailFromClaims();
            var context = BuildOperationContext();
            var result = await _saleService.Create(sale, email, context);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Sale sale)
        {
            sale.Id = id;
            var email = GetEmailFromClaims();
            var result = await _saleService.Update(sale, email);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var email = GetEmailFromClaims();
            var result = await _saleService.Delete(id, email);
            return result.IsSuccess ? NoContent() : NotFound(result.Error);
        }
    }
}
