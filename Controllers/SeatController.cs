using Microsoft.AspNetCore.Mvc;
using TransportAgency.Business.Interfaces;
using TransportAgency.Models.ViewModels;

namespace TransportAgency.Controllers
{
    public class SeatController : Controller
    {
        private readonly ISeatService _seatService;
        private readonly ITripService _tripService;
        private readonly ILogger<SeatController> _logger;

        public SeatController(
            ISeatService seatService,
            ITripService tripService,
            ILogger<SeatController> logger)
        {
            _seatService = seatService ?? throw new ArgumentNullException(nameof(seatService));
            _tripService = tripService ?? throw new ArgumentNullException(nameof(tripService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> SelectSeat(int tripId)
        {
            try
            {
                if (tripId <= 0)
                {
                    TempData["ErrorMessage"] = "ID de viaje inválido.";
                    return RedirectToAction("Index", "Trip");
                }

                // Verificar que el viaje existe y tiene asientos disponibles
                var hasAvailableSeats = await _tripService.HasAvailableSeatsAsync(tripId);
                if (!hasAvailableSeats)
                {
                    TempData["ErrorMessage"] = "No hay asientos disponibles para este viaje.";
                    return RedirectToAction("Details", "Trip", new { id = tripId });
                }

                // Obtener el ViewModel de selección de asientos
                var seatSelectionViewModel = await _seatService.GetSeatSelectionViewModelAsync(tripId);
                if (seatSelectionViewModel == null)
                {
                    TempData["ErrorMessage"] = "No se pudo cargar la información de asientos.";
                    return RedirectToAction("Details", "Trip", new { id = tripId });
                }

                return View(seatSelectionViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar selección de asientos para el viaje {TripId}", tripId);
                TempData["ErrorMessage"] = "Error al cargar la selección de asientos.";
                return RedirectToAction("Index", "Trip");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReserveSeat(int seatId)
        {
            try
            {
                if (seatId <= 0)
                {
                    return Json(new { success = false, message = "ID de asiento inválido" });
                }

                // Verificar que el asiento está disponible
                var isAvailable = await _seatService.IsSeatAvailableAsync(seatId);
                if (!isAvailable)
                {
                    return Json(new { success = false, message = "El asiento no está disponible" });
                }

                // Obtener información del asiento
                var seatDto = await _seatService.GetSeatByIdAsync(seatId);
                if (seatDto == null)
                {
                    return Json(new { success = false, message = "No se encontró el asiento" });
                }

                // En una implementación real, aquí podrías implementar una reserva temporal
                // Por ahora, solo devolvemos la información del asiento

                return Json(new
                {
                    success = true,
                    message = "Asiento seleccionado correctamente",
                    seatData = new
                    {
                        id = seatDto.Id,
                        number = seatDto.SeatNumber,
                        price = seatDto.Price,
                        tripInfo = seatDto.TripInfo
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reservar el asiento {SeatId}", seatId);
                return Json(new { success = false, message = "Error interno del servidor" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(int tripId)
        {
            try
            {
                if (tripId <= 0)
                {
                    TempData["ErrorMessage"] = "ID de viaje inválido.";
                    return RedirectToAction("Index", "Trip");
                }

                var seats = await _seatService.GetSeatsByTripAsync(tripId);
                var tripDetails = await _tripService.GetTripDetailsAsync(tripId);

                var seatViewModels = seats.Select(seat => new SeatViewModel
                {
                    Id = seat.Id,
                    SeatNumber = seat.SeatNumber,
                    IsOccupied = seat.IsOccupied,
                    TripId = seat.TripId,
                    TripInfo = seat.TripInfo,
                    Price = seat.Price,
                    IsSelected = false
                }).ToList();

                ViewBag.TripInfo = tripDetails;
                ViewBag.TripId = tripId;

                return View(seatViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener asientos del viaje {TripId}", tripId);
                TempData["ErrorMessage"] = "Error al cargar los asientos.";
                return RedirectToAction("Index", "Trip");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmSelection(int seatId)
        {
            try
            {
                if (seatId <= 0)
                {
                    TempData["ErrorMessage"] = "Debe seleccionar un asiento.";
                    return RedirectToAction("Index", "Trip");
                }

                // Verificar disponibilidad una vez más
                var isAvailable = await _seatService.IsSeatAvailableAsync(seatId);
                if (!isAvailable)
                {
                    TempData["ErrorMessage"] = "El asiento seleccionado ya no está disponible.";
                    return RedirectToAction("Index", "Trip");
                }

                // Redirigir al controlador de ventas para completar la compra
                return RedirectToAction("Create", "Sale", new { seatId = seatId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar selección del asiento {SeatId}", seatId);
                TempData["ErrorMessage"] = "Error al procesar la selección de asiento.";
                return RedirectToAction("Index", "Trip");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckAvailability(int tripId)
        {
            try
            {
                var availability = await _tripService.GetSeatAvailabilityAsync(tripId);
                if (availability == null)
                {
                    return Json(new { success = false, message = "Viaje no encontrado" });
                }

                return Json(new
                {
                    success = true,
                    available = availability.AvailableCount,
                    occupied = availability.OccupiedCount,
                    total = availability.TotalCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar disponibilidad del viaje {TripId}", tripId);
                return Json(new { success = false, message = "Error interno del servidor" });
            }
        }
    }
}