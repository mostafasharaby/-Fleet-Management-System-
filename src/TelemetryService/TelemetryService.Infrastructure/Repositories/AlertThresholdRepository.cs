using Microsoft.EntityFrameworkCore;
using TelemetryService.Domain.Models;
using TelemetryService.Domain.Repositories;
using TelemetryService.Infrastructure.Data;

namespace TelemetryService.Infrastructure.Repositories
{
    public class AlertThresholdRepository : IAlertThresholdRepository
    {
        private readonly TelemetryDbContext _dbContext;

        public AlertThresholdRepository(TelemetryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<AlertThreshold>> GetByVehicleIdAsync(Guid vehicleId)
        {
            return await _dbContext.AlertThresholds
                .Where(t => t.VehicleId == vehicleId)
                .ToListAsync();
        }

        public async Task<AlertThreshold> GetByIdAsync(Guid id)
        {
            return await _dbContext.AlertThresholds.FindAsync(id);
        }

        public async Task AddAsync(AlertThreshold alertThreshold)
        {
            await _dbContext.AlertThresholds.AddAsync(alertThreshold);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(AlertThreshold alertThreshold)
        {
            _dbContext.AlertThresholds.Update(alertThreshold);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var alertThreshold = await _dbContext.AlertThresholds.FindAsync(id);
            if (alertThreshold != null)
            {
                _dbContext.AlertThresholds.Remove(alertThreshold);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
