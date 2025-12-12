using SalesMicroserviceDomain.Entities;
using SalesMicroserviceDomain.Shared;

namespace SalesMicroserviceDomain.Validators
{
    public static class SaleValidators
    {
        public static Result<Sale> ValidateForCreate(Sale sale)
        {
            if (sale == null)
                return Result<Sale>.Failure("La venta no puede ser nula.");

            if (sale.ClientId <= 0)
                return Result<Sale>.Failure("El cliente es obligatorio.");

            if (sale.MembershipId <= 0)
                return Result<Sale>.Failure("La membresia es obligatoria.");

            if (sale.TotalAmount <= 0)
                return Result<Sale>.Failure("El monto debe ser mayor a cero.");

            if (sale.EndDate < sale.StartDate)
                return Result<Sale>.Failure("La fecha de fin no puede ser anterior al inicio.");

            if (string.IsNullOrWhiteSpace(sale.PaymentMethod))
                return Result<Sale>.Failure("El metodo de pago es obligatorio.");

            if (!string.IsNullOrWhiteSpace(sale.TaxId) && sale.TaxId.Length > 50)
                return Result<Sale>.Failure("El NIT/CI no puede exceder 50 caracteres.");

            if (!string.IsNullOrWhiteSpace(sale.BusinessName) && sale.BusinessName.Length > 200)
                return Result<Sale>.Failure("La razón social no puede exceder 200 caracteres.");

            return Result<Sale>.Success(sale);
        }

        public static Result<Sale> ValidateForUpdate(Sale sale)
        {
            if (sale == null)
                return Result<Sale>.Failure("La venta no puede ser nula.");

            if ((sale.Id ?? 0) <= 0)
                return Result<Sale>.Failure("El Id de la venta es obligatorio.");

            return ValidateForCreate(sale);
        }
    }
}
