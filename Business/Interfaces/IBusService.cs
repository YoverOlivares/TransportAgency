using TransportAgency.Models.DTOs;
using TransportAgency.Models.Entities;

namespace TransportAgency.Business.Interfaces
{
    public interface IBusService
    {
        /// <summary>
        /// Obtiene todos los buses activos
        /// </summary>
        /// <returns>Lista de buses activos</returns>
        Task<IEnumerable<BusDto>> GetAllActiveBusesAsync();

        /// <summary>
        /// Obtiene un bus por su ID con información completa
        /// </summary>
        /// <param name="id">ID del bus</param>
        /// <returns>Información del bus o null si no existe</returns>
        Task<BusDto?> GetBusByIdAsync(int id);

        /// <summary>
        /// Obtiene todos los buses (activos e inactivos)
        /// </summary>
        /// <returns>Lista completa de buses</returns>
        Task<IEnumerable<BusDto>> GetAllBusesAsync();

        /// <summary>
        /// Obtiene buses por rango de capacidad
        /// </summary>
        /// <param name="minCapacity">Capacidad mínima</param>
        /// <param name="maxCapacity">Capacidad máxima</param>
        /// <returns>Lista de buses filtrados</returns>
        Task<IEnumerable<BusDto>> GetBusesByCapacityAsync(int minCapacity, int maxCapacity);

        /// <summary>
        /// Obtiene un bus por su número de placa
        /// </summary>
        /// <param name="plateNumber">Número de placa</param>
        /// <returns>Información del bus o null si no existe</returns>
        Task<BusDto?> GetBusByPlateNumberAsync(string plateNumber);

        /// <summary>
        /// Verifica si un bus está disponible para un rango de fechas
        /// </summary>
        /// <param name="busId">ID del bus</param>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>True si está disponible</returns>
        Task<bool> IsBusAvailableAsync(int busId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Obtiene estadísticas de un bus específico
        /// </summary>
        /// <param name="busId">ID del bus</param>
        /// <returns>Estadísticas del bus</returns>
        Task<BusDto?> GetBusStatisticsAsync(int busId);
    }
}