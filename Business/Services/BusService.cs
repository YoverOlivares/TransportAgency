using TransportAgency.Business.Interfaces;
using TransportAgency.Data.Repositories.Interfaces;
using TransportAgency.Models.DTOs;
using TransportAgency.Models.Entities;

namespace TransportAgency.Business.Services
{
    public class BusService : IBusService
    {
        private readonly IBusRepository _busRepository;
        private readonly ITripRepository _tripRepository;

        public BusService(IBusRepository busRepository, ITripRepository tripRepository)
        {
            _busRepository = busRepository ?? throw new ArgumentNullException(nameof(busRepository));
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
        }

        public async Task<IEnumerable<BusDto>> GetAllActiveBusesAsync()
        {
            try
            {
                var buses = await _busRepository.GetActiveBusesAsync();
                return buses.Select(MapToDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener buses activos: {ex.Message}", ex);
            }
        }

        public async Task<BusDto?> GetBusByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("El ID del bus debe ser mayor a 0", nameof(id));

                var bus = await _busRepository.GetByIdAsync(id);
                return bus != null ? MapToDto(bus) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el bus con ID {id}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BusDto>> GetAllBusesAsync()
        {
            try
            {
                var buses = await _busRepository.GetAllAsync();
                return buses.Select(MapToDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todos los buses: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<BusDto>> GetBusesByCapacityAsync(int minCapacity, int maxCapacity)
        {
            try
            {
                if (minCapacity < 0 || maxCapacity < 0 || minCapacity > maxCapacity)
                    throw new ArgumentException("Rango de capacidad inválido");

                var buses = await _busRepository.GetBusesByCapacityRangeAsync(minCapacity, maxCapacity);
                return buses.Select(MapToDto);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener buses por capacidad: {ex.Message}", ex);
            }
        }

        public async Task<BusDto?> GetBusByPlateNumberAsync(string plateNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(plateNumber))
                    throw new ArgumentException("El número de placa no puede estar vacío", nameof(plateNumber));

                var bus = await _busRepository.GetByPlateNumberAsync(plateNumber);
                return bus != null ? MapToDto(bus) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el bus con placa {plateNumber}: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsBusAvailableAsync(int busId, DateTime startDate, DateTime endDate)
        {
            try
            {
                if (busId <= 0)
                    throw new ArgumentException("El ID del bus debe ser mayor a 0", nameof(busId));

                if (startDate >= endDate)
                    throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin");

                // Verificar que el bus existe y está activo
                var bus = await _busRepository.GetByIdAsync(busId);
                if (bus == null || !bus.IsActive)
                    return false;

                // Obtener viajes del bus en el rango de fechas
                var trips = await _tripRepository.GetTripsByBusAsync(busId);
                var conflictingTrips = trips.Where(t =>
                    t.IsActive &&
                    t.DepartureTime < endDate &&
                    t.ArrivalTime > startDate);

                return !conflictingTrips.Any();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar disponibilidad del bus {busId}: {ex.Message}", ex);
            }
        }

        public async Task<BusDto?> GetBusStatisticsAsync(int busId)
        {
            try
            {
                if (busId <= 0)
                    throw new ArgumentException("El ID del bus debe ser mayor a 0", nameof(busId));

                var bus = await _busRepository.GetByIdAsync(busId);
                if (bus == null)
                    return null;

                var trips = await _tripRepository.GetTripsByBusAsync(busId);
                var activeTrips = trips.Where(t => t.IsActive);

                var busDto = MapToDto(bus);
                busDto.TotalTrips = trips.Count();
                busDto.ActiveTrips = activeTrips.Count();

                // Calcular ocupación promedio
                if (activeTrips.Any())
                {
                    var totalSeats = activeTrips.Sum(t => t.Seats.Count);
                    var occupiedSeats = activeTrips.Sum(t => t.Seats.Count(s => s.IsOccupied));
                    busDto.AverageOccupancy = totalSeats > 0 ? (decimal)occupiedSeats / totalSeats * 100 : 0;
                }

                return busDto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estadísticas del bus {busId}: {ex.Message}", ex);
            }
        }

        private static BusDto MapToDto(Bus bus)
        {
            return new BusDto
            {
                Id = bus.Id,
                PlateNumber = bus.PlateNumber,
                Model = bus.Model,
                Capacity = bus.Capacity,
                IsActive = bus.IsActive,
                CreatedAt = bus.CreatedAt,
                TotalTrips = bus.Trips?.Count ?? 0,
                ActiveTrips = bus.Trips?.Count(t => t.IsActive) ?? 0
            };
        }
    }
}