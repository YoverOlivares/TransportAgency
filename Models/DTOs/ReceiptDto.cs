namespace TransportAgency.Models.DTOs
{
    public class ReceiptDto
    {
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; }
        public decimal Amount { get; set; }

        // Información del Cliente
        public CustomerInfo CustomerInfo { get; set; } = new CustomerInfo();

        // Información del Viaje
        public TripInfo TripInfo { get; set; } = new TripInfo();

        // Información del Asiento
        public string SeatNumber { get; set; } = string.Empty;

        // Información del Bus
        public BusInfo BusInfo { get; set; } = new BusInfo();

        // Información de la Empresa
        public CompanyInfo CompanyInfo { get; set; } = new CompanyInfo();

        // Propiedades adicionales para el recibo
        public string FormattedSaleDate => SaleDate.ToString("dd/MM/yyyy HH:mm");
        public string FormattedAmount => Amount.ToString("C");
        public string FullTripInfo => $"{TripInfo.Origin} → {TripInfo.Destination}";
        public string TripDateTime => $"{TripInfo.DepartureDate:dd/MM/yyyy} a las {TripInfo.DepartureTime:HH:mm}";
    }

    public class CustomerInfo
    {
        public string FullName { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    public class TripInfo
    {
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Distance { get; set; }
        public decimal Price { get; set; }
    }

    public class BusInfo
    {
        public string Model { get; set; } = string.Empty;
        public string PlateNumber { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string FullInfo => $"{Model} - {PlateNumber}";
    }

    public class CompanyInfo
    {
        public string CompanyName { get; set; } = "Agencia de Transportes";
        public string Address { get; set; } = "Av. Principal 123, Ciudad";
        public string Phone { get; set; } = "123-456-7890";
        public string Email { get; set; } = "info@transportes.com";
        public string Website { get; set; } = "www.transportes.com";
        public string Ruc { get; set; } = "12345678901";
        public string LegalName { get; set; } = "Transportes S.A.C.";
    }
}