using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.ViewModels
{
    public class TripViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Información del Bus")]
        public string BusInfo { get; set; } = string.Empty;

        [Display(Name = "Ruta")]
        public string RouteInfo { get; set; } = string.Empty;

        [Display(Name = "Hora de Salida")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime DepartureTime { get; set; }

        [Display(Name = "Hora de Llegada")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime ArrivalTime { get; set; }

        [Display(Name = "Precio")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [Display(Name = "Asientos Disponibles")]
        public int AvailableSeats { get; set; }

        [Display(Name = "Total de Asientos")]
        public int TotalSeats { get; set; }

        [Display(Name = "Activo")]
        public bool IsActive { get; set; }

        // Propiedades calculadas
        public int OccupiedSeats => TotalSeats - AvailableSeats;

        public double PercentageOccupied => TotalSeats > 0 ? (double)OccupiedSeats / TotalSeats * 100 : 0;

        public string PercentageOccupiedFormatted => $"{PercentageOccupied:F1}%";

        public string DepartureTimeFormatted => DepartureTime.ToString("dd/MM/yyyy HH:mm");

        public string ArrivalTimeFormatted => ArrivalTime.ToString("dd/MM/yyyy HH:mm");

        public string PriceFormatted => Price.ToString("C");

        public TimeSpan Duration => ArrivalTime - DepartureTime;

        public string DurationFormatted => $"{Duration.Hours}h {Duration.Minutes}m";

        public bool HasAvailableSeats => AvailableSeats > 0;

        public string AvailabilityStatus => HasAvailableSeats ? "Disponible" : "Agotado";

        public string AvailabilityClass => HasAvailableSeats ? "text-success" : "text-danger";

        public bool IsDepartureSoon => DepartureTime <= DateTime.Now.AddHours(2) && DepartureTime > DateTime.Now;

        public string StatusBadgeClass => IsActive
            ? (HasAvailableSeats ? "badge bg-success" : "badge bg-warning")
            : "badge bg-secondary";

        public string StatusText => !IsActive ? "Inactivo" : AvailabilityStatus;
    }
}