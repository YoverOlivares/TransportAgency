using System.ComponentModel.DataAnnotations;

namespace TransportAgency.Models.ViewModels
{
    public class DashboardViewModel
    {
        [Display(Name = "Viajes de Hoy")]
        public int TotalTripsToday { get; set; }

        [Display(Name = "Ventas de Hoy")]
        public int TotalSalesToday { get; set; }

        [Display(Name = "Ingresos de Hoy")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal TotalRevenue { get; set; }

        [Display(Name = "Próximos Viajes")]
        public List<TripViewModel> UpcomingTrips { get; set; } = new List<TripViewModel>();

        [Display(Name = "Ventas Recientes")]
        public List<SaleViewModel> RecentSales { get; set; } = new List<SaleViewModel>();

        [Display(Name = "Fecha Actual")]
        public DateTime CurrentDate { get; set; } = DateTime.Now;

        // Propiedades calculadas
        public string TotalRevenueFormatted => TotalRevenue.ToString("C");

        public bool HasUpcomingTrips => UpcomingTrips?.Any() == true;

        public bool HasRecentSales => RecentSales?.Any() == true;

        public string WelcomeMessage => $"Bienvenido - {CurrentDate:dddd, dd MMMM yyyy}";
    }
}