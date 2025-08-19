using Microsoft.AspNetCore.Mvc;
using TransportAgency.Services;

namespace TransportAgency.Controllers
{
    public class TicketController : Controller
    {
        private readonly IPdfService _pdfService;

        public TicketController(IPdfService pdfService)
        {
            _pdfService = pdfService;
        }

        public async Task<IActionResult> DownloadPdf(int id)
        {
            try
            {
                var pdfBytes = await _pdfService.GenerateTicketPdfAsync(id);
                return File(pdfBytes, "application/pdf", $"Boleto_{id}_{DateTime.Now:yyyyMMdd}.pdf");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al generar el PDF del boleto: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}