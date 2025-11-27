using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DisciplineMicroservice.DisciplineMicroserviceApplication.Interfaces;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Entities;
using System.Security.Claims;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class DisciplinesController : ControllerBase
{
    private readonly IDisciplineService _disciplineService;

    public DisciplinesController(IDisciplineService disciplineService)
    {
        _disciplineService = disciplineService;
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
    public async Task<IActionResult> GetAll()
    {
        var result = await _disciplineService.GetAll();
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _disciplineService.GetById(id);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Discipline discipline)
    {
        var email = GetEmailFromClaims();
        var result = await _disciplineService.Create(discipline, email);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Discipline discipline)
    {
        discipline.Id = (short)id;
        var email = GetEmailFromClaims();
        var result = await _disciplineService.Update(discipline, email);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var email = GetEmailFromClaims();
        var result = await _disciplineService.Delete((short)id, email);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}