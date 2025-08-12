using Microsoft.AspNetCore.Mvc;
using TransportAgency.Business.Interfaces;
using TransportAgency.Models.DTOs;
using TransportAgency.Models.ViewModels;

namespace TransportAgency.Controllers
{
    public class SaleController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly ISeatService _seatService;
        private readonly IPdfService _pdfService;
        private readonly ILogger<SaleController> _logger;

        public SaleController(
            ISaleService saleService,
            ISeatService seatService,
            IPdfService pdfService,
            ILogger<SaleController> logger)
        {
            _saleService = saleService ?? throw new ArgumentNullException(nameof(saleService));
            _seatService = seatService ?? throw new ArgumentNullException(nameof(seatService));
            _pdfService = pdfService ?? throw new ArgumentNullException(nameof(pdfService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var todaySales = await _saleService.GetSalesByDateAsync(DateTime.Today);

                var saleViewModels = todaySales.Select(sale => new SaleViewModel
                {
                    Id = sale.Id,
                    CustomerName = sale.CustomerName,
                    DocumentNumber = sale.DocumentNumber,
                    SeatNumber = sale.SeatNumber,
                    TripInfo = sale.TripInfo,
                    Amount = sale.Amount,
                    SaleDate = sale.SaleDate,
                    ReceiptNumber = sale.ReceiptNumber,
                    CustomerPhone = sale.CustomerPhone,
                    CustomerEmail = sale.CustomerEmail,
                    TripDepartureTime = sale.TripDepartureTime,
                    BusInfo = sale.BusInfo
                }).ToList();

                return View(saleViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las ventas del día");
                TempData["ErrorMessage"] = "Error al cargar las ventas.";
                return View(new List<SaleViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create(int seatId)
        {
            try
            {
                if (seatId <= 0)
                {
                    TempData["ErrorMessage"] = "Debe seleccionar un asiento válido.";
                    return RedirectToAction("Index", "Trip");
                }

                // Verificar que el asiento existe y está disponible
                var seatDto = await _seatService.GetSeatByIdAsync(seatId);
                if (seatDto == null)
                {
                    TempData["ErrorMessage"] = "El asiento seleccionado no existe.";
                    return RedirectToAction("Index", "Trip");
                }

                if (seatDto.IsOccupied)
                {
                    TempData["ErrorMessage"] = "El asiento seleccionado ya está ocupado.";
                    return RedirectToAction("SelectSeat", "Seat", new { tripId = seatDto.TripId });
                }

                // Crear ViewModel con información del asiento
                var viewModel = new CreateSaleViewModel
                {
                    SeatId = seatDto.Id,
                    SeatNumber = seatDto.SeatNumber,
                    TripInfo = seatDto.TripInfo,
                    Amount = seatDto.Price,
                    SelectedSeatViewModel = new SeatViewModel
                    {
                        Id = seatDto.Id,
                        SeatNumber = seatDto.SeatNumber,
                        IsOccupied = seatDto.IsOccupied,
                        TripId = seatDto.TripId,
                        TripInfo = seatDto.TripInfo,
                        Price = seatDto.Price
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario de venta para el asiento {SeatId}", seatId);
                TempData["ErrorMessage"] = "Error al cargar el formulario de venta.";
                return RedirectToAction("Index", "Trip");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSaleViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Recargar información del asiento si el modelo no es válido
                    var seatDto = await _seatService.GetSeatByIdAsync(model.SeatId);
                    if (seatDto != null)
                    {
                        model.SeatNumber = seatDto.SeatNumber;
                        model.TripInfo = seatDto.TripInfo;
                        model.Amount = seatDto.Price;
                    }

                    return View(model);
                }

                // Crear DTO para procesar la venta
                var createSaleDto = new CreateSaleDto
                {
                    CustomerName = model.FullName,
                    DocumentNumber = model.DocumentNumber,
                    Phone = model.Phone,
                    Email = model.Email,
                    SeatId = model.SeatId,
                    Amount = model.Amount
                };

                // Procesar la venta
                var saleDto = await _saleService.ProcessSaleAsync(createSaleDto);

                TempData["SuccessMessage"] = "¡Venta procesada exitosamente!";
                return RedirectToAction(nameof(Confirmation), new { saleId = saleDto.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la venta");

                string errorMessage = ex.Message.Contains("validación") || ex.Message.Contains("ocupado") || ex.Message.Contains("inactivo")
                    ? ex.Message
                    : "Error al procesar la venta. Inténtelo nuevamente.";

                ModelState.AddModelError("", errorMessage);

                // Recargar información del asiento
                var seatDto = await _seatService.GetSeatByIdAsync(model.SeatId);
                if (seatDto != null)
                {
                    model.SeatNumber = seatDto.SeatNumber;
                    model.TripInfo = seatDto.TripInfo;
                    model.Amount = seatDto.Price;
                }

                return View(model);
            }
        }

        public async Task<IActionResult> Confirmation(int saleId)
        {
            try
            {
                if (saleId <= 0)
                {
                    TempData["ErrorMessage"] = "ID de venta inválido.";
                    return RedirectToAction(nameof(Index));
                }

                var confirmationViewModel = await _saleService.GetSaleConfirmationViewModelAsync(saleId);
                if (confirmationViewModel == null)
                {
                    TempData["ErrorMessage"] = "No se encontró la venta especificada.";
                    return RedirectToAction(nameof(Index));
                }

                return View(confirmationViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al mostrar confirmación de venta {SaleId}", saleId);
                TempData["ErrorMessage"] = "Error al cargar la confirmación de venta.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> DownloadReceipt(int saleId)
        {
            try
            {
                if (saleId <= 0)
                {
                    TempData["ErrorMessage"] = "ID de venta inválido.";
                    return RedirectToAction(nameof(Index));
                }

                var saleDto = await _saleService.GetSaleByIdAsync(saleId);
                if (saleDto == null)
                {
                    TempData["ErrorMessage"] = "No se encontró la venta especificada.";
                    return RedirectToAction(nameof(Index));
                }

                // Generar PDF
                var pdfBytes = await _pdfService.GenerateReceiptPdfAsync(saleId);

                // Crear nombre de archivo
                var fileName = _pdfService.GetReceiptFileName(saleDto.ReceiptNumber);

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar PDF del recibo para la venta {SaleId}", saleId);
                TempData["ErrorMessage"] = "Error al generar el comprobante PDF.";
                return RedirectToAction(nameof(Confirmation), new { saleId });
            }
        }

        public async Task<IActionResult> Receipt(string receiptNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(receiptNumber))
                {
                    TempData["ErrorMessage"] = "Número de recibo inválido.";
                    return RedirectToAction(nameof(Index));
                }

                var saleDto = await _saleService.GetSaleByReceiptNumberAsync(receiptNumber);
                if (saleDto == null)
                {
                    TempData["ErrorMessage"] = "No se encontró un recibo con ese número.";
                    return RedirectToAction(nameof(Index));
                }

                // Crear ViewModel para mostrar el recibo
                var receiptViewModel = new SaleConfirmationViewModel
                {
                    SaleId = saleDto.Id,
                    ReceiptNumber = saleDto.ReceiptNumber,
                    CustomerName = saleDto.CustomerName,
                    TripInfo = saleDto.TripInfo,
                    SeatNumber = saleDto.SeatNumber,
                    Amount = saleDto.Amount,
                    SaleDate = saleDto.SaleDate,
                    Success = true,
                    Message = "Información del recibo",
                    DocumentNumber = saleDto.DocumentNumber,
                    TripDepartureTime = saleDto.TripDepartureTime,
                    BusInfo = saleDto.BusInfo
                };

                return View("Confirmation", receiptViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar el recibo {ReceiptNumber}", receiptNumber);
                TempData["ErrorMessage"] = "Error al buscar el recibo.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Search(string receiptNumber, string customerDocument, DateTime? saleDate)
        {
            try
            {
                var searchDto = new SaleSearchDto
                {
                    ReceiptNumber = receiptNumber,
                    DocumentNumber = customerDocument,
                    StartDate = saleDate,
                    EndDate = saleDate
                };

                var searchResults = await _saleService.SearchSalesAsync(searchDto);

                var saleViewModels = searchResults.Select(sale => new SaleViewModel
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

                ViewBag.SearchPerformed = true;
                ViewBag.SearchReceiptNumber = receiptNumber;
                ViewBag.SearchCustomerDocument = customerDocument;
                ViewBag.SearchSaleDate = saleDate;

                return View("Index", saleViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar ventas");
                TempData["ErrorMessage"] = "Error al realizar la búsqueda.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Success()
        {
            return View();
        }
    }
}