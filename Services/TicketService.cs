using Microsoft.EntityFrameworkCore;
using TransportAgency.Data;
using TransportAgency.Models.Entities;
using TransportAgency.Models.ViewModels;

namespace TransportAgency.Services
{
    public class TicketService : ITicketService
    {
        private readonly TransportAgencyContext _context;

        public TicketService(TransportAgencyContext context)
        {
            _context = context;
        }

        public List<AvailableTripViewModel> GetAvailableTrips(string origin, string destination, DateTime travelDate)
        {
            // Datos simulados de viajes disponibles
            var routes = GetSimulatedRoutes();

            return routes
                .Where(r => r.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase) &&
                           r.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase))
                .Select(r => new AvailableTripViewModel
                {
                    Origin = r.Origin,
                    Destination = r.Destination,
                    TravelDate = travelDate,
                    DepartureTime = r.DepartureTime,
                    ArrivalTime = r.ArrivalTime,
                    Price = r.Price,
                    AvailableSeats = r.AvailableSeats,
                    Duration = r.Duration,
                    BusType = r.BusType
                })
                .ToList();
        }

        public async Task<int> CreateSaleAsync(PassengerViewModel passengerData)
        {
            // Buscar o crear cliente
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.DocumentNumber == passengerData.DocumentNumber);

            if (customer == null)
            {
                customer = new Customer
                {
                    FullName = passengerData.FullName,
                    DocumentNumber = passengerData.DocumentNumber,
                    Phone = passengerData.Phone,
                    Email = passengerData.Email
                };
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Actualizar datos del cliente
                customer.FullName = passengerData.FullName;
                customer.Phone = passengerData.Phone;
                customer.Email = passengerData.Email;
            }

            // Crear venta
            var sale = new Sale
            {
                CustomerId = customer.Id,
                Origin = passengerData.Origin,
                Destination = passengerData.Destination,
                TravelDate = passengerData.TravelDate,
                SeatNumber = passengerData.SeatNumber,
                Price = passengerData.Price,
                TicketNumber = GenerateTicketNumber(),
                TravelTime = passengerData.DepartureTime
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            return sale.Id;
        }

        public async Task<ConfirmationViewModel?> GetSaleConfirmationAsync(int saleId)
        {
            var sale = await _context.Sales
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) return null;

            // Obtener información adicional del viaje simulado
            var tripInfo = GetSimulatedTripInfo(sale.Origin, sale.Destination, sale.TravelTime);

            return new ConfirmationViewModel
            {
                SaleId = sale.Id,
                TicketNumber = sale.TicketNumber,
                PassengerName = sale.Customer.FullName,
                DocumentNumber = sale.Customer.DocumentNumber,
                Origin = sale.Origin,
                Destination = sale.Destination,
                TravelDate = sale.TravelDate,
                DepartureTime = sale.TravelTime,
                ArrivalTime = tripInfo.ArrivalTime,
                SeatNumber = sale.SeatNumber,
                Price = sale.Price,
                PurchaseDate = sale.PurchaseDate,
                Phone = sale.Customer.Phone,
                Email = sale.Customer.Email,
                Duration = tripInfo.Duration,
                BusType = tripInfo.BusType
            };
        }

        public async Task<List<SaleListItemViewModel>> GetAllSalesAsync()
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .OrderByDescending(s => s.PurchaseDate)
                .Select(s => new SaleListItemViewModel
                {
                    Id = s.Id,
                    TicketNumber = s.TicketNumber,
                    PassengerName = s.Customer.FullName,
                    DocumentNumber = s.Customer.DocumentNumber,
                    Route = $"{s.Origin} → {s.Destination}",
                    TravelDate = s.TravelDate,
                    SeatNumber = s.SeatNumber,
                    Price = s.Price,
                    PurchaseDate = s.PurchaseDate
                })
                .ToListAsync();
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> UpdateSaleAsync(EditSaleViewModel model)
        {
            var sale = await _context.Sales
                .Include(s => s.Customer)
                .FirstOrDefaultAsync(s => s.Id == model.Id);

            if (sale == null) return false;

            // Actualizar datos del cliente
            sale.Customer.FullName = model.PassengerName;
            sale.Customer.DocumentNumber = model.DocumentNumber;
            sale.Customer.Phone = model.Phone;
            sale.Customer.Email = model.Email;

            // Actualizar datos de la venta
            sale.Origin = model.Origin;
            sale.Destination = model.Destination;
            sale.TravelDate = model.TravelDate;
            sale.SeatNumber = model.SeatNumber;
            sale.Price = model.Price;
            sale.TravelTime = model.TravelTime;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSaleAsync(int id)
        {
            var sale = await _context.Sales.FindAsync(id);
            if (sale == null) return false;

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            var today = DateTime.Today;

            var totalSales = await _context.Sales.CountAsync();
            var totalRevenue = await _context.Sales.SumAsync(s => s.Price);

            var todaySales = await _context.Sales
                .Where(s => s.PurchaseDate.Date == today)
                .CountAsync();

            var todayRevenue = await _context.Sales
                .Where(s => s.PurchaseDate.Date == today)
                .SumAsync(s => (decimal?)s.Price) ?? 0;

            var recentSales = await _context.Sales
                .Include(s => s.Customer)
                .OrderByDescending(s => s.PurchaseDate)
                .Take(5)
                .Select(s => new SaleListItemViewModel
                {
                    Id = s.Id,
                    TicketNumber = s.TicketNumber,
                    PassengerName = s.Customer.FullName,
                    DocumentNumber = s.Customer.DocumentNumber,
                    Route = $"{s.Origin} → {s.Destination}",
                    TravelDate = s.TravelDate,
                    SeatNumber = s.SeatNumber,
                    Price = s.Price,
                    PurchaseDate = s.PurchaseDate
                })
                .ToListAsync();

            return new AdminDashboardViewModel
            {
                TotalSales = totalSales,
                TotalRevenue = totalRevenue,
                TodaySales = todaySales,
                TodayRevenue = todayRevenue,
                RecentSales = recentSales
            };
        }

        public List<string> GetAvailableCities()
        {
            return new List<string>
            {
                "Lima", "Arequipa", "Cusco", "Trujillo", "Chiclayo",
                "Piura", "Iquitos", "Huancayo", "Ayacucho", "Pucallpa"
            };
        }

        public List<string> GetAvailableSeats(string origin, string destination, DateTime travelDate, string departureTime)
        {
            // Simulación de asientos disponibles
            var totalSeats = 40;
            var availableSeats = new List<string>();

            for (int i = 1; i <= totalSeats; i++)
            {
                availableSeats.Add($"A{i:D2}");
            }

            return availableSeats;
        }

        private string GenerateTicketNumber()
        {
            return $"TK{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        private List<SimulatedRoute> GetSimulatedRoutes()
        {
            return new List<SimulatedRoute>
            {
                // Lima - Arequipa
                new("Lima", "Arequipa", "06:00", "22:00", 80m, 35, "16h", "Económico"),
                new("Lima", "Arequipa", "14:00", "06:00", 120m, 28, "16h", "VIP"),
                new("Lima", "Arequipa", "22:00", "14:00", 100m, 32, "16h", "Semi Cama"),
                
                // Lima - Cusco
                new("Lima", "Cusco", "07:00", "20:00", 90m, 30, "13h", "Económico"),
                new("Lima", "Cusco", "15:00", "04:00", 150m, 25, "13h", "VIP"),
                new("Lima", "Cusco", "21:00", "10:00", 110m, 28, "13h", "Semi Cama"),
                
                // Lima - Trujillo
                new("Lima", "Trujillo", "08:00", "16:00", 50m, 35, "8h", "Económico"),
                new("Lima", "Trujillo", "16:00", "00:00", 75m, 30, "8h", "Semi Cama"),
                new("Lima", "Trujillo", "23:00", "07:00", 65m, 32, "8h", "Cama"),
                
                // Rutas de regreso
                new("Arequipa", "Lima", "06:00", "22:00", 80m, 35, "16h", "Económico"),
                new("Arequipa", "Lima", "14:00", "06:00", 120m, 28, "16h", "VIP"),
                new("Cusco", "Lima", "07:00", "20:00", 90m, 30, "13h", "Económico"),
                new("Cusco", "Lima", "15:00", "04:00", 150m, 25, "13h", "VIP"),
                new("Trujillo", "Lima", "08:00", "16:00", 50m, 35, "8h", "Económico"),
                new("Trujillo", "Lima", "16:00", "00:00", 75m, 30, "8h", "Semi Cama")
            };
        }

        private (string ArrivalTime, string Duration, string BusType) GetSimulatedTripInfo(string origin, string destination, string departureTime)
        {
            var routes = GetSimulatedRoutes();
            var route = routes.FirstOrDefault(r => r.Origin == origin && r.Destination == destination && r.DepartureTime == departureTime);

            return route != null
                ? (route.ArrivalTime, route.Duration, route.BusType)
                : ("00:00", "0h", "Económico");
        }

        private record SimulatedRoute(string Origin, string Destination, string DepartureTime,
            string ArrivalTime, decimal Price, int AvailableSeats, string Duration, string BusType);
    }
}
