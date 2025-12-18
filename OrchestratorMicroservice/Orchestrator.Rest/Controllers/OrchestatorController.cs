using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Orchestrator.Application.Interfaces;
using Orchestrator.Application.Models;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrchestatorController : ControllerBase
{
    private readonly IOrchestatorService _orchestratorService;

    public OrchestatorController(IOrchestatorService orchestratorService)
    {
        _orchestratorService = orchestratorService;
    }

    [HttpPost("sale-created")]
    public async Task<IActionResult> OnSaleCreated([FromBody] SaleCreatedEvent @event)
    {
        await _orchestratorService.StartSagaAsync(@event);
        return Ok();
    }
}