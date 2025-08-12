using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.DTOs
{
    public class CreateSaleDto
    {
        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de documento es obligatorio")]
        [StringLength(20, ErrorMessage = "El documento no puede exceder 20 caracteres")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [StringLength(15, ErrorMessage = "El teléfono no puede exceder 15 caracteres")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "El ID del asiento es obligatorio")]
        public int SeatId { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Range(0.01, 9999.99, ErrorMessage = "El monto debe estar entre 0.01 y 9999.99")]
        public decimal Amount { get; set; }
    }

    public class SaleDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public string TripInfo { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime SaleDate { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;

        // Propiedades adicionales para información completa
        public int CustomerId { get; set; }
        public int SeatId { get; set; }
        public int TripId { get; set; }
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public DateTime TripDepartureTime { get; set; }
        public DateTime TripArrivalTime { get; set; }
        public string BusInfo { get; set; } = string.Empty;
        public string RouteInfo { get; set; } = string.Empty;
    }

    public class SaleReportDto
    {
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public DateTime ReportDate { get; set; }
        public List<SaleDto> Sales { get; set; } = new List<SaleDto>();
    }

    public class SaleSearchDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CustomerName { get; set; }
        public string? DocumentNumber { get; set; }
        public string? ReceiptNumber { get; set; }
        public int? TripId { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
    }
}