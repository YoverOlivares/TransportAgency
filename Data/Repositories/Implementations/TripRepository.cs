using Microsoft.EntityFrameworkCore;
using TransportAgency.Data.Context;
using TransportAgency.Data.Repositories.Interfaces;
using TransportAgency.Models.Entities;

namespace TransportAgency.Data.Repositories.Implementations
{
    public class TripRepository : GenericRepository<Trip>, ITripRepository
    {
        public TripRepository(TransportAgencyContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Trip>> GetTripsByDateAsync(DateTime date)
        {
            try
            {
                var startDate = date.Date;
                var endDate = startDate.AddDays(1);

                return await _dbSet
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                    .Where(t => t.DepartureTime >= startDate && t.DepartureTime < endDate && t.IsActive)
                    .OrderBy(t => t.DepartureTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes por fecha {date:dd/MM/yyyy}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Trip>> GetTripsWithAvailableSeatsAsync()
        {
            try
            {
                return await _dbSet
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                    .Include(t => t.Seats)
                    .Where(t => t.IsActive && t.Seats.Any(s => !s.IsOccupied))
                    .OrderBy(t => t.DepartureTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes con asientos disponibles: {ex.Message}", ex);
            }
        }

        public async Task<Trip?> GetTripWithSeatsAsync(int tripId)
        {
            try
            {
                return await _dbSet
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                    .Include(t => t.Seats.OrderBy(s => s.SeatNumber))
                    .FirstOrDefaultAsync(t => t.Id == tripId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el viaje {tripId} con asientos: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Trip>> GetTripsWithFullInfoAsync()
        {
            try
            {
                return await _dbSet
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                    .Include(t => t.Seats)
                    .Where(t => t.IsActive)
                    .OrderBy(t => t.DepartureTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes con información completa: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Trip>> GetTripsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var start = startDate.Date;
                var end = endDate.Date.AddDays(1);

                return await _dbSet
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                    .Where(t => t.DepartureTime >= start && t.DepartureTime < end && t.IsActive)
                    .OrderBy(t => t.DepartureTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes entre {startDate:dd/MM/yyyy} y {endDate:dd/MM/yyyy}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Trip>> GetTripsByRouteAsync(int routeId)
        {
            try
            {
                return await _dbSet
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                    .Where(t => t.RouteId == routeId && t.IsActive)
                    .OrderBy(t => t.DepartureTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes de la ruta {routeId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Trip>> GetTripsByBusAsync(int busId)
        {
            try
            {
                return await _dbSet
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                    .Where(t => t.BusId == busId && t.IsActive)
                    .OrderBy(t => t.DepartureTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes del bus {busId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Trip>> GetUpcomingTripsAsync(int count = 10)
        {
            try
            {
                var now = DateTime.Now;

                return await _dbSet
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                    .Where(t => t.DepartureTime > now && t.IsActive)
                    .OrderBy(t => t.DepartureTime)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener próximos viajes: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Trip>> GetTodayTripsAsync()
        {
            try
            {
                var today = DateTime.Today;
                var tomorrow = today.AddDays(1);

                return await _dbSet
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                    .Include(t => t.Seats)
                    .Where(t => t.DepartureTime >= today && t.DepartureTime < tomorrow && t.IsActive)
                    .OrderBy(t => t.DepartureTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener viajes de hoy: {ex.Message}", ex);
            }
        }

        public override async Task<IEnumerable<Trip>> GetAllAsync()
        {
            try
            {
                return await _dbSet
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                    .OrderBy(t => t.DepartureTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todos los viajes: {ex.Message}", ex);
            }
        }

        public override async Task<Trip?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet
                    .Include(t => t.Bus)
                    .Include(t => t.Route)
                    .Include(t => t.Seats)
                    .FirstOrDefaultAsync(t => t.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el viaje con ID {id}: {ex.Message}", ex);
            }
        }
    }
}