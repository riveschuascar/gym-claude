using Microsoft.AspNetCore.Mvc;
using MembershipMicroservice.MembershipMicroserviceApplication.Interfaces;

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
}