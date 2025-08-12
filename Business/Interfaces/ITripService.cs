using TransportAgency.Models.DTOs;
using TransportAgency.Models.ViewModels;

namespace TransportAgency.Business.Interfaces
{
    public interface ITripService
    {
        /// <summary>
        /// Obtiene viajes por fecha específica
        /// </summary>
        /// <param name="date">Fecha de búsqueda</param>
        /// <returns>Lista de viajes en la fecha</returns>
        Task<IEnumerable<TripDto>> GetTripsByDateAsync(DateTime date);

        /// <summary>
        /// Obtiene viajes que tienen asientos disponibles
        /// </summary>
        /// <returns>Lista de viajes con disponibilidad</returns>
        Task<IEnumerable<TripDto>> GetTripsWithAvailabilityAsync();

        /// <summary>
        /// Obtiene detalles completos de un viaje específico
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Detalles del viaje o null si no existe</returns>
        Task<TripDto?> GetTripDetailsAsync(int tripId);

        /// <summary>
        /// Obtiene viajes por rango de fechas
        /// </summary>
        /// <param name="startDate">Fecha de inicio</param>
        /// <param name="endDate">Fecha de fin</param>
        /// <returns>Lista de viajes en el rango</returns>
        Task<IEnumerable<TripDto>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Obtiene próximos viajes disponibles
        /// </summary>
        /// <param name="count">Número de viajes a obtener</param>
        /// <returns>Lista de próximos viajes</returns>
        Task<IEnumerable<TripDto>> GetUpcomingTripsAsync(int count = 10);

        /// <summary>
        /// Obtiene viajes de hoy
        /// </summary>
        /// <returns>Lista de viajes de hoy</returns>
        Task<IEnumerable<TripDto>> GetTodayTripsAsync();

        /// <summary>
        /// Obtiene viajes por ruta específica
        /// </summary>
        /// <param name="routeId">ID de la ruta</param>
        /// <returns>Lista de viajes de la ruta</returns>
        Task<IEnumerable<TripDto>> GetTripsByRouteAsync(int routeId);

        /// <summary>
        /// Obtiene viajes por bus específico
        /// </summary>
        /// <param name="busId">ID del bus</param>
        /// <returns>Lista de viajes del bus</returns>
        Task<IEnumerable<TripDto>> GetTripsByBusAsync(int busId);

        /// <summary>
        /// Busca viajes por criterios específicos
        /// </summary>
        /// <param name="searchDto">Criterios de búsqueda</param>
        /// <returns>Lista de viajes que coinciden</returns>
        Task<IEnumerable<TripDto>> SearchTripsAsync(TripSearchDto searchDto);

        /// <summary>
        /// Obtiene disponibilidad de asientos para un viaje
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Información de disponibilidad</returns>
        Task<SeatAvailabilityDto?> GetSeatAvailabilityAsync(int tripId);

        /// <summary>
        /// Verifica si un viaje tiene asientos disponibles
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>True si tiene asientos disponibles</returns>
        Task<bool> HasAvailableSeatsAsync(int tripId);

        /// <summary>
        /// Calcula estadísticas de ocupación de un viaje
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Estadísticas de ocupación</returns>
        Task<TripDto?> GetTripOccupancyStatsAsync(int tripId);
    }
}