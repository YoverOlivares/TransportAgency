using TransportAgency.Business.Interfaces;
using TransportAgency.Data.Repositories.Interfaces;
using TransportAgency.Models.DTOs;
using TransportAgency.Models.Entities;
using TransportAgency.Models.ViewModels;

namespace TransportAgency.Business.Services
{
    public class SeatService : ISeatService
    {
        private readonly ISeatRepository _seatRepository;
        private readonly ITripRepository _tripRepository;

        public SeatService(ISeatRepository seatRepository, ITripRepository tripRepository)
        {
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
        }

        public async Task<IEnumerable<SeatDto>> GetSeatsByTripAsync(int tripId)
        {
            try
            {
                if (tripId <= 0)
                    throw new ArgumentException("El ID del viaje debe ser mayor a 0", nameof(tripId));

                var seats = await _seatRepository.GetSeatsByTripAsync(tripId);
                return seats.Select(MapToDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener asientos del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SeatDto>> GetAvailableSeatsAsync(int tripId)
        {
            try
            {
                if (tripId <= 0)
                    throw new ArgumentException("El ID del viaje debe ser mayor a 0", nameof(tripId));

                var seats = await _seatRepository.GetAvailableSeatsByTripAsync(tripId);
                return seats.Select(MapToDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener asientos disponibles del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<bool> ReserveSeatAsync(int seatId)
        {
            try
            {
                if (seatId <= 0)
                    throw new ArgumentException("El ID del asiento debe ser mayor a 0", nameof(seatId));

                // Verificar que el asiento existe y está disponible
                var seat = await _seatRepository.GetByIdAsync(seatId);
                if (seat == null)
                    throw new ArgumentException($"No se encontró el asiento con ID {seatId}");

                if (seat.IsOccupied)
                    throw new InvalidOperationException($"El asiento {seat.SeatNumber} ya está ocupado");

                // Verificar que el viaje está activo
                if (!seat.Trip.IsActive)
                    throw new InvalidOperationException("No se puede reservar un asiento en un viaje inactivo");

                // Verificar que el viaje no ha partido
                if (seat.Trip.DepartureTime <= DateTime.Now)
                    throw new InvalidOperationException("No se puede reservar un asiento en un viaje que ya ha partido");

                var result = await _seatRepository.MarkSeatAsOccupiedAsync(seatId);
                if (result)
                {
                    await _seatRepository.SaveChangesAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al reservar el asiento {seatId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<SeatDto>> GenerateSeatsForTripAsync(int tripId, int capacity)
        {
            try
            {
                if (tripId <= 0)
                    throw new ArgumentException("El ID del viaje debe ser mayor a 0", nameof(tripId));

                if (capacity <= 0 || capacity > 100)
                    throw new ArgumentException("La capacidad debe estar entre 1 y 100", nameof(capacity));

                // Verificar que el viaje existe
                var trip = await _tripRepository.GetByIdAsync(tripId);
                if (trip == null)
                    throw new ArgumentException($"No se encontró el viaje con ID {tripId}");

                // Verificar que no existen asientos ya creados
                var existingSeats = await _seatRepository.GetSeatsByTripAsync(tripId);
                if (existingSeats.Any())
                    throw new InvalidOperationException($"Ya existen asientos para el viaje {tripId}");

                var seats = await _seatRepository.GenerateSeatsForTripAsync(tripId, capacity);
                await _seatRepository.SaveChangesAsync();

                return seats.Select(MapToDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar asientos para el viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<SeatDto?> GetSeatByIdAsync(int seatId)
        {
            try
            {
                if (seatId <= 0)
                    throw new ArgumentException("El ID del asiento debe ser mayor a 0", nameof(seatId));

                var seat = await _seatRepository.GetByIdAsync(seatId);
                return seat != null ? MapToDto(seat) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el asiento {seatId}: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsSeatAvailableAsync(int seatId)
        {
            try
            {
                if (seatId <= 0)
                    return false;

                return await _seatRepository.IsSeatAvailableAsync(seatId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar disponibilidad del asiento {seatId}: {ex.Message}", ex);
            }
        }

        public async Task<bool> ReleaseSeatAsync(int seatId)
        {
            try
            {
                if (seatId <= 0)
                    throw new ArgumentException("El ID del asiento debe ser mayor a 0", nameof(seatId));

                // Verificar que el asiento existe
                var seat = await _seatRepository.GetByIdAsync(seatId);
                if (seat == null)
                    throw new ArgumentException($"No se encontró el asiento con ID {seatId}");

                if (!seat.IsOccupied)
                    throw new InvalidOperationException($"El asiento {seat.SeatNumber} ya está disponible");

                var result = await _seatRepository.MarkSeatAsAvailableAsync(seatId);
                if (result)
                {
                    await _seatRepository.SaveChangesAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al liberar el asiento {seatId}: {ex.Message}", ex);
            }
        }

        public async Task<SeatDto?> GetSeatByNumberAndTripAsync(string seatNumber, int tripId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(seatNumber))
                    throw new ArgumentException("El número de asiento no puede estar vacío", nameof(seatNumber));

                if (tripId <= 0)
                    throw new ArgumentException("El ID del viaje debe ser mayor a 0", nameof(tripId));

                var seat = await _seatRepository.GetSeatByNumberAndTripAsync(seatNumber, tripId);
                return seat != null ? MapToDto(seat) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el asiento {seatNumber} del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<int> CountAvailableSeatsAsync(int tripId)
        {
            try
            {
                if (tripId <= 0)
                    throw new ArgumentException("El ID del viaje debe ser mayor a 0", nameof(tripId));

                return await _seatRepository.CountAvailableSeatsAsync(tripId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al contar asientos disponibles del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<int> CountOccupiedSeatsAsync(int tripId)
        {
            try
            {
                if (tripId <= 0)
                    throw new ArgumentException("El ID del viaje debe ser mayor a 0", nameof(tripId));

                return await _seatRepository.CountOccupiedSeatsAsync(tripId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al contar asientos ocupados del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<SeatSelectionViewModel?> GetSeatSelectionViewModelAsync(int tripId)
        {
            try
            {
                if (tripId <= 0)
                    throw new ArgumentException("El ID del viaje debe ser mayor a 0", nameof(tripId));

                var trip = await _tripRepository.GetTripWithSeatsAsync(tripId);
                if (trip == null)
                    return null;

                var seats = await _seatRepository.GetSeatsByTripAsync(tripId);
                var seatViewModels = seats.Select(seat => new SeatViewModel
                {
                    Id = seat.Id,
                    SeatNumber = seat.SeatNumber,
                    IsOccupied = seat.IsOccupied,
                    TripId = seat.TripId,
                    TripInfo = $"{trip.Route.Origin} → {trip.Route.Destination}",
                    Price = trip.Price,
                    IsSelected = false,
                    DepartureTime = trip.DepartureTime,
                    RouteInfo = $"{trip.Route.Origin} → {trip.Route.Destination}"
                }).ToList();

                var tripViewModel = new TripViewModel
                {
                    Id = trip.Id,
                    BusInfo = $"{trip.Bus.Model} - {trip.Bus.PlateNumber}",
                    RouteInfo = $"{trip.Route.Origin} → {trip.Route.Destination}",
                    DepartureTime = trip.DepartureTime,
                    ArrivalTime = trip.ArrivalTime,
                    Price = trip.Price,
                    AvailableSeats = seatViewModels.Count(s => !s.IsOccupied),
                    TotalSeats = seatViewModels.Count,
                    IsActive = trip.IsActive
                };

                return new SeatSelectionViewModel
                {
                    TripInfo = tripViewModel,
                    Seats = seatViewModels,
                    SelectedSeatId = null,
                    TotalAmount = 0
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ViewModel de selección de asientos para el viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<bool> CanGenerateSeatsForTripAsync(int tripId)
        {
            try
            {
                if (tripId <= 0)
                    return false;

                // Verificar que el viaje existe
                var trip = await _tripRepository.GetByIdAsync(tripId);
                if (trip == null || !trip.IsActive)
                    return false;

                // Verificar que no existen asientos ya creados
                var existingSeats = await _seatRepository.GetSeatsByTripAsync(tripId);
                return !existingSeats.Any();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar si se pueden generar asientos para el viaje {tripId}: {ex.Message}", ex);
            }
        }

        private static SeatDto MapToDto(Seat seat)
        {
            return new SeatDto
            {
                Id = seat.Id,
                SeatNumber = seat.SeatNumber,
                IsOccupied = seat.IsOccupied,
                TripId = seat.TripId,
                TripInfo = seat.Trip != null
                    ? $"{seat.Trip.Route?.Origin} → {seat.Trip.Route?.Destination} - {seat.Trip.DepartureTime:dd/MM/yyyy HH:mm}"
                    : string.Empty,
                Price = seat.Trip?.Price ?? 0,
                CreatedAt = seat.CreatedAt,
                TripDepartureTime = seat.Trip?.DepartureTime,
                RouteInfo = seat.Trip?.Route != null
                    ? $"{seat.Trip.Route.Origin} → {seat.Trip.Route.Destination}"
                    : string.Empty,
                BusInfo = seat.Trip?.Bus != null
                    ? $"{seat.Trip.Bus.Model} - {seat.Trip.Bus.PlateNumber}"
                    : string.Empty
            };
        }
    }
}