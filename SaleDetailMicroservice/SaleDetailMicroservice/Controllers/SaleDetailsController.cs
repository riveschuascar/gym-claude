using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaleDetailMicroservice.Application.Interfaces;
using SaleDetailMicroservice.Domain.Entities;

namespace SaleDetailMicroservice.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SaleDetailsController : ControllerBase
    {
        private readonly ISaleDetailService _service;

        public SaleDetailsController(ISaleDetailService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAll();
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("sale/{saleId:int}")]
        public async Task<IActionResult> GetBySaleId(int saleId)
        {
            var result = await _service.GetBySaleId(saleId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRange([FromBody] List<SaleDetail> details)
        {
            var result = await _service.CreateDetails(details);
            return result.IsSuccess ? Ok() : BadRequest(new { error = result.Error });
        }
    }
}
