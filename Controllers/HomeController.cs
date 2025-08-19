using Microsoft.AspNetCore.Mvc;
using TransportAgency.Models.ViewModels;
using TransportAgency.Services;

namespace TransportAgency.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITicketService _ticketService;

        public HomeController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public IActionResult Index()
        {
            var model = new TripSearchViewModel();
            ViewBag.Cities = _ticketService.GetAvailableCities();
            return View(model);
        }

        [HttpPost]
        public IActionResult SearchTrips(TripSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cities = _ticketService.GetAvailableCities();
                return View("Index", model);
            }

            if (model.TravelDate < DateTime.Today)
            {
                ModelState.AddModelError("TravelDate", "La fecha de viaje no puede ser anterior a hoy");
                ViewBag.Cities = _ticketService.GetAvailableCities();
                return View("Index", model);
            }

            var trips = _ticketService.GetAvailableTrips(model.Origin, model.Destination, model.TravelDate);

            if (!trips.Any())
            {
                TempData["ErrorMessage"] = "No hay viajes disponibles para la ruta y fecha seleccionadas.";
                ViewBag.Cities = _ticketService.GetAvailableCities();
                return View("Index", model);
            }

            ViewBag.SearchData = model;
            return View("SearchResults", trips);
        }

        public IActionResult SearchResults()
        {
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult SelectSeat(string origin, string destination, DateTime travelDate,
            string departureTime, string arrivalTime, decimal price, string duration, string busType)
        {
            var availableSeats = _ticketService.GetAvailableSeats(origin, destination, travelDate, departureTime);

            var model = new PassengerViewModel
            {
                Origin = origin,
                Destination = destination,
                TravelDate = travelDate,
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                Price = price,
                Duration = duration,
                BusType = busType
            };

            ViewBag.AvailableSeats = availableSeats;
            return View(model);
        }

        [HttpPost]
        public IActionResult ProcessPassengerData(PassengerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var availableSeats = _ticketService.GetAvailableSeats(model.Origin, model.Destination, model.TravelDate, model.DepartureTime);
                ViewBag.AvailableSeats = availableSeats;
                return View("SelectSeat", model);
            }

            return View("Confirmation", model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmPurchase(PassengerViewModel model)
        {
            try
            {
                var saleId = await _ticketService.CreateSaleAsync(model);
                TempData["SuccessMessage"] = "¡Compra realizada exitosamente!";
                return RedirectToAction("PurchaseConfirmation", new { id = saleId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al procesar la compra: " + ex.Message;
                var availableSeats = _ticketService.GetAvailableSeats(model.Origin, model.Destination, model.TravelDate, model.DepartureTime);
                ViewBag.AvailableSeats = availableSeats;
                return View("SelectSeat", model);
            }
        }

        public async Task<IActionResult> PurchaseConfirmation(int id)
        {
            var confirmation = await _ticketService.GetSaleConfirmationAsync(id);

            if (confirmation == null)
            {
                TempData["ErrorMessage"] = "No se encontró la compra especificada.";
                return RedirectToAction("Index");
            }

            return View(confirmation);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}