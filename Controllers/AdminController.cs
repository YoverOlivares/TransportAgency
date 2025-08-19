using Microsoft.AspNetCore.Mvc;
using TransportAgency.Models.ViewModels;
using TransportAgency.Services;

namespace TransportAgency.Controllers
{
    public class AdminController : Controller
    {
        private readonly ITicketService _ticketService;

        public AdminController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public IActionResult Login()
        {
            if (IsLoggedIn())
                return RedirectToAction("Dashboard");

            return View(new AdminLoginViewModel());
        }

        [HttpPost]
        public IActionResult Login(AdminLoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Credenciales simples para demo
            if (model.Username == "admin" && model.Password == "admin123")
            {
                HttpContext.Session.SetString("AdminLoggedIn", "true");
                HttpContext.Session.SetString("AdminUsername", model.Username);
                TempData["SuccessMessage"] = "Bienvenido al panel administrativo";
                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Usuario o contraseña incorrectos");
            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["InfoMessage"] = "Sesión cerrada correctamente";
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login");

            var dashboardData = await _ticketService.GetDashboardDataAsync();
            return View(dashboardData);
        }

        public async Task<IActionResult> Sales()
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login");

            var sales = await _ticketService.GetAllSalesAsync();
            return View(sales);
        }

        public async Task<IActionResult> EditSale(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login");

            var sale = await _ticketService.GetSaleByIdAsync(id);
            if (sale == null)
            {
                TempData["ErrorMessage"] = "Venta no encontrada";
                return RedirectToAction("Sales");
            }

            var model = new EditSaleViewModel
            {
                Id = sale.Id,
                PassengerName = sale.Customer.FullName,
                DocumentNumber = sale.Customer.DocumentNumber,
                Phone = sale.Customer.Phone,
                Email = sale.Customer.Email,
                Origin = sale.Origin,
                Destination = sale.Destination,
                TravelDate = sale.TravelDate,
                SeatNumber = sale.SeatNumber,
                Price = sale.Price,
                TravelTime = sale.TravelTime,
                TicketNumber = sale.TicketNumber
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditSale(EditSaleViewModel model)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
                return View(model);

            var success = await _ticketService.UpdateSaleAsync(model);

            if (success)
            {
                TempData["SuccessMessage"] = "Venta actualizada correctamente";
                return RedirectToAction("Sales");
            }

            TempData["ErrorMessage"] = "Error al actualizar la venta";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSale(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login");

            var success = await _ticketService.DeleteSaleAsync(id);

            if (success)
                TempData["SuccessMessage"] = "Venta eliminada correctamente";
            else
                TempData["ErrorMessage"] = "Error al eliminar la venta";

            return RedirectToAction("Sales");
        }

        public async Task<IActionResult> ViewSale(int id)
        {
            if (!IsLoggedIn())
                return RedirectToAction("Login");

            var confirmation = await _ticketService.GetSaleConfirmationAsync(id);

            if (confirmation == null)
            {
                TempData["ErrorMessage"] = "Venta no encontrada";
                return RedirectToAction("Sales");
            }

            return View(confirmation);
        }

        private bool IsLoggedIn()
        {
            return HttpContext.Session.GetString("AdminLoggedIn") == "true";
        }
    }
}
