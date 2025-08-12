using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TransportAgency.Business.Interfaces;
using TransportAgency.Models;
using TransportAgency.Models.ViewModels;

namespace TransportAgency.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITripService _tripService;
        private readonly ISaleService _saleService;

        public HomeController(
            ILogger<HomeController> logger,
            ITripService tripService,
            ISaleService saleService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));
            _saleService = saleService ?? throw new ArgumentNullException(nameof(saleService));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var today = DateTime.Today;

                // Obtener estadísticas del día
                var todayTrips = await _tripService.GetTodayTripsAsync();
                var todaySales = await _saleService.GetSalesByDateAsync(today);
                var totalRevenue = await _saleService.GetTotalRevenueByDateAsync(today);
                var upcomingTrips = await _tripService.GetUpcomingTripsAsync(5);
                var recentSales = await _saleService.GetRecentSalesAsync(10);

                // Mapear a ViewModels
                var upcomingTripViewModels = upcomingTrips.Select(trip => new TripViewModel
                {
                    Id = trip.Id,
                    BusInfo = trip.BusInfo,
                    RouteInfo = trip.RouteInfo,
                    DepartureTime = trip.DepartureTime,
                    ArrivalTime = trip.ArrivalTime,
                    Price = trip.Price,
                    AvailableSeats = trip.AvailableSeats,
                    TotalSeats = trip.TotalSeats,
                    IsActive = trip.IsActive
                }).ToList();

                var recentSaleViewModels = recentSales.Select(sale => new SaleViewModel
                {
                    Id = sale.Id,
                    CustomerName = sale.CustomerName,
                    DocumentNumber = sale.DocumentNumber,
                    SeatNumber = sale.SeatNumber,
                    TripInfo = sale.TripInfo,
                    Amount = sale.Amount,
                    SaleDate = sale.SaleDate,
                    ReceiptNumber = sale.ReceiptNumber
                }).ToList();

                // Crear ViewModel del dashboard
                var dashboardViewModel = new DashboardViewModel
                {
                    TotalTripsToday = todayTrips.Count(),
                    TotalSalesToday = todaySales.Count(),
                    TotalRevenue = totalRevenue,
                    UpcomingTrips = upcomingTripViewModels,
                    RecentSales = recentSaleViewModels,
                    CurrentDate = DateTime.Now
                };

                return View(dashboardViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el dashboard");
                TempData["ErrorMessage"] = "Error al cargar la información del dashboard.";

                // Retornar dashboard vacío en caso de error
                var emptyDashboard = new DashboardViewModel
                {
                    CurrentDate = DateTime.Now
                };

                return View(emptyDashboard);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}