using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.ViewModels
{
    public class CreateSaleViewModel
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [Display(Name = "Nombre Completo")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de documento es obligatorio")]
        [Display(Name = "Número de Documento")]
        [StringLength(20, ErrorMessage = "El documento no puede exceder 20 caracteres")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Display(Name = "Teléfono")]
        [Phone(ErrorMessage = "El formato del teléfono no es válido")]
        [StringLength(15, ErrorMessage = "El teléfono no puede exceder 15 caracteres")]
        public string? Phone { get; set; }

        [Display(Name = "Correo Electrónico")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        public string? Email { get; set; }

        // Información del asiento (readonly)
        public int SeatId { get; set; }

        [Display(Name = "Número de Asiento")]
        public string SeatNumber { get; set; } = string.Empty;

        [Display(Name = "Información del Viaje")]
        public string TripInfo { get; set; } = string.Empty;

        [Display(Name = "Monto a Pagar")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Amount { get; set; }

        [Display(Name = "Asiento Seleccionado")]
        public SeatViewModel? SelectedSeatViewModel { get; set; }

        // Propiedades calculadas
        public string AmountFormatted => Amount.ToString("C");

        public string PurchaseSummary => $"Asiento {SeatNumber} - {TripInfo} - {AmountFormatted}";

        public bool IsValidCustomerData => !string.IsNullOrWhiteSpace(FullName) && !string.IsNullOrWhiteSpace(DocumentNumber);
    }

    public class SaleViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Cliente")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Documento")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Display(Name = "Asiento")]
        public string SeatNumber { get; set; } = string.Empty;

        [Display(Name = "Viaje")]
        public string TripInfo { get; set; } = string.Empty;

        [Display(Name = "Monto")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Amount { get; set; }

        [Display(Name = "Fecha de Venta")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime SaleDate { get; set; }

        [Display(Name = "Número de Recibo")]
        public string ReceiptNumber { get; set; } = string.Empty;

        // Propiedades adicionales para detalles
        public string? CustomerPhone { get; set; }
        public string? CustomerEmail { get; set; }
        public DateTime? TripDepartureTime { get; set; }
        public string? BusInfo { get; set; }

        // Propiedades calculadas
        public string AmountFormatted => Amount.ToString("C");

        public string SaleDateFormatted => SaleDate.ToString("dd/MM/yyyy HH:mm");

        public string SaleTimeFormatted => SaleDate.ToString("HH:mm");

        public string CustomerInfo => $"{CustomerName} ({DocumentNumber})";

        public string SaleSummary => $"{CustomerName} - {SeatNumber} - {AmountFormatted}";

        public bool IsRecentSale => SaleDate >= DateTime.Now.AddDays(-1);

        public string RecentBadge => IsRecentSale ? "badge bg-info" : "";

        public string TripDepartureFormatted => TripDepartureTime?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

        public bool HasContactInfo => !string.IsNullOrWhiteSpace(CustomerPhone) || !string.IsNullOrWhiteSpace(CustomerEmail);
    }

    public class SaleConfirmationViewModel
    {
        public int SaleId { get; set; }

        [Display(Name = "Número de Recibo")]
        public string ReceiptNumber { get; set; } = string.Empty;

        [Display(Name = "Cliente")]
        public string CustomerName { get; set; } = string.Empty;

        [Display(Name = "Información del Viaje")]
        public string TripInfo { get; set; } = string.Empty;

        [Display(Name = "Asiento")]
        public string SeatNumber { get; set; } = string.Empty;

        [Display(Name = "Monto")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal Amount { get; set; }

        [Display(Name = "Fecha de Venta")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        public DateTime SaleDate { get; set; }

        [Display(Name = "Exitoso")]
        public bool Success { get; set; }

        [Display(Name = "Mensaje")]
        public string Message { get; set; } = string.Empty;

        // Propiedades adicionales
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime TripDepartureTime { get; set; }
        public string BusInfo { get; set; } = string.Empty;

        // Propiedades calculadas
        public string AmountFormatted => Amount.ToString("C");

        public string SaleDateFormatted => SaleDate.ToString("dd/MM/yyyy HH:mm");

        public string ConfirmationTitle => Success ? "¡Compra Exitosa!" : "Error en la Compra";

        public string MessageClass => Success ? "alert alert-success" : "alert alert-danger";

        public string IconClass => Success ? "fas fa-check-circle text-success" : "fas fa-exclamation-circle text-danger";

        public string PurchaseDetails => $"Recibo: {ReceiptNumber} | Cliente: {CustomerName} | Asiento: {SeatNumber} | {AmountFormatted}";

        public bool CanDownloadReceipt => Success && !string.IsNullOrWhiteSpace(ReceiptNumber);

        public string TripDepartureFormatted => TripDepartureTime.ToString("dd/MM/yyyy HH:mm");
    }
}