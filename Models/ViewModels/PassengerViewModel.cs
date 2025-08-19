using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.ViewModels
{
    public class PassengerViewModel
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [Display(Name = "Nombre Completo")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de documento es obligatorio")]
        [Display(Name = "Número de Documento")]
        [StringLength(20)]
        public string DocumentNumber { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        [Phone]
        [StringLength(15)]
        public string? Phone { get; set; }

        [Display(Name = "Correo Electrónico")]
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        // Trip Info (hidden fields)
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime TravelDate { get; set; }
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string BusType { get; set; } = string.Empty;

        public string PriceFormatted => Price.ToString("C");
        public string RouteInfo => $"{Origin} → {Destination}";
        public string TravelInfo => $"{TravelDate:dd/MM/yyyy} - {DepartureTime}";
    }
}
