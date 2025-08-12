using TransportAgency.Models.DTOs;
using TransportAgency.Models.Entities;

namespace TransportAgency.Business.Interfaces
{
    public interface IPdfService
    {
        /// <summary>
        /// Genera un PDF del recibo/boleto para una venta
        /// </summary>
        /// <param name="sale">Entidad de venta</param>
        /// <returns>Array de bytes del PDF generado</returns>
        Task<byte[]> GenerateReceiptPdfAsync(Sale sale);

        /// <summary>
        /// Genera un PDF del recibo/boleto por ID de venta
        /// </summary>
        /// <param name="saleId">ID de la venta</param>
        /// <returns>Array de bytes del PDF generado</returns>
        Task<byte[]> GenerateReceiptPdfAsync(int saleId);

        /// <summary>
        /// Genera un PDF del recibo usando DTO
        /// </summary>
        /// <param name="receiptDto">Datos del recibo</param>
        /// <returns>Array de bytes del PDF generado</returns>
        Task<byte[]> GenerateReceiptPdfAsync(ReceiptDto receiptDto);

        /// <summary>
        /// Genera un reporte PDF de ventas por fecha
        /// </summary>
        /// <param name="date">Fecha del reporte</param>
        /// <returns>Array de bytes del PDF generado</returns>
        Task<byte[]> GenerateSalesReportPdfAsync(DateTime date);

        /// <summary>
        /// Genera un reporte PDF de ventas por rango de fechas
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Array de bytes del PDF generado</returns>
        Task<byte[]> GenerateSalesReportPdfAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Genera un PDF con la lista de pasajeros de un viaje
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Array de bytes del PDF generado</returns>
        Task<byte[]> GeneratePassengerListPdfAsync(int tripId);

        /// <summary>
        /// Genera un PDF con estadísticas de ocupación de buses
        /// </summary>
        /// <param name="date">Fecha de las estadísticas</param>
        /// <returns>Array de bytes del PDF generado</returns>
        Task<byte[]> GenerateBusOccupancyReportPdfAsync(DateTime date);

        /// <summary>
        /// Obtiene el nombre de archivo sugerido para un recibo
        /// </summary>
        /// <param name="receiptNumber">Número de recibo</param>
        /// <returns>Nombre de archivo sugerido</returns>
        string GetReceiptFileName(string receiptNumber);

        /// <summary>
        /// Obtiene el nombre de archivo sugerido para un reporte
        /// </summary>
        /// <param name="reportType">Tipo de reporte</param>
        /// <param name="date">Fecha del reporte</param>
        /// <returns>Nombre de archivo sugerido</returns>
        string GetReportFileName(string reportType, DateTime date);

        /// <summary>
        /// Valida que se puedan generar PDFs (configuración, dependencias, etc.)
        /// </summary>
        /// <returns>True si está configurado correctamente</returns>
        Task<bool> ValidatePdfGenerationAsync();
    }
}