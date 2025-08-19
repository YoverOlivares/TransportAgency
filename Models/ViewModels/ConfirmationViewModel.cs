namespace TransportAgency.Models.ViewModels
{
    public class ConfirmationViewModel
    {
        public int SaleId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime TravelDate { get; set; }
        public string DepartureTime { get; set; } = string.Empty;
        public string ArrivalTime { get; set; } = string.Empty;
        public string SeatNumber { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Duration { get; set; } = string.Empty;
        public string BusType { get; set; } = string.Empty;

        public string PriceFormatted => Price.ToString("C");
        public string RouteInfo => $"{Origin} → {Destination}";
        public string TravelDateFormatted => TravelDate.ToString("dd/MM/yyyy");
        public string PurchaseDateFormatted => PurchaseDate.ToString("dd/MM/yyyy HH:mm");
        public string TravelInfo => $"{TravelDateFormatted} - {DepartureTime}";
    }
}