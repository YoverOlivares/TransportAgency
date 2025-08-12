using TransportAgency.Business.Interfaces;
using TransportAgency.Data.Repositories.Interfaces;
using TransportAgency.Models.DTOs;
using TransportAgency.Models.Entities;
using TransportAgency.Models.ViewModels;
using System.Text.RegularExpressions;

namespace TransportAgency.Business.Services
{
    public class SaleService : ISaleService
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ISeatRepository _seatRepository;
        private readonly IGenericRepository<Customer> _customerRepository;

        public SaleService(
            ISaleRepository saleRepository,
            ISeatRepository seatRepository,
            IGenericRepository<Customer> customerRepository)
        {
            _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        public async Task<SaleDto> ProcessSaleAsync(CreateSaleDto saleDto)
        {
            try
            {
                // Validar datos de entrada
                var validationErrors = await ValidateSaleDataAsync(saleDto);
                if (validationErrors.Any())
                    throw new ArgumentException($"Errores de validación: {string.Join(", ", validationErrors)}");

                // Verificar disponibilidad del asiento
                var seat = await _seatRepository.GetByIdAsync(saleDto.SeatId);
                if (seat == null)
                    throw new ArgumentException($"No se encontró el asiento con ID {saleDto.SeatId}");

                if (seat.IsOccupied)
                    throw new InvalidOperationException($"El asiento {seat.SeatNumber} ya está ocupado");

                // Verificar que el viaje está activo y no ha partido
                if (!seat.Trip.IsActive)
                    throw new InvalidOperationException("No se puede vender un asiento de un viaje inactivo");

                if (seat.Trip.DepartureTime <= DateTime.Now.AddMinutes(30)) // 30 minutos antes del viaje
                    throw new InvalidOperationException("No se puede vender un asiento para un viaje que parte en menos de 30 minutos");

                // Buscar o crear cliente
                var customer = await GetOrCreateCustomerAsync(saleDto);

                // Generar número de recibo único
                var receiptNumber = await GenerateReceiptNumberAsync();

                // Crear la venta
                var sale = new Sale
                {
                    CustomerId = customer.Id,
                    SeatId = saleDto.SeatId,
                    Amount = saleDto.Amount,
                    SaleDate = DateTime.Now,
                    ReceiptNumber = receiptNumber
                };

                // Procesar la transacción
                await _saleRepository.AddAsync(sale);
                await _seatRepository.MarkSeatAsOccupiedAsync(saleDto.SeatId);
                await _saleRepository.SaveChangesAsync();

                // Obtener la venta completa para retornar
                var completeSale = await _saleRepository.GetByIdAsync(sale.Id);
                return MapToDto(completeSale!);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al procesar la venta: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByDateAsync(DateTime date)
        {
            try
            {
                var sales = await _saleRepository.GetSalesByDateAsync(date);
                return sales.Select(MapToDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ventas por fecha {date:dd/MM/yyyy}: {ex.Message}", ex);
            }
        }

        public async Task<SaleDto?> GetSaleByReceiptNumberAsync(string receiptNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(receiptNumber))
                    throw new ArgumentException("El número de recibo no puede estar vacío", nameof(receiptNumber));

                var sale = await _saleRepository.GetSaleByReceiptNumberAsync(receiptNumber);
                return sale != null ? MapToDto(sale) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la venta con recibo {receiptNumber}: {ex.Message}", ex);
            }
        }

        public async Task<string> GenerateReceiptNumberAsync()
        {
            try
            {
                return await _saleRepository.GenerateUniqueReceiptNumberAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar número de recibo: {ex.Message}", ex);
            }
        }

        public async Task<SaleDto?> GetSaleByIdAsync(int saleId)
        {
            try
            {
                if (saleId <= 0)
                    throw new ArgumentException("El ID de la venta debe ser mayor a 0", nameof(saleId));

                var sale = await _saleRepository.GetByIdAsync(saleId);
                return sale != null ? MapToDto(sale) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la venta {saleId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    throw new ArgumentException("La fecha de inicio debe ser anterior o igual a la fecha de fin");

                var sales = await _saleRepository.GetSalesByDateRangeAsync(startDate, endDate);
                return sales.Select(MapToDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ventas por rango de fechas: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SaleDto>> GetRecentSalesAsync(int count = 10)
        {
            try
            {
                if (count <= 0)
                    count = 10;

                var sales = await _saleRepository.GetRecentSalesAsync(count);
                return sales.Select(MapToDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ventas recientes: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByCustomerAsync(int customerId)
        {
            try
            {
                if (customerId <= 0)
                    throw new ArgumentException("El ID del cliente debe ser mayor a 0", nameof(customerId));

                var sales = await _saleRepository.GetSalesByCustomerAsync(customerId);
                return sales.Select(MapToDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ventas del cliente {customerId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SaleDto>> GetSalesByCustomerDocumentAsync(string documentNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(documentNumber))
                    throw new ArgumentException("El número de documento no puede estar vacío", nameof(documentNumber));

                var sales = await _saleRepository.GetSalesByCustomerDocumentAsync(documentNumber);
                return sales.Select(MapToDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ventas del documento {documentNumber}: {ex.Message}", ex);
            }
        }

        public async Task<decimal> GetTotalRevenueByDateAsync(DateTime date)
        {
            try
            {
                return await _saleRepository.GetTotalRevenueByDateAsync(date);
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
                if (startDate > endDate)
                    throw new ArgumentException("La fecha de inicio debe ser anterior o igual a la fecha de fin");

                return await _saleRepository.GetTotalRevenueByDateRangeAsync(startDate, endDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al calcular ingresos por rango de fechas: {ex.Message}", ex);
            }
        }

        public async Task<int> CountSalesByDateAsync(DateTime date)
        {
            try
            {
                return await _saleRepository.CountSalesByDateAsync(date);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al contar ventas del {date:dd/MM/yyyy}: {ex.Message}", ex);
            }
        }

        public async Task<bool> CancelSaleAsync(int saleId)
        {
            try
            {
                if (saleId <= 0)
                    throw new ArgumentException("El ID de la venta debe ser mayor a 0", nameof(saleId));

                var sale = await _saleRepository.GetByIdAsync(saleId);
                if (sale == null)
                    throw new ArgumentException($"No se encontró la venta con ID {saleId}");

                // Verificar que se puede cancelar (el viaje no ha partido)
                if (sale.Seat.Trip.DepartureTime <= DateTime.Now)
                    throw new InvalidOperationException("No se puede cancelar una venta de un viaje que ya ha partido");

                // Liberar el asiento
                await _seatRepository.MarkSeatAsAvailableAsync(sale.SeatId);

                // Eliminar la venta (o marcarla como cancelada si prefieres mantener el historial)
                await _saleRepository.DeleteAsync(sale);
                await _saleRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al cancelar la venta {saleId}: {ex.Message}", ex);
            }
        }

        public async Task<List<string>> ValidateSaleDataAsync(CreateSaleDto saleDto)
        {
            var errors = new List<string>();

            try
            {
                if (saleDto == null)
                {
                    errors.Add("Los datos de la venta son requeridos");
                    return errors;
                }

                // Validar datos del cliente
                if (string.IsNullOrWhiteSpace(saleDto.CustomerName))
                    errors.Add("El nombre del cliente es requerido");
                else if (saleDto.CustomerName.Length > 100)
                    errors.Add("El nombre del cliente no puede exceder 100 caracteres");

                if (string.IsNullOrWhiteSpace(saleDto.DocumentNumber))
                    errors.Add("El número de documento es requerido");
                else if (saleDto.DocumentNumber.Length > 20)
                    errors.Add("El número de documento no puede exceder 20 caracteres");

                // Validar teléfono si se proporciona
                if (!string.IsNullOrWhiteSpace(saleDto.Phone))
                {
                    if (saleDto.Phone.Length > 15)
                        errors.Add("El teléfono no puede exceder 15 caracteres");
                    if (!Regex.IsMatch(saleDto.Phone, @"^[\d\-\+\(\)\s]+$"))
                        errors.Add("El formato del teléfono no es válido");
                }

                // Validar email si se proporciona
                if (!string.IsNullOrWhiteSpace(saleDto.Email))
                {
                    if (saleDto.Email.Length > 100)
                        errors.Add("El correo electrónico no puede exceder 100 caracteres");
                    if (!Regex.IsMatch(saleDto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        errors.Add("El formato del correo electrónico no es válido");
                }

                // Validar datos de la venta
                if (saleDto.SeatId <= 0)
                    errors.Add("El ID del asiento debe ser mayor a 0");

                if (saleDto.Amount <= 0)
                    errors.Add("El monto debe ser mayor a 0");
                else if (saleDto.Amount > 9999.99m)
                    errors.Add("El monto no puede exceder 9999.99");

                // Validar que el asiento existe y está disponible
                if (saleDto.SeatId > 0)
                {
                    var seat = await _seatRepository.GetByIdAsync(saleDto.SeatId);
                    if (seat == null)
                        errors.Add($"No se encontró el asiento con ID {saleDto.SeatId}");
                    else if (seat.IsOccupied)
                        errors.Add($"El asiento {seat.SeatNumber} ya está ocupado");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error al validar los datos: {ex.Message}");
            }

            return errors;
        }

        public async Task<SaleConfirmationViewModel?> GetSaleConfirmationViewModelAsync(int saleId)
        {
            try
            {
                if (saleId <= 0)
                    throw new ArgumentException("El ID de la venta debe ser mayor a 0", nameof(saleId));

                var sale = await _saleRepository.GetByIdAsync(saleId);
                if (sale == null)
                    return null;

                return new SaleConfirmationViewModel
                {
                    SaleId = sale.Id,
                    ReceiptNumber = sale.ReceiptNumber,
                    CustomerName = sale.Customer.FullName,
                    TripInfo = $"{sale.Seat.Trip.Route.Origin} → {sale.Seat.Trip.Route.Destination}",
                    SeatNumber = sale.Seat.SeatNumber,
                    Amount = sale.Amount,
                    SaleDate = sale.SaleDate,
                    Success = true,
                    Message = "¡Venta procesada exitosamente!",
                    DocumentNumber = sale.Customer.DocumentNumber,
                    TripDepartureTime = sale.Seat.Trip.DepartureTime,
                    BusInfo = $"{sale.Seat.Trip.Bus.Model} - {sale.Seat.Trip.Bus.PlateNumber}"
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener confirmación de venta {saleId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SaleDto>> SearchSalesAsync(SaleSearchDto searchDto)
        {
            try
            {
                if (searchDto == null)
                    throw new ArgumentNullException(nameof(searchDto));

                var allSales = await _saleRepository.GetSalesWithFullInfoAsync();
                var filteredSales = allSales.AsQueryable();

                // Aplicar filtros
                if (searchDto.StartDate.HasValue)
                {
                    var startDate = searchDto.StartDate.Value.Date;
                    filteredSales = filteredSales.Where(s => s.SaleDate >= startDate);
                }

                if (searchDto.EndDate.HasValue)
                {
                    var endDate = searchDto.EndDate.Value.Date.AddDays(1);
                    filteredSales = filteredSales.Where(s => s.SaleDate < endDate);
                }

                if (!string.IsNullOrWhiteSpace(searchDto.CustomerName))
                {
                    filteredSales = filteredSales.Where(s =>
                        s.Customer.FullName.ToLower().Contains(searchDto.CustomerName.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(searchDto.DocumentNumber))
                {
                    filteredSales = filteredSales.Where(s =>
                        s.Customer.DocumentNumber == searchDto.DocumentNumber);
                }

                if (!string.IsNullOrWhiteSpace(searchDto.ReceiptNumber))
                {
                    filteredSales = filteredSales.Where(s =>
                        s.ReceiptNumber == searchDto.ReceiptNumber);
                }

                if (searchDto.TripId.HasValue)
                {
                    filteredSales = filteredSales.Where(s =>
                        s.Seat.TripId == searchDto.TripId.Value);
                }

                if (searchDto.MinAmount.HasValue)
                {
                    filteredSales = filteredSales.Where(s => s.Amount >= searchDto.MinAmount.Value);
                }

                if (searchDto.MaxAmount.HasValue)
                {
                    filteredSales = filteredSales.Where(s => s.Amount <= searchDto.MaxAmount.Value);
                }

                return filteredSales.Select(MapToDto).OrderByDescending(s => s.SaleDate);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar ventas: {ex.Message}", ex);
            }
        }

        private async Task<Customer> GetOrCreateCustomerAsync(CreateSaleDto saleDto)
        {
            // Buscar cliente existente por documento
            var existingCustomer = await _customerRepository.GetFirstOrDefaultAsync(
                c => c.DocumentNumber == saleDto.DocumentNumber);

            if (existingCustomer != null)
            {
                // Actualizar información si es necesario
                if (existingCustomer.FullName != saleDto.CustomerName ||
                    existingCustomer.Phone != saleDto.Phone ||
                    existingCustomer.Email != saleDto.Email)
                {
                    existingCustomer.FullName = saleDto.CustomerName;
                    existingCustomer.Phone = saleDto.Phone;
                    existingCustomer.Email = saleDto.Email;
                    await _customerRepository.UpdateAsync(existingCustomer);
                    await _customerRepository.SaveChangesAsync();
                }
                return existingCustomer;
            }

            // Crear nuevo cliente
            var newCustomer = new Customer
            {
                FullName = saleDto.CustomerName,
                DocumentNumber = saleDto.DocumentNumber,
                Phone = saleDto.Phone,
                Email = saleDto.Email,
                CreatedAt = DateTime.Now
            };

            await _customerRepository.AddAsync(newCustomer);
            await _customerRepository.SaveChangesAsync();
            return newCustomer;
        }

        private static SaleDto MapToDto(Sale sale)
        {
            return new SaleDto
            {
                Id = sale.Id,
                CustomerName = sale.Customer.FullName,
                DocumentNumber = sale.Customer.DocumentNumber,
                SeatNumber = sale.Seat.SeatNumber,
                TripInfo = $"{sale.Seat.Trip.Route.Origin} → {sale.Seat.Trip.Route.Destination} - {sale.Seat.Trip.DepartureTime:dd/MM/yyyy HH:mm}",
                Amount = sale.Amount,
                SaleDate = sale.SaleDate,
                ReceiptNumber = sale.ReceiptNumber,
                CustomerId = sale.CustomerId,
                SeatId = sale.SeatId,
                TripId = sale.Seat.TripId,
                CustomerPhone = sale.Customer.Phone,
                CustomerEmail = sale.Customer.Email,
                TripDepartureTime = sale.Seat.Trip.DepartureTime,
                TripArrivalTime = sale.Seat.Trip.ArrivalTime,
                BusInfo = $"{sale.Seat.Trip.Bus.Model} - {sale.Seat.Trip.Bus.PlateNumber}",
                RouteInfo = $"{sale.Seat.Trip.Route.Origin} → {sale.Seat.Trip.Route.Destination}"
            };
        }
    }
}