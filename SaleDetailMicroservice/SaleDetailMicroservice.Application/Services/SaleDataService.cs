using SaleDetailMicroservice.Application.Interfaces;
using SaleDetailMicroservice.Domain.Entities;
using SaleDetailMicroservice.Domain.Ports;
using SaleDetailMicroservice.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleDetailMicroservice.Application.Services
{
    public class SaleDetailService : ISaleDetailService
    {
        private readonly ISaleDetailRepository _repo;

        public SaleDetailService(ISaleDetailRepository repo)
        {
            _repo = repo;
        }

        public async Task<Result<IEnumerable<SaleDetail>>> GetAll()
        {
            return await _repo.GetAll();
        }

        public async Task<Result<IEnumerable<SaleDetail>>> GetBySaleId(int saleId)
        {
            return await _repo.GetBySaleId(saleId);
        }

        public async Task<Result> CreateDetails(List<SaleDetail> details)
        {
            if (details == null || !details.Any())
                return Result.Failure("La lista de detalles no puede estar vacía.");

            foreach (var d in details)
            {
                if (d.SaleId <= 0) return Result.Failure("ID de venta inválido en los detalles.");
                if (d.Qty <= 0) return Result.Failure("La cantidad debe ser mayor a 0.");

                // Calculo de seguridad por si viene mal del front
                if (d.Total <= 0) d.Total = d.Qty * d.Price;
            }

            return await _repo.CreateRange(details);
        }
    }
}
