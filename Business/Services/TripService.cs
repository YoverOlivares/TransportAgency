using TransportAgency.Business.Interfaces;
using TransportAgency.Data.Repositories.Interfaces;
using TransportAgency.Models.DTOs;
using TransportAgency.Models.Entities;

namespace TransportAgency.Business.Services
{
    public class TripService : ITripService
    {
        private readonly ITripRepository _tripRepository;
        private readonly ISeatRepository _seatRepository;

        public TripService(ITripRepository tripRepository, ISeatRepository seatRepository)
        {
            _tripRepository = tripRepository ?? throw new ArgumentNullException(nameof(tripRepository));
            _seatRepository = seatRepository ?? throw new ArgumentNullException(nameof(seatRepository));
        }

        public async Task<IEnumerable<TripDto>> GetTripsByDateAsync(DateTime date)
        {
            try
            {
                var trips = await _tripRepository.GetTripsByDateAsync(date);
                var tripDtos = new List<TripDto>();

                foreach (var trip in trips)
                {
                    var tripDto = await MapToDtoWithAvailabilityAsync(trip);
                    tripDtos.Add(tripDto);
                }

                return tripDtos.OrderBy(t => t.DepartureTime);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes por fecha {date:dd/MM/yyyy}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<TripDto>> GetTripsWithAvailabilityAsync()
        {
            try
            {
                var trips = await _tripRepository.GetTripsWithAvailableSeatsAsync();
                var tripDtos = new List<TripDto>();

                foreach (var trip in trips)
                {
                    var tripDto = await MapToDtoWithAvailabilityAsync(trip);
                    if (tripDto.AvailableSeats > 0)
                    {
                        tripDtos.Add(tripDto);
                    }
                }

                return tripDtos.OrderBy(t => t.DepartureTime);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes con disponibilidad: {ex.Message}", ex);
            }
        }

        public async Task<TripDto?> GetTripDetailsAsync(int tripId)
        {
            try
            {
                if (tripId <= 0)
                    throw new ArgumentException("El ID del viaje debe ser mayor a 0", nameof(tripId));

                var trip = await _tripRepository.GetTripWithSeatsAsync(tripId);
                return trip != null ? await MapToDtoWithAvailabilityAsync(trip) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener detalles del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<TripDto>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    throw new ArgumentException("La fecha de inicio debe ser anterior o igual a la fecha de fin");

                var trips = await _tripRepository.GetTripsByDateRangeAsync(startDate, endDate);
                var tripDtos = new List<TripDto>();

                foreach (var trip in trips)
                {
                    var tripDto = await MapToDtoWithAvailabilityAsync(trip);
                    tripDtos.Add(tripDto);
                }

                return tripDtos.OrderBy(t => t.DepartureTime);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes por rango de fechas: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<TripDto>> GetUpcomingTripsAsync(int count = 10)
        {
            try
            {
                if (count <= 0)
                    count = 10;

                var trips = await _tripRepository.GetUpcomingTripsAsync(count);
                var tripDtos = new List<TripDto>();

                foreach (var trip in trips)
                {
                    var tripDto = await MapToDtoWithAvailabilityAsync(trip);
                    tripDtos.Add(tripDto);
                }

                return tripDtos;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener próximos viajes: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<TripDto>> GetTodayTripsAsync()
        {
            try
            {
                var trips = await _tripRepository.GetTodayTripsAsync();
                var tripDtos = new List<TripDto>();

                foreach (var trip in trips)
                {
                    var tripDto = await MapToDtoWithAvailabilityAsync(trip);
                    tripDtos.Add(tripDto);
                }

                return tripDtos.OrderBy(t => t.DepartureTime);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes de hoy: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<TripDto>> GetTripsByRouteAsync(int routeId)
        {
            try
            {
                if (routeId <= 0)
                    throw new ArgumentException("El ID de la ruta debe ser mayor a 0", nameof(routeId));

                var trips = await _tripRepository.GetTripsByRouteAsync(routeId);
                var tripDtos = new List<TripDto>();

                foreach (var trip in trips)
                {
                    var tripDto = await MapToDtoWithAvailabilityAsync(trip);
                    tripDtos.Add(tripDto);
                }

                return tripDtos.OrderBy(t => t.DepartureTime);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes de la ruta {routeId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<TripDto>> GetTripsByBusAsync(int busId)
        {
            try
            {
                if (busId <= 0)
                    throw new ArgumentException("El ID del bus debe ser mayor a 0", nameof(busId));

                var trips = await _tripRepository.GetTripsByBusAsync(busId);
                var tripDtos = new List<TripDto>();

                foreach (var trip in trips)
                {
                    var tripDto = await MapToDtoWithAvailabilityAsync(trip);
                    tripDtos.Add(tripDto);
                }

                return tripDtos.OrderBy(t => t.DepartureTime);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes del bus {busId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<TripDto>> SearchTripsAsync(TripSearchDto searchDto)
        {
            try
            {
                if (searchDto == null)
                    throw new ArgumentNullException(nameof(searchDto));

                var allTrips = await _tripRepository.GetTripsWithFullInfoAsync();
                var filteredTrips = allTrips.AsQueryable();

                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(searchDto.Origin))
                {
                    filteredTrips = filteredTrips.Where(t =>
                        t.Route.Origin.ToLower().Contains(searchDto.Origin.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(searchDto.Destination))
                {
                    filteredTrips = filteredTrips.Where(t =>
                        t.Route.Destination.ToLower().Contains(searchDto.Destination.ToLower()));
                }

                if (searchDto.DepartureDate.HasValue)
                {
                    var date = searchDto.DepartureDate.Value.Date;
                    filteredTrips = filteredTrips.Where(t => t.DepartureTime.Date == date);
                }

                if (searchDto.MinPrice.HasValue)
                {
                    filteredTrips = filteredTrips.Where(t => t.Price >= searchDto.MinPrice.Value);
                }

                if (searchDto.MaxPrice.HasValue)
                {
                    filteredTrips = filteredTrips.Where(t => t.Price <= searchDto.MaxPrice.Value);
                }

                var results = filteredTrips.ToList();
                var tripDtos = new List<TripDto>();

                foreach (var trip in results)
                {
                    var tripDto = await MapToDtoWithAvailabilityAsync(trip);

                    // Filtro adicional por disponibilidad
                    if (searchDto.OnlyAvailable == true && tripDto.AvailableSeats <= 0)
                        continue;

                    tripDtos.Add(tripDto);
                }

                return tripDtos.OrderBy(t => t.DepartureTime);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al buscar viajes: {ex.Message}", ex);
            }
        }

        public async Task<SeatAvailabilityDto?> GetSeatAvailabilityAsync(int tripId)
        {
            try
            {
                if (tripId <= 0)
                    throw new ArgumentException("El ID del viaje debe ser mayor a 0", nameof(tripId));

                var trip = await _tripRepository.GetTripWithSeatsAsync(tripId);
                if (trip == null)
                    return null;

                var seats = await _seatRepository.GetSeatsByTripAsync(tripId);
                var seatDtos = seats.Select(MapSeatToDto).ToList();

                return new SeatAvailabilityDto
                {
                    TripId = tripId,
                    TripInfo = $"{trip.Route.Origin} → {trip.Route.Destination} - {trip.DepartureTime:dd/MM/yyyy HH:mm}",
                    Seats = seatDtos,
                    AvailableCount = seatDtos.Count(s => !s.IsOccupied),
                    OccupiedCount = seatDtos.Count(s => s.IsOccupied),
                    TotalCount = seatDtos.Count,
                    SeatPrice = trip.Price
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener disponibilidad de asientos del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<bool> HasAvailableSeatsAsync(int tripId)
        {
            try
            {
                if (tripId <= 0)
                    return false;

                var availableCount = await _seatRepository.CountAvailableSeatsAsync(tripId);
                return availableCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar disponibilidad de asientos del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<TripDto?> GetTripOccupancyStatsAsync(int tripId)
        {
            try
            {
                if (tripId <= 0)
                    throw new ArgumentException("El ID del viaje debe ser mayor a 0", nameof(tripId));

                var trip = await _tripRepository.GetTripWithSeatsAsync(tripId);
                return trip != null ? await MapToDtoWithAvailabilityAsync(trip) : null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener estadísticas de ocupación del viaje {tripId}: {ex.Message}", ex);
            }
        }

        private async Task<TripDto> MapToDtoWithAvailabilityAsync(Trip trip)
        {
            var availableSeats = await _seatRepository.CountAvailableSeatsAsync(trip.Id);
            var totalSeats = trip.Bus.Capacity;

            return new TripDto
            {
                Id = trip.Id,
                BusModel = trip.Bus.Model,
                BusPlateNumber = trip.Bus.PlateNumber,
                BusCapacity = trip.Bus.Capacity,
                Origin = trip.Route.Origin,
                Destination = trip.Route.Destination,
                Distance = trip.Route.Distance,
                BasePrice = trip.Route.BasePrice,
                DepartureTime = trip.DepartureTime,
                ArrivalTime = trip.ArrivalTime,
                Price = trip.Price,
                AvailableSeats = availableSeats,
                TotalSeats = totalSeats,
                IsActive = trip.IsActive
            };
        }

        private static SeatDto MapSeatToDto(Seat seat)
        {
            return new SeatDto
            {
                Id = seat.Id,
                SeatNumber = seat.SeatNumber,
                IsOccupied = seat.IsOccupied,
                TripId = seat.TripId,
                TripInfo = seat.Trip != null
                    ? $"{seat.Trip.Route.Origin} → {seat.Trip.Route.Destination}"
                    : string.Empty,
                Price = seat.Trip?.Price ?? 0,
                CreatedAt = seat.CreatedAt
            };
        }
    }
}