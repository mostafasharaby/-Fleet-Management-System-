using Microsoft.EntityFrameworkCore;
using TelemetryService.Domain.Models;
using TelemetryService.Domain.Repositories;
using TelemetryService.Infrastructure.Data;

namespace TelemetryService.Infrastructure.Repositories
{
    public class TelemetryRepository : ITelemetryRepository
    {
        private readonly TelemetryDbContext _dbContext;

        public TelemetryRepository(TelemetryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TelemetryData> GetByIdAsync(Guid id)
        {
            return await _dbContext.TelemetryData.FindAsync(id);
        }

        public async Task<IEnumerable<TelemetryData>> GetByVehicleIdAsync(Guid vehicleId, int limit = 100)
        {
            return await _dbContext.TelemetryData
                .Where(t => t.VehicleId == vehicleId)
                .OrderByDescending(t => t.Timestamp)
                .Take(limit) // ex: if 1 => then largest timestamp
                .ToListAsync();
        }

        public async Task<IEnumerable<TelemetryData>> GetByVehicleIdTimeRangeAsync(Guid vehicleId, DateTime startTime, DateTime endTime)
        {
            return await _dbContext.TelemetryData
                .Where(t => t.VehicleId == vehicleId && t.Timestamp >= startTime && t.Timestamp <= endTime)
                .OrderBy(t => t.Timestamp)
                .ToListAsync();
        }

        public async Task AddAsync(TelemetryData telemetryData)
        {
            await _dbContext.TelemetryData.AddAsync(telemetryData);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<TelemetryData> telemetryData)
        {
            await _dbContext.TelemetryData.AddRangeAsync(telemetryData);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<TelemetryData>> GetLatestForAllVehiclesAsync()
        {
            // large timestamp for each unique vehicle ID 
            var latestTelemetryQuery = from t in _dbContext.TelemetryData
                                       group t by t.VehicleId into g
                                       select g.OrderByDescending(t => t.Timestamp).FirstOrDefault();

            return await latestTelemetryQuery.ToListAsync();
        }
    }
}
