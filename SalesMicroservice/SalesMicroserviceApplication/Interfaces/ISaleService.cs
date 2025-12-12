using SalesMicroserviceDomain.Entities;
using SalesMicroserviceDomain.Shared;

namespace SalesMicroserviceApplication.Interfaces
{
    public interface ISaleService
    {
        Task<Result<IEnumerable<Sale>>> GetAll();
        Task<Result<Sale>> GetById(int id);
        Task<Result<Sale>> Create(Sale sale, string? userEmail = null, OperationContext? context = null);
        Task<Result<Sale>> Update(Sale sale, string? userEmail = null);
        Task<Result> Delete(int id, string? userEmail = null);
    }
}
