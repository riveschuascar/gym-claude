using Orchestrator.Application.Models;

namespace Orchestrator.Application.Interfaces
{
    public interface IOrchestatorService
    {
        Task StartSagaAsync(SaleCreatedEvent @event);
    }
}