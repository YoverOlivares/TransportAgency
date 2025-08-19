using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "El usuario es obligatorio")]
        [Display(Name = "Usuario")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [Display(Name = "Contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class AdminDashboardViewModel
    {
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TodaySales { get; set; }
        public decimal TodayRevenue { get; set; }
        public List<SaleListItemViewModel> RecentSales { get; set; } = new();

        public string TotalRevenueFormatted => TotalRevenue.ToString("C");
        public string TodayRevenueFormatted => TodayRevenue.ToString("C");
    }

    public class SaleListItemViewModel
    {
        public int Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public DateTime TravelDate { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }

        public string PriceFormatted => Price.ToString("C");
        public string TravelDateFormatted => TravelDate.ToString("dd/MM/yyyy");
        public string PurchaseDateFormatted => PurchaseDate.ToString("dd/MM/yyyy HH:mm");
    }

    public class EditSaleViewModel
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Nombre del Pasajero")]
        public string PassengerName { get; set; } = string.Empty;

        [Required, StringLength(20)]
        [Display(Name = "Número de Documento")]
        public string DocumentNumber { get; set; } = string.Empty;

        [StringLength(15)]
        [Display(Name = "Teléfono")]
        public string? Phone { get; set; }

        [StringLength(100), EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Origen")]
        public string Origin { get; set; } = string.Empty;

        [Required, StringLength(100)]
        [Display(Name = "Destino")]
        public string Destination { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Fecha de Viaje")]
        [DataType(DataType.Date)]
        public DateTime TravelDate { get; set; }

        [Required, StringLength(10)]
        [Display(Name = "Número de Asiento")]
        public string SeatNumber { get; set; } = string.Empty;

        [Required, Range(0.01, 9999.99)]
        [Display(Name = "Precio")]
        public decimal Price { get; set; }

        [Required, StringLength(20)]
        [Display(Name = "Hora de Viaje")]
        public string TravelTime { get; set; } = string.Empty;

        public string TicketNumber { get; set; } = string.Empty;

        public string PriceFormatted => Price.ToString("C");
    }
}