using TransportAgency.Models.Entities;

namespace TransportAgency.Data.Repositories.Interfaces
{
    public interface ITripRepository : IGenericRepository<Trip>
    {
        /// <summary>
        /// Obtiene viajes por fecha específica
        /// </summary>
        /// <param name="date">Fecha de búsqueda</param>
        /// <returns>Lista de viajes en la fecha</returns>
        Task<IEnumerable<Trip>> GetTripsByDateAsync(DateTime date);

        /// <summary>
        /// Obtiene viajes que tienen asientos disponibles
        /// </summary>
        /// <returns>Lista de viajes con asientos disponibles</returns>
        Task<IEnumerable<Trip>> GetTripsWithAvailableSeatsAsync();

        /// <summary>
        /// Obtiene un viaje con todos sus asientos incluidos
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Viaje con asientos o null</returns>
        Task<Trip?> GetTripWithSeatsAsync(int tripId);

        /// <summary>
        /// Obtiene viajes con información completa (Bus, Route, Seats)
        /// </summary>
        /// <returns>Lista de viajes con información completa</returns>
        Task<IEnumerable<Trip>> GetTripsWithFullInfoAsync();

        /// <summary>
        /// Obtiene viajes por rango de fechas
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Lista de viajes en el rango</returns>
        Task<IEnumerable<Trip>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Obtiene viajes por ruta específica
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <returns>Lista de viajes de la ruta</returns>
        Task<IEnumerable<Trip>> GetTripsByRouteAsync(int routeId);

        /// <summary>
        /// Obtiene viajes por bus específico
        /// </summary>
        /// <param name="busId">ID del bus</param>
        /// <returns>Lista de viajes del bus</returns>
        Task<IEnumerable<Trip>> GetTripsByBusAsync(int busId);

        /// <summary>
        /// Obtiene próximos viajes activos
        /// </summary>
        /// <param name="count">Número de viajes a obtener</param>
        /// <returns>Lista de próximos viajes</returns>
        Task<IEnumerable<Trip>> GetUpcomingTripsAsync(int count = 10);

        /// <summary>
        /// Obtiene viajes activos para el día de hoy
        /// </summary>
        /// <returns>Lista de viajes de hoy</returns>
        Task<IEnumerable<Trip>> GetTodayTripsAsync();
    }
}