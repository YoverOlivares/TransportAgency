using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.ViewModels
{
    public class SeatViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Número de Asiento")]
        public string SeatNumber { get; set; } = string.Empty;

        [Display(Name = "Ocupado")]
        public bool IsOccupied { get; set; }

        [Display(Name = "ID del Viaje")]
        public int TripId { get; set; }

        [Display(Name = "Información del Viaje")]
        public string TripInfo { get; set; } = string.Empty;

        [Display(Name = "Precio")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [Display(Name = "Seleccionado")]
        public bool IsSelected { get; set; }

        // Propiedades adicionales para UI
        public DateTime? DepartureTime { get; set; }
        public string RouteInfo { get; set; } = string.Empty;

        // Propiedades calculadas
        public string StatusText => IsOccupied ? "Ocupado" : "Disponible";

        public string StatusClass => IsOccupied ? "seat-occupied" : (IsSelected ? "seat-selected" : "seat-available");

        public string StatusBadgeClass => IsOccupied ? "badge bg-danger" : "badge bg-success";

        public bool IsAvailable => !IsOccupied;

        public string PriceFormatted => Price.ToString("C");

        public string SeatDisplayInfo => $"Asiento {SeatNumber} - {StatusText}";

        public string TooltipText => IsOccupied
            ? $"Asiento {SeatNumber} - No disponible"
            : $"Asiento {SeatNumber} - Disponible - {PriceFormatted}";

        public string CssClass
        {
            get
            {
                if (IsOccupied) return "btn btn-danger btn-seat disabled";
                if (IsSelected) return "btn btn-warning btn-seat selected";
                return "btn btn-success btn-seat available";
            }
        }

        public string IconClass => IsOccupied ? "fas fa-times" : (IsSelected ? "fas fa-check" : "fas fa-chair");
    }

    public class SeatSelectionViewModel
    {
        [Display(Name = "Información del Viaje")]
        public TripViewModel TripInfo { get; set; } = new TripViewModel();

        [Display(Name = "Asientos")]
        public List<SeatViewModel> Seats { get; set; } = new List<SeatViewModel>();

        [Display(Name = "Asiento Seleccionado")]
        public int? SelectedSeatId { get; set; }

        [Display(Name = "Total a Pagar")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal TotalAmount { get; set; }

        // Propiedades calculadas
        public bool HasSelectedSeat => SelectedSeatId.HasValue;

        public SeatViewModel? SelectedSeat => Seats.FirstOrDefault(s => s.Id == SelectedSeatId);

        public int AvailableSeatsCount => Seats.Count(s => s.IsAvailable);

        public int OccupiedSeatsCount => Seats.Count(s => s.IsOccupied);

        public string TotalAmountFormatted => TotalAmount.ToString("C");

        public bool CanProceedToPayment => HasSelectedSeat && TotalAmount > 0;

        public string SelectionSummary => HasSelectedSeat
            ? $"Asiento {SelectedSeat?.SeatNumber} seleccionado - {TotalAmountFormatted}"
            : "Seleccione un asiento para continuar";
    }
}