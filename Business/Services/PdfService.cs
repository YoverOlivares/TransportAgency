using iText.Kernel.Pdf;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.Layout.Properties;
using Document = iText.Layout.Document;
using Table = iText.Layout.Element.Table;
using Paragraph = iText.Layout.Element.Paragraph;
using Cell = iText.Layout.Element.Cell;
using TransportAgency.Business.Interfaces;
using TransportAgency.Data.Repositories.Interfaces;
using TransportAgency.Models.DTOs;
using TransportAgency.Models.Entities;

namespace TransportAgency.Business.Services
{
    public class PdfService : IPdfService
    {
        private readonly ISaleRepository _saleRepository;

        public PdfService(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
        }

        public async Task<byte[]> GenerateReceiptPdfAsync(Sale sale)
        {
            try
            {
                var receiptDto = new ReceiptDto
                {
                    ReceiptNumber = sale.ReceiptNumber,
                    SaleDate = sale.SaleDate,
                    Amount = sale.Amount,
                    SeatNumber = sale.Seat.SeatNumber,
                    CustomerInfo = new CustomerInfo
                    {
                        FullName = sale.Customer.FullName,
                        DocumentNumber = sale.Customer.DocumentNumber,
                        Phone = sale.Customer.Phone,
                        Email = sale.Customer.Email
                    },
                    TripInfo = new TripInfo
                    {
                        Origin = sale.Seat.Trip.Route.Origin,
                        Destination = sale.Seat.Trip.Route.Destination,
                        DepartureDate = sale.Seat.Trip.DepartureTime.Date,
                        DepartureTime = sale.Seat.Trip.DepartureTime,
                        ArrivalTime = sale.Seat.Trip.ArrivalTime
                    },
                    BusInfo = new BusInfo
                    {
                        Model = sale.Seat.Trip.Bus.Model,
                        PlateNumber = sale.Seat.Trip.Bus.PlateNumber,
                        Capacity = sale.Seat.Trip.Bus.Capacity
                    },
                    CompanyInfo = new CompanyInfo()
                };

                return await GenerateReceiptPdfAsync(receiptDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar PDF del recibo: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> GenerateReceiptPdfAsync(int saleId)
        {
            try
            {
                var sale = await _saleRepository.GetByIdAsync(saleId);
                if (sale == null)
                    throw new ArgumentException($"No se encontró la venta con ID {saleId}");

                return await GenerateReceiptPdfAsync(sale);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar PDF para la venta {saleId}: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> GenerateReceiptPdfAsync(ReceiptDto receiptDto)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using var pdfWriter = new PdfWriter(memoryStream);
                using var pdfDocument = new PdfDocument(pdfWriter);
                using var document = new Document(pdfDocument);

                var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // Header de empresa
                document.Add(new Paragraph(receiptDto.CompanyInfo.CompanyName)
                    .SetFont(boldFont)
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.CENTER));

                document.Add(new Paragraph($"RUC: {receiptDto.CompanyInfo.Ruc} | Tel: {receiptDto.CompanyInfo.Phone}")
                    .SetFont(regularFont)
                    .SetFontSize(10)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // Título
                document.Add(new Paragraph("BOLETO DE VIAJE")
                    .SetFont(boldFont)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // Información principal
                var infoTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2 }))
                    .SetWidth(UnitValue.CreatePercentValue(100));

                AddTableRow(infoTable, "Recibo N°:", receiptDto.ReceiptNumber, boldFont, regularFont);
                AddTableRow(infoTable, "Fecha:", receiptDto.FormattedSaleDate, boldFont, regularFont);
                AddTableRow(infoTable, "Pasajero:", receiptDto.CustomerInfo.FullName, boldFont, regularFont);
                AddTableRow(infoTable, "Documento:", receiptDto.CustomerInfo.DocumentNumber, boldFont, regularFont);
                AddTableRow(infoTable, "Ruta:", receiptDto.FullTripInfo, boldFont, regularFont);
                AddTableRow(infoTable, "Fecha Viaje:", receiptDto.TripDateTime, boldFont, regularFont);
                AddTableRow(infoTable, "Bus:", receiptDto.BusInfo.FullInfo, boldFont, regularFont);
                AddTableRow(infoTable, "Asiento:", receiptDto.SeatNumber, boldFont, regularFont);
                AddTableRow(infoTable, "Monto:", receiptDto.FormattedAmount, boldFont, regularFont);

                document.Add(infoTable);

                // Footer
                document.Add(new Paragraph("¡Gracias por viajar con nosotros!")
                    .SetFont(regularFont)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(30));

                document.Close();
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar PDF: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> GenerateSalesReportPdfAsync(DateTime date)
        {
            try
            {
                var sales = await _saleRepository.GetSalesByDateAsync(date);
                var totalRevenue = await _saleRepository.GetTotalRevenueByDateAsync(date);

                using var memoryStream = new MemoryStream();
                using var pdfWriter = new PdfWriter(memoryStream);
                using var pdfDocument = new PdfDocument(pdfWriter);
                using var document = new Document(pdfDocument);

                var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                var regularFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // Título
                document.Add(new Paragraph($"REPORTE DE VENTAS - {date:dd/MM/yyyy}")
                    .SetFont(boldFont)
                    .SetFontSize(16)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // Resumen
                document.Add(new Paragraph($"Total Ventas: {sales.Count()} | Ingresos: {totalRevenue:C}")
                    .SetFont(boldFont)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // Tabla de ventas
                if (sales.Any())
                {
                    var table = new Table(UnitValue.CreatePercentArray(new float[] { 2, 2, 1, 1 }))
                        .SetWidth(UnitValue.CreatePercentValue(100));

                    table.AddHeaderCell(new Cell().Add(new Paragraph("Cliente").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Ruta").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Asiento").SetFont(boldFont)));
                    table.AddHeaderCell(new Cell().Add(new Paragraph("Monto").SetFont(boldFont)));

                    foreach (var sale in sales)
                    {
                        table.AddCell(new Cell().Add(new Paragraph(sale.Customer.FullName).SetFont(regularFont)));
                        table.AddCell(new Cell().Add(new Paragraph($"{sale.Seat.Trip.Route.Origin} → {sale.Seat.Trip.Route.Destination}").SetFont(regularFont)));
                        table.AddCell(new Cell().Add(new Paragraph(sale.Seat.SeatNumber).SetFont(regularFont)));
                        table.AddCell(new Cell().Add(new Paragraph(sale.Amount.ToString("C")).SetFont(regularFont)));
                    }

                    document.Add(table);
                }
                else
                {
                    document.Add(new Paragraph("No se encontraron ventas para esta fecha.")
                        .SetFont(regularFont)
                        .SetTextAlignment(TextAlignment.CENTER));
                }

                document.Close();
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar reporte: {ex.Message}", ex);
            }
        }

        public async Task<byte[]> GenerateSalesReportPdfAsync(DateTime startDate, DateTime endDate)
        {
            return await GenerateSalesReportPdfAsync(startDate); // Implementación básica
        }

        public async Task<byte[]> GeneratePassengerListPdfAsync(int tripId)
        {
            throw new NotImplementedException("Funcionalidad en desarrollo");
        }

        public async Task<byte[]> GenerateBusOccupancyReportPdfAsync(DateTime date)
        {
            throw new NotImplementedException("Funcionalidad en desarrollo");
        }

        public string GetReceiptFileName(string receiptNumber)
        {
            return $"Recibo_{receiptNumber.Replace("/", "-")}_{DateTime.Now:yyyyMMdd}.pdf";
        }

        public string GetReportFileName(string reportType, DateTime date)
        {
            return $"Reporte_{reportType}_{date:yyyyMMdd}.pdf";
        }

        public async Task<bool> ValidatePdfGenerationAsync()
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using var pdfWriter = new PdfWriter(memoryStream);
                using var pdfDocument = new PdfDocument(pdfWriter);
                using var document = new Document(pdfDocument);
                document.Add(new Paragraph("Test"));
                document.Close();
                return memoryStream.Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private void AddTableRow(Table table, string label, string value, PdfFont boldFont, PdfFont regularFont)
        {
            table.AddCell(new Cell().Add(new Paragraph(label).SetFont(boldFont)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
            table.AddCell(new Cell().Add(new Paragraph(value).SetFont(regularFont)).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
        }
    }
}