using Microsoft.AspNetCore.Mvc;
using DisciplineMicroservice.DisciplineMicroserviceApplication.Interfaces;

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
}
