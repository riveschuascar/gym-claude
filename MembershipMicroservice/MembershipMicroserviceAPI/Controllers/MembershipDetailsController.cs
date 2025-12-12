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

            var email = GetEmailFromClaims();
            var result = await _detailService.AddDiscipline(request.MembershipId, request.DisciplineId, email);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        [HttpDelete("{membershipId:int}/{disciplineId:int}")]
        public async Task<IActionResult> Remove(int membershipId, int disciplineId)
        {
            var email = GetEmailFromClaims();
            var result = await _detailService.RemoveDiscipline(membershipId, disciplineId, email);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return NoContent();
        }
    }
}
