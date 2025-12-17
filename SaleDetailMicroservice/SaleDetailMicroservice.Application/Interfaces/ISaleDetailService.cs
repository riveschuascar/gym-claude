using SaleDetailMicroservice.Domain.Entities;
using SaleDetailMicroservice.Domain.Shared;

namespace SaleDetailMicroservice.Application.Interfaces
{
    public interface ISaleDetailService
    {
        Task<Result<IEnumerable<SaleDetail>>> GetAll();
        Task<Result<IEnumerable<SaleDetail>>> GetBySaleId(int saleId);
        Task<Result> CreateDetails(List<SaleDetail> details);
    }
}
