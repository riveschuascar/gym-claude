using SaleDetailMicroservice.Domain.Entities;
using SaleDetailMicroservice.Domain.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaleDetailMicroservice.Domain.Ports
{
    public interface ISaleDetailRepository
    {
        // Trae todos los detalles de una venta específica
        Task<Result<IEnumerable<SaleDetail>>> GetBySaleId(int saleId);

        // Trae absolutamente todos los detalles 
        Task<Result<IEnumerable<SaleDetail>>> GetAll();

        // Inserta una lista de detalles masivamente
        Task<Result> CreateRange(IEnumerable<SaleDetail> details);
    }
}