using TransportAgency.Models.Entities;

namespace TransportAgency.Data.Repositories.Interfaces
{
    public interface IBusRepository : IGenericRepository<Bus>
    {
        /// <summary>
        /// Obtiene todos los buses activos
        /// </summary>
        /// <returns>Lista de buses activos</returns>
        Task<IEnumerable<Bus>> GetActiveBusesAsync();

        /// <summary>
        /// Obtiene un bus por su placa
        /// </summary>
        /// <param name="plateNumber">Número de placa</param>
        /// <returns>Bus encontrado o null</returns>
        Task<Bus?> GetByPlateNumberAsync(string plateNumber);

        /// <summary>
        /// Obtiene buses con sus viajes incluidos
        /// </summary>
        /// <returns>Lista de buses con viajes</returns>
        Task<IEnumerable<Bus>> GetBusesWithTripsAsync();

        /// <summary>
        /// Obtiene buses activos con capacidad específica
        /// </summary>
        /// <param name="minCapacity">Capacidad mínima</param>
        /// <param name="maxCapacity">Capacidad máxima</param>
        /// <returns>Lista de buses filtrados</returns>
        Task<IEnumerable<Bus>> GetBusesByCapacityRangeAsync(int minCapacity, int maxCapacity);

        /// <summary>
        /// Verifica si existe un bus con la placa especificada
        /// </summary>
        /// <param name="plateNumber">Número de placa</param>
        /// <param name="excludeId">ID a excluir de la verificación (para updates)</param>
        /// <returns>True si existe, false si no</returns>
        Task<bool> PlateNumberExistsAsync(string plateNumber, int? excludeId = null);
    }
}