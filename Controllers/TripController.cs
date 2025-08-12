using Microsoft.AspNetCore.Mvc;
using TransportAgency.Business.Interfaces;
using TransportAgency.Models.ViewModels;

namespace TransportAgency.Controllers
{
    public class TripController : Controller
    {
        private readonly ITripService _tripService;
        private readonly ISeatService _seatService;
        private readonly ILogger<TripController> _logger;

        public TripController(
            ITripService tripService,
            ISeatService seatService,
            ILogger<TripController> logger)
        {
            _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));
            _seatService = seatService ?? throw new ArgumentNullException(nameof(seatService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var todayTrips = await _tripService.GetTodayTripsAsync();

                var tripViewModels = todayTrips.Select(trip => new TripViewModel
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

                return View(tripViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de viajes");
                TempData["ErrorMessage"] = "Error al cargar los viajes disponibles.";
                return View(new List<TripViewModel>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMessage"] = "ID de viaje inválido.";
                    return RedirectToAction(nameof(Index));
                }

                var tripDto = await _tripService.GetTripDetailsAsync(id);
                if (tripDto == null)
                {
                    TempData["ErrorMessage"] = "No se encontró el viaje especificado.";
                    return RedirectToAction(nameof(Index));
                }

                // Obtener mapa de asientos
                var seatSelectionViewModel = await _seatService.GetSeatSelectionViewModelAsync(id);
                if (seatSelectionViewModel == null)
                {
                    TempData["ErrorMessage"] = "No se pudo cargar la información de asientos.";
                    return RedirectToAction(nameof(Index));
                }

                // Crear ViewModel completo del viaje con asientos
                var tripViewModel = new TripViewModel
                {
                    Id = tripDto.Id,
                    BusInfo = tripDto.BusInfo,
                    RouteInfo = tripDto.RouteInfo,
                    DepartureTime = tripDto.DepartureTime,
                    ArrivalTime = tripDto.ArrivalTime,
                    Price = tripDto.Price,
                    AvailableSeats = tripDto.AvailableSeats,
                    TotalSeats = tripDto.TotalSeats,
                    IsActive = tripDto.IsActive
                };

                // Pasar ambos ViewModels a la vista
                ViewBag.SeatSelection = seatSelectionViewModel;

                return View(tripViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles del viaje {TripId}", id);
                TempData["ErrorMessage"] = "Error al cargar los detalles del viaje.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string origin, string destination, DateTime? departureDate)
        {
            try
            {
                var searchDto = new Models.DTOs.TripSearchDto
                {
                    Origin = origin,
                    Destination = destination,
                    DepartureDate = departureDate,
                    OnlyAvailable = true
                };

                var searchResults = await _tripService.SearchTripsAsync(searchDto);

                var tripViewModels = searchResults.Select(trip => new TripViewModel
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

                ViewBag.SearchPerformed = true;
                ViewBag.SearchOrigin = origin;
                ViewBag.SearchDestination = destination;
                ViewBag.SearchDate = departureDate;

                return View("Index", tripViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar viajes");
                TempData["ErrorMessage"] = "Error al realizar la búsqueda de viajes.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}