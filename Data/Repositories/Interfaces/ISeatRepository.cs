using TransportAgency.Models.Entities;

namespace TransportAgency.Data.Repositories.Interfaces
{
    public interface ISeatRepository : IGenericRepository<Seat>
    {
        /// <summary>
        /// Obtiene todos los asientos de un viaje específico
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Lista de asientos del viaje</returns>
        Task<IEnumerable<Seat>> GetSeatsByTripAsync(int tripId);

        /// <summary>
        /// Obtiene solo los asientos disponibles de un viaje
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Lista de asientos disponibles</returns>
        Task<IEnumerable<Seat>> GetAvailableSeatsByTripAsync(int tripId);

        /// <summary>
        /// Marca un asiento como ocupado
        /// </summary>
        /// <param name="seatId">ID del asiento</param>
        /// <returns>True si se actualizó correctamente</returns>
        Task<bool> MarkSeatAsOccupiedAsync(int seatId);

        /// <summary>
        /// Marca un asiento como disponible
        /// </summary>
        /// <param name="seatId">ID del asiento</param>
        /// <returns>True si se actualizó correctamente</returns>
        Task<bool> MarkSeatAsAvailableAsync(int seatId);

        /// <summary>
        /// Obtiene un asiento por su número y viaje
        /// </summary>
        /// <param name="seatNumber">Número del asiento</param>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Asiento encontrado o null</returns>
        Task<Seat?> GetSeatByNumberAndTripAsync(string seatNumber, int tripId);

        /// <summary>
        /// Obtiene asientos con información del viaje incluida
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Lista de asientos con información del viaje</returns>
        Task<IEnumerable<Seat>> GetSeatsWithTripInfoAsync(int tripId);

        /// <summary>
        /// Cuenta los asientos disponibles en un viaje
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Número de asientos disponibles</returns>
        Task<int> CountAvailableSeatsAsync(int tripId);

        /// <summary>
        /// Cuenta los asientos ocupados en un viaje
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Número de asientos ocupados</returns>
        Task<int> CountOccupiedSeatsAsync(int tripId);

        /// <summary>
        /// Verifica si un asiento específico está disponible
        /// </summary>
        /// <param name="seatId">ID del asiento</param>
        /// <returns>True si está disponible, false si está ocupado</returns>
        Task<bool> IsSeatAvailableAsync(int seatId);

        /// <summary>
        /// Genera asientos automáticamente para un viaje basado en la capacidad del bus
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <param name="busCapacity">Capacidad del bus</param>
        /// <returns>Lista de asientos creados</returns>
        Task<IEnumerable<Seat>> GenerateSeatsForTripAsync(int tripId, int busCapacity);
    }
}