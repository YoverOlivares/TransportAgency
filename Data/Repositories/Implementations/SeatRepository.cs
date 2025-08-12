using Microsoft.EntityFrameworkCore;
using TransportAgency.Data.Context;
using TransportAgency.Data.Repositories.Interfaces;
using TransportAgency.Models.Entities;

namespace TransportAgency.Data.Repositories.Implementations
{
    public class SeatRepository : GenericRepository<Seat>, ISeatRepository
    {
        public SeatRepository(TransportAgencyContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Seat>> GetSeatsByTripAsync(int tripId)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Trip)
                    .ThenInclude(t => t.Bus)
                    .Include(s => s.Trip.Route)
                    .Where(s => s.TripId == tripId)
                    .OrderBy(s => s.SeatNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener asientos del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Seat>> GetAvailableSeatsByTripAsync(int tripId)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Trip)
                    .ThenInclude(t => t.Bus)
                    .Include(s => s.Trip.Route)
                    .Where(s => s.TripId == tripId && !s.IsOccupied)
                    .OrderBy(s => s.SeatNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener asientos disponibles del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<bool> MarkSeatAsOccupiedAsync(int seatId)
        {
            try
            {
                var seat = await _dbSet.FindAsync(seatId);
                if (seat == null)
                    return false;

                if (seat.IsOccupied)
                    throw new InvalidOperationException($"El asiento {seat.SeatNumber} ya está ocupado");

                seat.IsOccupied = true;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al marcar el asiento {seatId} como ocupado: {ex.Message}", ex);
            }
        }

        public async Task<bool> MarkSeatAsAvailableAsync(int seatId)
        {
            try
            {
                var seat = await _dbSet.FindAsync(seatId);
                if (seat == null)
                    return false;

                seat.IsOccupied = false;
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al marcar el asiento {seatId} como disponible: {ex.Message}", ex);
            }
        }

        public async Task<Seat?> GetSeatByNumberAndTripAsync(string seatNumber, int tripId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(seatNumber))
                    throw new ArgumentException("El número de asiento no puede estar vacío", nameof(seatNumber));

                return await _dbSet
                    .Include(s => s.Trip)
                    .ThenInclude(t => t.Bus)
                    .Include(s => s.Trip.Route)
                    .FirstOrDefaultAsync(s => s.SeatNumber == seatNumber && s.TripId == tripId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el asiento {seatNumber} del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Seat>> GetSeatsWithTripInfoAsync(int tripId)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Trip)
                    .ThenInclude(t => t.Bus)
                    .Include(s => s.Trip.Route)
                    .Where(s => s.TripId == tripId)
                    .OrderBy(s => s.SeatNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener asientos con información del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<int> CountAvailableSeatsAsync(int tripId)
        {
            try
            {
                return await _dbSet
                    .CountAsync(s => s.TripId == tripId && !s.IsOccupied);
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
                return await _dbSet
                    .CountAsync(s => s.TripId == tripId && s.IsOccupied);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al contar asientos ocupados del viaje {tripId}: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsSeatAvailableAsync(int seatId)
        {
            try
            {
                var seat = await _dbSet.FindAsync(seatId);
                return seat != null && !seat.IsOccupied;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar disponibilidad del asiento {seatId}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Seat>> GenerateSeatsForTripAsync(int tripId, int busCapacity)
        {
            try
            {
                if (busCapacity <= 0)
                    throw new ArgumentException("La capacidad del bus debe ser mayor a 0", nameof(busCapacity));

                // Verificar si ya existen asientos para este viaje
                var existingSeats = await _dbSet
                    .Where(s => s.TripId == tripId)
                    .ToListAsync();

                if (existingSeats.Any())
                    throw new InvalidOperationException($"Ya existen asientos para el viaje {tripId}");

                var seats = new List<Seat>();
                for (int i = 1; i <= busCapacity; i++)
                {
                    seats.Add(new Seat
                    {
                        TripId = tripId,
                        SeatNumber = i.ToString("D2"), // 01, 02, 03, etc.
                        IsOccupied = false,
                        CreatedAt = DateTime.Now
                    });
                }

                await _dbSet.AddRangeAsync(seats);
                return seats;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al generar asientos para el viaje {tripId}: {ex.Message}", ex);
            }
        }

        public override async Task<IEnumerable<Seat>> GetAllAsync()
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Trip)
                    .ThenInclude(t => t.Bus)
                    .Include(s => s.Trip.Route)
                    .OrderBy(s => s.TripId)
                    .ThenBy(s => s.SeatNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todos los asientos: {ex.Message}", ex);
            }
        }

        public override async Task<Seat?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet
                    .Include(s => s.Trip)
                    .ThenInclude(t => t.Bus)
                    .Include(s => s.Trip.Route)
                    .Include(s => s.Sales)
                    .FirstOrDefaultAsync(s => s.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el asiento con ID {id}: {ex.Message}", ex);
            }
        }
    }
}