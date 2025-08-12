using TransportAgency.Models.DTOs;
using TransportAgency.Models.ViewModels;

namespace TransportAgency.Business.Interfaces
{
    public interface ISaleService
    {
        /// <summary>
        /// Procesa una nueva venta con todas las validaciones de negocio
        /// </summary>
        /// <param name="saleDto">Datos de la venta</param>
        /// <returns>Información de la venta procesada</returns>
        Task<SaleDto> ProcessSaleAsync(CreateSaleDto saleDto);

        /// <summary>
        /// Obtiene ventas por fecha específica
        /// </summary>
        /// <param name="date">Fecha de búsqueda</param>
        /// <returns>Lista de ventas en la fecha</returns>
        Task<IEnumerable<SaleDto>> GetSalesByDateAsync(DateTime date);

        /// <summary>
        /// Obtiene una venta por número de recibo
        /// </summary>
        /// <param name="receiptNumber">Número de recibo</param>
        /// <returns>Información de la venta o null si no existe</returns>
        Task<SaleDto?> GetSaleByReceiptNumberAsync(string receiptNumber);

        /// <summary>
        /// Genera un número de recibo único
        /// </summary>
        /// <returns>Número de recibo único</returns>
        Task<string> GenerateReceiptNumberAsync();

        /// <summary>
        /// Obtiene una venta por su ID
        /// </summary>
        /// <param name="saleId">ID de la venta</param>
        /// <returns>Información de la venta o null si no existe</returns>
        Task<SaleDto?> GetSaleByIdAsync(int saleId);

        /// <summary>
        /// Obtiene ventas por rango de fechas
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Lista de ventas en el rango</returns>
        Task<IEnumerable<SaleDto>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Obtiene las ventas más recientes
        /// </summary>
        /// <param name="count">Número de ventas a obtener</param>
        /// <returns>Lista de ventas recientes</returns>
        Task<IEnumerable<SaleDto>> GetRecentSalesAsync(int count = 10);

        /// <summary>
        /// Obtiene ventas por cliente específico
        /// </summary>
        /// <param name="customerId">ID del cliente</param>
        /// <returns>Lista de ventas del cliente</returns>
        Task<IEnumerable<SaleDto>> GetSalesByCustomerAsync(int customerId);

        /// <summary>
        /// Obtiene ventas por documento de cliente
        /// </summary>
        /// <param name="documentNumber">Número de documento</param>
        /// <returns>Lista de ventas del cliente</returns>
        Task<IEnumerable<SaleDto>> GetSalesByCustomerDocumentAsync(string documentNumber);

        /// <summary>
        /// Calcula el total de ingresos por fecha
        /// </summary>
        /// <param name="date">Fecha de cálculo</param>
        /// <returns>Total de ingresos</returns>
        Task<decimal> GetTotalRevenueByDateAsync(DateTime date);

        /// <summary>
        /// Calcula el total de ingresos por rango de fechas
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Total de ingresos</returns>
        Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Cuenta las ventas por fecha
        /// </summary>
        /// <param name="date">Fecha de conteo</param>
        /// <returns>Número de ventas</returns>
        Task<int> CountSalesByDateAsync(DateTime date);

        /// <summary>
        /// Cancela una venta (libera el asiento)
        /// </summary>
        /// <param name="saleId">ID de la venta</param>
        /// <returns>True si se canceló correctamente</returns>
        Task<bool> CancelSaleAsync(int saleId);

        /// <summary>
        /// Valida los datos de una venta antes de procesarla
        /// </summary>
        /// <param name="saleDto">Datos de la venta</param>
        /// <returns>Lista de errores de validación (vacía si es válida)</returns>
        Task<List<string>> ValidateSaleDataAsync(CreateSaleDto saleDto);

        /// <summary>
        /// Obtiene ViewModel de confirmación para una venta
        /// </summary>
        /// <param name="saleId">ID de la venta</param>
        /// <returns>ViewModel de confirmación</returns>
        Task<SaleConfirmationViewModel?> GetSaleConfirmationViewModelAsync(int saleId);

        /// <summary>
        /// Busca ventas por criterios específicos
        /// </summary>
        /// <param name="searchDto">Criterios de búsqueda</param>
        /// <returns>Lista de ventas que coinciden</returns>
        Task<IEnumerable<SaleDto>> SearchSalesAsync(SaleSearchDto searchDto);
    }
}