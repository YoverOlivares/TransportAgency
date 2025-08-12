using Microsoft.EntityFrameworkCore;
using TransportAgency.Data.Context;
using TransportAgency.Data.Repositories.Interfaces;
using TransportAgency.Models.Entities;

namespace TransportAgency.Data.Repositories.Implementations
{
    public class SaleRepository : GenericRepository<Sale>, ISaleRepository
    {
        public SaleRepository(TransportAgencyContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Sale>> GetSalesByDateAsync(DateTime date)
        {
            try
            {
                var startDate = date.Date;
                var endDate = startDate.AddDays(1);

                return await _dbSet
                    .Include(s => s.Customer)
                    .Include(s => s.Seat)
                    .ThenInclude(seat => seat.Trip)
                    .ThenInclude(trip => trip.Bus)
                    .Include(s => s.Seat.Trip.Route)
                    .Where(s => s.SaleDate >= startDate && s.SaleDate < endDate)
                    .OrderByDescending(s => s.SaleDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ventas por fecha {date:dd/MM/yyyy}: {ex.Message}", ex);
            }
        }

        public async Task<Sale?> GetSaleByReceiptNumberAsync(string receiptNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(receiptNumber))
                    throw new ArgumentException("El número de recibo no puede estar vacío", nameof(receiptNumber));

                return await _dbSet
                    .Include(s => s.Customer)
                    .Include(s => s.Seat)
                    .ThenInclude(seat => seat.Trip)
                    .ThenInclude(trip => trip.Bus)
                    .Include(s => s.Seat.Trip.Route)
                    .FirstOrDefaultAsync(s => s.ReceiptNumber == receiptNumber);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la venta con recibo {receiptNumber}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Sale>> GetSalesWithFullInfoAsync()
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Customer)
                    .Include(s => s.Seat)
                    .ThenInclude(seat => seat.Trip)
                    .ThenInclude(trip => trip.Bus)
                    .Include(s => s.Seat.Trip.Route)
                    .OrderByDescending(s => s.SaleDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ventas con información completa: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var start = startDate.Date;
                var end = endDate.Date.AddDays(1);

                return await _dbSet
                    .Include(s => s.Customer)
                    .Include(s => s.Seat)
                    .ThenInclude(seat => seat.Trip)
                    .ThenInclude(trip => trip.Bus)
                    .Include(s => s.Seat.Trip.Route)
                    .Where(s => s.SaleDate >= start && s.SaleDate < end)
                    .OrderByDescending(s => s.SaleDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ventas entre {startDate:dd/MM/yyyy} y {endDate:dd/MM/yyyy}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Sale>> GetSalesByCustomerAsync(int customerId)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Customer)
                    .Include(s => s.Seat)
                    .ThenInclude(seat => seat.Trip)
                    .ThenInclude(trip => trip.Bus)
                    .Include(s => s.Seat.Trip.Route)
                    .Where(s => s.CustomerId == customerId)
                    .OrderByDescending(s => s.SaleDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ventas del cliente {customerId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Sale>> GetSalesByCustomerDocumentAsync(string documentNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(documentNumber))
                    throw new ArgumentException("El número de documento no puede estar vacío", nameof(documentNumber));

                return await _dbSet
                    .Include(s => s.Customer)
                    .Include(s => s.Seat)
                    .ThenInclude(seat => seat.Trip)
                    .ThenInclude(trip => trip.Bus)
                    .Include(s => s.Seat.Trip.Route)
                    .Where(s => s.Customer.DocumentNumber == documentNumber)
                    .OrderByDescending(s => s.SaleDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ventas del documento {documentNumber}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Sale>> GetRecentSalesAsync(int count = 10)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Customer)
                    .Include(s => s.Seat)
                    .ThenInclude(seat => seat.Trip)
                    .ThenInclude(trip => trip.Bus)
                    .Include(s => s.Seat.Trip.Route)
                    .OrderByDescending(s => s.SaleDate)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener las {count} ventas más recientes: {ex.Message}", ex);
            }
        }

        public async Task<decimal> GetTotalRevenueByDateAsync(DateTime date)
        {
            try
            {
                var startDate = date.Date;
                var endDate = startDate.AddDays(1);

                return await _dbSet
                    .Where(s => s.SaleDate >= startDate && s.SaleDate < endDate)
                    .SumAsync(s => s.Amount);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al calcular ingresos del {date:dd/MM/yyyy}: {ex.Message}", ex);
            }
        }

        public async Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var start = startDate.Date;
                var end = endDate.Date.AddDays(1);

                return await _dbSet
                    .Where(s => s.SaleDate >= start && s.SaleDate < end)
                    .SumAsync(s => s.Amount);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al calcular ingresos entre {startDate:dd/MM/yyyy} y {endDate:dd/MM/yyyy}: {ex.Message}", ex);
            }
        }

        public async Task<int> CountSalesByDateAsync(DateTime date)
        {
            try
            {
                var startDate = date.Date;
                var endDate = startDate.AddDays(1);

                return await _dbSet
                    .CountAsync(s => s.SaleDate >= startDate && s.SaleDate < endDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al contar ventas del {date:dd/MM/yyyy}: {ex.Message}", ex);
            }
        }

        public async Task<bool> ReceiptNumberExistsAsync(string receiptNumber, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(receiptNumber))
                    return false;

                var query = _dbSet.Where(s => s.ReceiptNumber == receiptNumber);

                if (excludeId.HasValue)
                {
                    query = query.Where(s => s.Id != excludeId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar la existencia del recibo {receiptNumber}: {ex.Message}", ex);
            }
        }

        public async Task<string> GenerateUniqueReceiptNumberAsync()
        {
            try
            {
                string receiptNumber;
                bool exists;
                int attempts = 0;
                const int maxAttempts = 100;

                do
                {
                    if (attempts >= maxAttempts)
                        throw new InvalidOperationException("No se pudo generar un número de recibo único después de múltiples intentos");

                    // Formato: REC-YYYYMMDD-HHMMSS-XXX
                    var now = DateTime.Now;
                    var random = new Random();
                    var randomSuffix = random.Next(100, 999);

                    receiptNumber = $"REC-{now:yyyyMMdd}-{now:HHmmss}-{randomSuffix}";

                    exists = await ReceiptNumberExistsAsync(receiptNumber);
                    attempts++;

                } while (exists);

                return receiptNumber;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar número de recibo único: {ex.Message}", ex);
            }
        }

        public override async Task<IEnumerable<Sale>> GetAllAsync()
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Customer)
                    .Include(s => s.Seat)
                    .ThenInclude(seat => seat.Trip)
                    .ThenInclude(trip => trip.Bus)
                    .Include(s => s.Seat.Trip.Route)
                    .OrderByDescending(s => s.SaleDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todas las ventas: {ex.Message}", ex);
            }
        }

        public override async Task<Sale?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Customer)
                    .Include(s => s.Seat)
                    .ThenInclude(seat => seat.Trip)
                    .ThenInclude(trip => trip.Bus)
                    .Include(s => s.Seat.Trip.Route)
                    .FirstOrDefaultAsync(s => s.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la venta con ID {id}: {ex.Message}", ex);
            }
        }
    }
}