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
    public class MembershipsController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public MembershipsController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        private string? GetEmailFromClaims()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            // Debug: Si no encuentra el email con ClaimTypes.Email, intenta con el nombre literal
            if (string.IsNullOrEmpty(email))
            {
                email = User.FindFirst("email")?.Value;
            }

            // Debug: Log de todos los claims
            var allClaims = User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            System.Diagnostics.Debug.WriteLine($"All claims: {string.Join(", ", allClaims)}");
            System.Diagnostics.Debug.WriteLine($"Email extracted: {email}");

            return email;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Membership>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _membershipService.GetAll();
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _membershipService.GetById(id);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Membership membership)
        {
            var email = GetEmailFromClaims();
            var result = await _membershipService.Create(membership, email);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Membership membership)
        {
            membership.Id = (short)id;
            var email = GetEmailFromClaims();
            var result = await _membershipService.Update(membership, email);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var email = GetEmailFromClaims();
            var result = await _membershipService.Delete(id, email);
            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
        }
    }
}