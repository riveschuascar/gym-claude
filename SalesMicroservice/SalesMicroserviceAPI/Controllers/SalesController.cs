using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesMicroserviceApplication.Interfaces;
using SalesMicroserviceDomain.Entities;
using System.Security.Claims;
using SalesMicroserviceDomain.Shared;

namespace SalesMicroserviceAPI.Controllers
{
    [ApiController]
    //[Authorize(Roles = "Admin,Instructor")]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SalesController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        private string? GetUserIdFromClaims()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirst("sub")?.Value;
            }
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirst(ClaimTypes.Email)?.Value;
            }
            return userId;
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
            var userId = GetUserIdFromClaims();
            var context = BuildOperationContext();
            var result = await _saleService.Create(sale, userId, context);
            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Sale sale)
        {
            sale.Id = id;
            var userId = GetUserIdFromClaims();
            var result = await _saleService.Update(sale, userId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserIdFromClaims();
            var result = await _saleService.Delete(id, userId);
            return result.IsSuccess ? NoContent() : NotFound(result.Error);
        }

        [HttpPost("status/{id:int}")]
        public async Task<IActionResult> UpdateSaleStatus(int id, [FromBody] string status)
        {
            var result = await _saleService.UpdateSaleStatus(id, status);
            return result.IsSuccess ? Ok() : BadRequest(result.Error);
        }
    }
}
