using Microsoft.AspNetCore.Mvc;
using MembershipMicroservice.MembershipMicroserviceApplication.Interfaces;
using MembershipMicroservice.MembershipMicroserviceDomain.Entities;

namespace MembershipMicroservice.MembershipMicroserviceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembershipsController : ControllerBase
    {
        private readonly IMembershipService _membershipService;

        public MembershipsController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
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
        var result = await _membershipService.Create(membership);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Membership membership)
    {
        membership.Id = (short)id;
        var result = await _membershipService.Update(membership);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _membershipService.Delete(id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
