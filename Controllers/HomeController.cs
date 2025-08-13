using Microsoft.AspNetCore.Mvc;
using TransportAgency.Models;
using TransportAgency.Models.ViewModels;
using TransportAgency.Business.Interfaces;
using System.Diagnostics;

namespace TransportAgency.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITripService? _tripService;
        private readonly ISaleService? _saleService;

        public HomeController(ILogger<HomeController> logger, ITripService? tripService = null, ISaleService? saleService = null)
        {
            _logger = logger;
            _tripService = tripService;
            _saleService = saleService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var viewModel = new DashboardViewModel
                {
                    CurrentDate = DateTime.Now
                };

                // Si los servicios están disponibles, obtener datos reales
                if (_tripService != null)
                {
                    var todayTrips = await _tripService.GetTodayTripsAsync();
                    viewModel.TotalTripsToday = todayTrips.Count();

                    var upcomingTrips = await _tripService.GetUpcomingTripsAsync(5);
                    viewModel.UpcomingTrips = upcomingTrips.Select(trip => new TripViewModel
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
                }

                if (_saleService != null)
                {
                    var todaySales = await _saleService.GetSalesByDateAsync(DateTime.Today);
                    viewModel.TotalSalesToday = todaySales.Count();
                    viewModel.TotalRevenue = await _saleService.GetTotalRevenueByDateAsync(DateTime.Today);

                    var recentSales = await _saleService.GetRecentSalesAsync(5);
                    viewModel.RecentSales = recentSales.Select(sale => new SaleViewModel
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
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el dashboard");

                // Retornar vista con datos por defecto en caso de error
                var defaultViewModel = new DashboardViewModel
                {
                    CurrentDate = DateTime.Now,
                    TotalTripsToday = 0,
                    TotalSalesToday = 0,
                    TotalRevenue = 0,
                    UpcomingTrips = new List<TripViewModel>(),
                    RecentSales = new List<SaleViewModel>()
                };

                return View(defaultViewModel);
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