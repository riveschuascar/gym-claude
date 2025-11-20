using Microsoft.AspNetCore.Mvc;
using DisciplineMicroservice.DisciplineMicroserviceApplication.Interfaces;
using DisciplineMicroservice.DisciplineMicroserviceDomain.Entities;


[ApiController]
[Route("api/[controller]")]
public class DisciplinesController : ControllerBase
{
    private readonly IDisciplineService _disciplineService;

    public DisciplinesController(IDisciplineService disciplineService)
    {
        _disciplineService = disciplineService;
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
        var result = await _disciplineService.Create(discipline);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Discipline discipline)
    {
        discipline.Id = (short)id; // conversión correcta
        var result = await _disciplineService.Update(discipline);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _disciplineService.Delete((short)id);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}