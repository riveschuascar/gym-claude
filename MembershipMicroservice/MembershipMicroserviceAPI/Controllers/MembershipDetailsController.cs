using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MembershipMicroservice.MembershipMicroserviceApplication.Interfaces;
using MembershipMicroservice.MembershipMicroserviceDomain.Entities;
using System.Security.Claims;

namespace MembershipMicroservice.MembershipMicroserviceAPI.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin,Instructor")]
    [Route("api/[controller]")]
    public class MembershipDetailsController : ControllerBase
    {
        private readonly IMembershipDetailService _detailService;

        public MembershipDetailsController(IMembershipDetailService detailService)
        {
            _detailService = detailService;
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

        [HttpGet("{membershipId:int}")]
        [ProducesResponseType(typeof(IEnumerable<MembershipDiscipline>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByMembership(int membershipId)
        {
            var result = await _detailService.GetByMembership(membershipId);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        public class AddDetailRequest
        {
            public int MembershipId { get; set; }
            public int DisciplineId { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddDetailRequest request)
        {
            if (request == null)
                return BadRequest(new { error = "Datos requeridos." });

            var userId = GetUserIdFromClaims();
            var result = await _detailService.AddDiscipline(request.MembershipId, request.DisciplineId, userId);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpDelete("{membershipId:int}/{disciplineId:int}")]
        public async Task<IActionResult> Remove(int membershipId, int disciplineId)
        {
            var userId = GetUserIdFromClaims();
            var result = await _detailService.RemoveDiscipline(membershipId, disciplineId, userId);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return NoContent();
        }
    }
}
