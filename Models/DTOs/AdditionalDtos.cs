using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.DTOs
{
    public class TripDto
    {
        public int Id { get; set; }
        public string BusModel { get; set; } = string.Empty;
        public string BusPlateNumber { get; set; } = string.Empty;
        public int BusCapacity { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public decimal BasePrice { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public int TotalSeats { get; set; }
        public bool IsActive { get; set; }

        // Propiedades calculadas
        public int OccupiedSeats => TotalSeats - AvailableSeats;
        public string BusInfo => $"{BusModel} - {BusPlateNumber}";
        public string RouteInfo => $"{Origin} → {Destination}";
        public TimeSpan Duration => ArrivalTime - DepartureTime;
    }

    public class SeatDto
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public bool IsOccupied { get; set; }
        public int TripId { get; set; }
        public string TripInfo { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }

        // Información adicional del viaje para contexto
        public DateTime? TripDepartureTime { get; set; }
        public string? RouteInfo { get; set; }
        public string? BusInfo { get; set; }
    }

    public class CustomerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }

        // Información adicional
        public int TotalPurchases { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
    }

    public class BusDto
    {
        public int Id { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Información adicional
        public int TotalTrips { get; set; }
        public int ActiveTrips { get; set; }
        public decimal? AverageOccupancy { get; set; }
    }

    public class RouteDto
    {
        public int Id { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Información adicional
        public int TotalTrips { get; set; }
        public int ActiveTrips { get; set; }
        public string RouteInfo => $"{Origin} → {Destination}";
    }

    public class DashboardDto
    {
        public int TotalTripsToday { get; set; }
        public int TotalSalesToday { get; set; }
        public decimal TotalRevenueToday { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalActiveBuses { get; set; }
        public int TotalActiveRoutes { get; set; }
        public List<TripDto> UpcomingTrips { get; set; } = new List<TripDto>();
        public List<SaleDto> RecentSales { get; set; } = new List<SaleDto>();
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }

    // DTOs para búsquedas y filtros
    public class TripSearchDto
    {
        public string? Origin { get; set; }
        public string? Destination { get; set; }
        public DateTime? DepartureDate { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? OnlyAvailable { get; set; }
    }

    public class SeatAvailabilityDto
    {
        public int TripId { get; set; }
        public string TripInfo { get; set; } = string.Empty;
        public List<SeatDto> Seats { get; set; } = new List<SeatDto>();
        public int AvailableCount { get; set; }
        public int OccupiedCount { get; set; }
        public int TotalCount { get; set; }
        public decimal SeatPrice { get; set; }
    }
}