using SalesMicroserviceApplication.Interfaces;
using SalesMicroserviceDomain.Entities;
using SalesMicroserviceDomain.Ports;
using SalesMicroserviceDomain.Shared;
using SalesMicroserviceDomain.Validators;

namespace SalesMicroserviceApplication.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _repo;

        public SaleService(ISaleRepository repo)
        {
            _repo = repo;
        }

        public async Task<Result<IEnumerable<Sale>>> GetAll() => await _repo.GetAll();

        public async Task<Result<Sale>> GetById(int id) => await _repo.GetById(id);

        public async Task<Result<Sale>> Create(Sale sale, string? userEmail = null)
        {
            var validation = SaleValidators.ValidateForCreate(sale);
            if (validation.IsFailure)
                return Result<Sale>.Failure(validation.Error!);

            sale.SaleDate = sale.SaleDate == default ? DateTime.UtcNow.Date : sale.SaleDate.Date;
            sale.StartDate = sale.SaleDate;
            sale.EndDate = sale.SaleDate;
            sale.CreatedAt = DateTime.UtcNow;
            sale.LastModification = DateTime.UtcNow;
            sale.IsActive = true;

            return await _repo.Create(sale, userEmail);
        }

        public async Task<Result<Sale>> Update(Sale sale, string? userEmail = null)
        {
            var validation = SaleValidators.ValidateForUpdate(sale);
            if (validation.IsFailure)
                return Result<Sale>.Failure(validation.Error!);

            sale.SaleDate = sale.SaleDate == default ? DateTime.UtcNow.Date : sale.SaleDate.Date;
            sale.StartDate = sale.SaleDate;
            sale.EndDate = sale.SaleDate;
            sale.LastModification = DateTime.UtcNow;
            return await _repo.Update(sale, userEmail);
        }

        public async Task<Result> Delete(int id, string? userEmail = null)
        {
            return await _repo.Delete(id, userEmail);
        }
    }
}
