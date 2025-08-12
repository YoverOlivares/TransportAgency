using Microsoft.EntityFrameworkCore;
using TransportAgency.Data.Context;
using TransportAgency.Data.Repositories.Interfaces;
using TransportAgency.Models.Entities;

namespace TransportAgency.Data.Repositories.Implementations
{
    public class BusRepository : GenericRepository<Bus>, IBusRepository
    {
        public BusRepository(TransportAgencyContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Bus>> GetActiveBusesAsync()
        {
            try
            {
                return await _dbSet
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.PlateNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener buses activos: {ex.Message}", ex);
            }
        }

        public async Task<Bus?> GetByPlateNumberAsync(string plateNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(plateNumber))
                    throw new ArgumentException("El número de placa no puede estar vacío", nameof(plateNumber));

                return await _dbSet
                    .FirstOrDefaultAsync(b => b.PlateNumber.ToLower() == plateNumber.ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el bus con placa {plateNumber}: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Bus>> GetBusesWithTripsAsync()
        {
            try
            {
                return await _dbSet
                    .Include(b => b.Trips)
                    .ThenInclude(t => t.Route)
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.PlateNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener buses con viajes: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<Bus>> GetBusesByCapacityRangeAsync(int minCapacity, int maxCapacity)
        {
            try
            {
                if (minCapacity < 0 || maxCapacity < 0 || minCapacity > maxCapacity)
                    throw new ArgumentException("Rango de capacidad inválido");

                return await _dbSet
                    .Where(b => b.IsActive && b.Capacity >= minCapacity && b.Capacity <= maxCapacity)
                    .OrderBy(b => b.Capacity)
                    .ThenBy(b => b.PlateNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener buses por capacidad: {ex.Message}", ex);
            }
        }

        public async Task<bool> PlateNumberExistsAsync(string plateNumber, int? excludeId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(plateNumber))
                    return false;

                var query = _dbSet.Where(b => b.PlateNumber.ToLower() == plateNumber.ToLower());

                if (excludeId.HasValue)
                {
                    query = query.Where(b => b.Id != excludeId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al verificar la existencia de la placa {plateNumber}: {ex.Message}", ex);
            }
        }

        public override async Task<IEnumerable<Bus>> GetAllAsync()
        {
            try
            {
                return await _dbSet
                    .OrderBy(b => b.PlateNumber)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener todos los buses: {ex.Message}", ex);
            }
        }

        public override async Task<Bus?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet
                    .Include(b => b.Trips)
                    .FirstOrDefaultAsync(b => b.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el bus con ID {id}: {ex.Message}", ex);
            }
        }
    }
}