using TransportAgency.Models.DTOs;
using TransportAgency.Models.ViewModels;

namespace TransportAgency.Business.Interfaces
{
    public interface ISeatService
    {
        /// <summary>
        /// Obtiene todos los asientos de un viaje específico
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Lista de asientos del viaje</returns>
        Task<IEnumerable<SeatDto>> GetSeatsByTripAsync(int tripId);

        /// <summary>
        /// Obtiene solo los asientos disponibles de un viaje
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Lista de asientos disponibles</returns>
        Task<IEnumerable<SeatDto>> GetAvailableSeatsAsync(int tripId);

        /// <summary>
        /// Reserva un asiento específico
        /// </summary>
        /// <param name="seatId">ID del asiento</param>
        /// <returns>True si se reservó correctamente</returns>
        Task<bool> ReserveSeatAsync(int seatId);

        /// <summary>
        /// Genera asientos automáticamente para un viaje
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <param name="capacity">Capacidad del bus</param>
        /// <returns>Lista de asientos generados</returns>
        Task<IEnumerable<SeatDto>> GenerateSeatsForTripAsync(int tripId, int capacity);

        /// <summary>
        /// Obtiene información de un asiento específico
        /// </summary>
        /// <param name="seatId">ID del asiento</param>
        /// <returns>Información del asiento o null si no existe</returns>
        Task<SeatDto?> GetSeatByIdAsync(int seatId);

        /// <summary>
        /// Verifica si un asiento está disponible
        /// </summary>
        /// <param name="seatId">ID del asiento</param>
        /// <returns>True si está disponible</returns>
        Task<bool> IsSeatAvailableAsync(int seatId);

        /// <summary>
        /// Libera un asiento (marca como disponible)
        /// </summary>
        /// <param name="seatId">ID del asiento</param>
        /// <returns>True si se liberó correctamente</returns>
        Task<bool> ReleaseSeatAsync(int seatId);

        /// <summary>
        /// Obtiene asiento por número y viaje
        /// </summary>
        /// <param name="seatNumber">Número del asiento</param>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>Información del asiento o null si no existe</returns>
        Task<SeatDto?> GetSeatByNumberAndTripAsync(string seatNumber, int tripId);

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
        /// Obtiene información de selección de asientos para la UI
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>ViewModel para selección de asientos</returns>
        Task<SeatSelectionViewModel?> GetSeatSelectionViewModelAsync(int tripId);

        /// <summary>
        /// Valida que se puedan generar asientos para un viaje
        /// </summary>
        /// <param name="tripId">ID del viaje</param>
        /// <returns>True si es válido generar asientos</returns>
        Task<bool> CanGenerateSeatsForTripAsync(int tripId);
    }
}