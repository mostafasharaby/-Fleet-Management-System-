using Microsoft.EntityFrameworkCore;
using RouteService.Domain.Enums;
using RouteService.Domain.Models;
using RouteService.Domain.Repositories;
using RouteService.Infrastructure.Data;

namespace RouteService.Infrastructure.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        private readonly RouteDbContext _dbContext;

        public RouteRepository(RouteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Route> GetByIdAsync(Guid id)
        {
            return await _dbContext.Routes
                .Include(r => r.Stops)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Route>> GetAllAsync()
        {
            return await _dbContext.Routes
                .Include(r => r.Stops)
                .ToListAsync();
        }

        public async Task<IEnumerable<Route>> GetByVehicleIdAsync(Guid vehicleId)
        {
            return await _dbContext.Routes
                .Include(r => r.Stops)
                .Where(r => r.VehicleId == vehicleId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Route>> GetByDriverIdAsync(Guid driverId)
        {
            return await _dbContext.Routes
                .Include(r => r.Stops)
                .Where(r => r.DriverId == driverId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Route>> GetByStatusAsync(RouteStatus status)
        {
            return await _dbContext.Routes
                .Include(r => r.Stops)
                .Where(r => r.Status == status)
                .ToListAsync();
        }

        public async Task<IEnumerable<Route>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Routes
                .Include(r => r.Stops)
                .Where(r => r.StartTime >= startDate && r.StartTime <= endDate)
                .ToListAsync();
        }

        public async Task AddAsync(Route route)
        {
            await _dbContext.Routes.AddAsync(route);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Route route)
        {
            _dbContext.Routes.Update(route);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var route = await GetByIdAsync(id);
            if (route != null)
            {
                _dbContext.Routes.Remove(route);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
