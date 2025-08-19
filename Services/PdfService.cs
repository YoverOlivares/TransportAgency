using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TransportAgency.Services;

namespace TransportAgency.Services
{
    public class PdfService : IPdfService
    {
        private readonly ITicketService _ticketService;

        public PdfService(ITicketService ticketService)
        {
            _ticketService = ticketService;

            // Configuración requerida para QuestPDF (Community License)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GenerateTicketPdfAsync(int saleId)
        {
            var confirmation = await _ticketService.GetSaleConfirmationAsync(saleId);
            if (confirmation == null)
                throw new ArgumentException($"Sale with ID {saleId} not found");

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            // Encabezado
                            column.Item().AlignCenter().Text("TRANSPORT AGENCY")
                                .FontSize(20).Bold();

                            column.Item().AlignCenter().Text("BOLETO DE VIAJE")
                                .FontSize(16).Bold();

                            column.Item().PaddingVertical(5);

                            // Información del boleto
                            column.Item().Text($"Número de Boleto: {confirmation.TicketNumber}")
                                .FontSize(12).Bold();
                            column.Item().Text($"Fecha de Compra: {confirmation.PurchaseDateFormatted}")
                                .FontSize(10);

                            // Línea separadora
                            column.Item().PaddingVertical(10).LineHorizontal(1);

                            // Información del pasajero
                            column.Item().Text("INFORMACIÓN DEL PASAJERO")
                                .FontSize(14).Bold();

                            column.Item().PaddingVertical(5);

                            column.Item().Text($"Nombre: {confirmation.PassengerName}");
                            column.Item().Text($"Documento: {confirmation.DocumentNumber}");

                            if (!string.IsNullOrEmpty(confirmation.Phone))
                                column.Item().Text($"Teléfono: {confirmation.Phone}");

                            if (!string.IsNullOrEmpty(confirmation.Email))
                                column.Item().Text($"Email: {confirmation.Email}");

                            // Línea separadora
                            column.Item().PaddingVertical(10).LineHorizontal(1);

                            // Información del viaje
                            column.Item().Text("INFORMACIÓN DEL VIAJE")
                                .FontSize(14).Bold();

                            column.Item().PaddingVertical(5);

                            column.Item().AlignCenter().Text($"{confirmation.Origin} → {confirmation.Destination}")
                                .FontSize(16).Bold();

                            column.Item().PaddingVertical(5);

                            column.Item().Text($"Fecha de Viaje: {confirmation.TravelDateFormatted}");
                            column.Item().Text($"Hora de Salida: {confirmation.DepartureTime}");
                            column.Item().Text($"Hora de Llegada: {confirmation.ArrivalTime}");
                            column.Item().Text($"Asiento: {confirmation.SeatNumber}").Bold();
                            column.Item().Text($"Tipo de Bus: {confirmation.BusType}");
                            column.Item().Text($"Duración: {confirmation.Duration}");

                            // Línea separadora
                            column.Item().PaddingVertical(10).LineHorizontal(1);

                            // Precio
                            column.Item().AlignCenter().Text($"PRECIO TOTAL: {confirmation.PriceFormatted}")
                                .FontSize(18).Bold();

                            // Línea separadora
                            column.Item().PaddingVertical(10).LineHorizontal(1);

                            // Instrucciones
                            column.Item().AlignCenter().Text("INSTRUCCIONES IMPORTANTES")
                                .FontSize(12).Bold();

                            column.Item().PaddingVertical(5);

                            column.Item().Text("• Presente este boleto 30 minutos antes de la salida");
                            column.Item().Text("• Conserve este documento durante todo el viaje");
                            column.Item().Text("• Verifique sus datos antes del viaje");
                            column.Item().Text("• En caso de dudas, comuníquese con nosotros");

                            // Información de contacto
                            column.Item().PaddingVertical(10);
                            column.Item().Text("Contacto: +51 123 456 789 | info@transportagency.com")
                                .FontSize(9).AlignCenter();

                            // Footer
                            column.Item().PaddingVertical(10);
                            column.Item().AlignCenter().Text("¡Buen viaje!")
                                .FontSize(14).Bold();
                            column.Item().AlignCenter().Text("Gracias por elegir Transport Agency")
                                .FontSize(10);
                        });
                });
            });

            return document.GeneratePdf();
        }
    }
}