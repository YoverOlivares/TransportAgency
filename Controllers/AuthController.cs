using Microsoft.AspNetCore.Mvc;

namespace TransportAgency.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
