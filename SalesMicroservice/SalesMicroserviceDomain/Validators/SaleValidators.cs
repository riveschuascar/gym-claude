using SalesMicroserviceDomain.Entities;
using SalesMicroserviceDomain.Shared;
using System.Linq;
using System;

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

            if (sale.TotalAmount <= 0)
                return Result<Sale>.Failure("El monto debe ser mayor a cero.");

            if (sale.Details == null || !sale.Details.Any())
                return Result<Sale>.Failure("La venta debe contener al menos un detalle con una disciplina.");

            foreach (var d in sale.Details)
            {
                if (d.DisciplineId <= 0)
                    return Result<Sale>.Failure("Cada detalle debe contener una disciplina válida.");
                if (d.Qty <= 0)
                    return Result<Sale>.Failure("La cantidad en los detalles debe ser mayor a cero.");
                if (d.Price <= 0)
                    return Result<Sale>.Failure("El precio en los detalles debe ser mayor a cero.");
                if (d.Total <= d.Price * d.Qty - 0.0001m) // small tolerance
                    return Result<Sale>.Failure("El total en los detalles debe ser igual a precio * cantidad.");

                if (d.StartDate.HasValue && d.EndDate.HasValue && d.EndDate < d.StartDate)
                    return Result<Sale>.Failure("La fecha de fin del detalle no puede ser anterior al inicio.");
            }

            if (!string.IsNullOrWhiteSpace(sale.Nit) && sale.Nit.Length > 50)
                return Result<Sale>.Failure("El NIT no puede exceder 50 caracteres.");

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
