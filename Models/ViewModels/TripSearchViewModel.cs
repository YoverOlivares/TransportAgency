using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.ViewModels
{
    public class TripSearchViewModel
    {
        [Required(ErrorMessage = "El origen es obligatorio")]
        [Display(Name = "Origen")]
        public string Origin { get; set; } = string.Empty;

        [Required(ErrorMessage = "El destino es obligatorio")]
        [Display(Name = "Destino")]
        public string Destination { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de viaje es obligatoria")]
        [Display(Name = "Fecha de Viaje")]
        [DataType(DataType.Date)]
        public DateTime TravelDate { get; set; } = DateTime.Today.AddDays(1);
    }

    public class AvailableTripViewModel
    {
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime TravelDate { get; set; }
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string BusType { get; set; } = string.Empty;

        public string PriceFormatted => Price.ToString("C");
        public string TravelDateFormatted => TravelDate.ToString("dd/MM/yyyy");
        public string RouteInfo => $"{Origin} → {Destination}";
    }
}
