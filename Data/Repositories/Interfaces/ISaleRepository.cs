using TransportAgency.Models.Entities;

namespace TransportAgency.Data.Repositories.Interfaces
{
    public interface ISaleRepository : IGenericRepository<Sale>
    {
        /// <summary>
        /// Obtiene ventas por fecha específica
        /// </summary>
        /// <param name="date">Fecha de búsqueda</param>
        /// <returns>Lista de ventas en la fecha</returns>
        Task<IEnumerable<Sale>> GetSalesByDateAsync(DateTime date);

        /// <summary>
        /// Obtiene una venta por número de recibo
        /// </summary>
        /// <param name="receiptNumber">Número de recibo</param>
        /// <returns>Venta encontrada o null</returns>
        Task<Sale?> GetSaleByReceiptNumberAsync(string receiptNumber);

        /// <summary>
        /// Obtiene ventas con información completa (Customer, Seat, Trip)
        /// </summary>
        /// <returns>Lista de ventas con información completa</returns>
        Task<IEnumerable<Sale>> GetSalesWithFullInfoAsync();

        /// <summary>
        /// Obtiene ventas por rango de fechas
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Lista de ventas en el rango</returns>
        Task<IEnumerable<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Obtiene ventas por cliente específico
        /// </summary>
        /// <param name="customerId">ID del cliente</param>
        /// <returns>Lista de ventas del cliente</returns>
        Task<IEnumerable<Sale>> GetSalesByCustomerAsync(int customerId);

        /// <summary>
        /// Obtiene ventas por documento de cliente
        /// </summary>
        /// <param name="documentNumber">Número de documento</param>
        /// <returns>Lista de ventas del cliente</returns>
        Task<IEnumerable<Sale>> GetSalesByCustomerDocumentAsync(string documentNumber);

        /// <summary>
        /// Obtiene las ventas más recientes
        /// </summary>
        /// <param name="count">Número de ventas a obtener</param>
        /// <returns>Lista de ventas recientes</returns>
        Task<IEnumerable<Sale>> GetRecentSalesAsync(int count = 10);

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
        /// Verifica si existe una venta con el número de recibo especificado
        /// </summary>
        /// <param name="receiptNumber">Número de recibo</param>
        /// <param name="excludeId">ID a excluir de la verificación (para updates)</param>
        /// <returns>True si existe, false si no</returns>
        Task<bool> ReceiptNumberExistsAsync(string receiptNumber, int? excludeId = null);

        /// <summary>
        /// Genera un número de recibo único
        /// </summary>
        /// <returns>Número de recibo único</returns>
        Task<string> GenerateUniqueReceiptNumberAsync();
    }
}