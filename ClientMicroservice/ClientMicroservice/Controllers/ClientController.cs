using ClientMicroservice.Application.Services;
using ClientMicroservice.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClientMicroservice.Controllers;

[Authorize(Roles = "Admin,Instructor")]
[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly ClientService _service;
    
    public ClientController(ClientService service)
    {
        _service = service;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Client>>> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Client>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Client>> Create([FromBody] Client client)
    {
        try
        {
            var userId = GetUserIdFromClaims();
            var created = await _service.CreateAsync(client, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Client>> Update(int id, [FromBody] Client client)
    {
        if (id != client.Id) return BadRequest("ID de ruta y cuerpo no coinciden");
        try
        {
            var userId = GetUserIdFromClaims();
            var updated = await _service.UpdateAsync(client, userId);
            if (updated is null) return NotFound();
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetUserIdFromClaims();
        var ok = await _service.DeleteByIdAsync(id, userId);
        if (!ok) return NotFound();
        return NoContent();
    }
}
