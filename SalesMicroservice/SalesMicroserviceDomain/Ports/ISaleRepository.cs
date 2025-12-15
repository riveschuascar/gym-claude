using SalesMicroserviceDomain.Entities;
using SalesMicroserviceDomain.Shared;

namespace SalesMicroserviceDomain.Ports
{
    public interface ISaleRepository
    {
        Task<Result<IEnumerable<Sale>>> GetAll();
        Task<Result<Sale>> GetById(int id);
        Task<Result<Sale>> Create(Sale sale, string? userEmail = null);
        Task<Result<Sale>> Update(Sale sale, string? userEmail = null);
        Task<Result> Delete(int id, string? userEmail = null);
        Task<Result> UpdateSaleStatus(int Id, string Status);
    }
}
