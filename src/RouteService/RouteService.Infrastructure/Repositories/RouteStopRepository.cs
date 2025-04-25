using Microsoft.EntityFrameworkCore;
using RouteService.Domain.Models;
using RouteService.Domain.Repositories;
using RouteService.Infrastructure.Data;

namespace RouteService.Infrastructure.Repositories
{
    public class RouteStopRepository : IRouteStopRepository
    {
        private readonly RouteDbContext _dbContext;

        public RouteStopRepository(RouteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RouteStop> GetByIdAsync(Guid id)
        {
            return await _dbContext.RouteStops
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<RouteStop>> GetByRouteIdAsync(Guid routeId)
        {
            return await _dbContext.RouteStops
                .Where(s => s.RouteId == routeId)
                .OrderBy(s => s.SequenceNumber)
                .ToListAsync();
        }

        public async Task AddAsync(RouteStop stop)
        {
            await _dbContext.RouteStops.AddAsync(stop);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(RouteStop stop)
        {
            _dbContext.Entry(stop).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var stop = await _dbContext.RouteStops.FindAsync(id);
            if (stop != null)
            {
                _dbContext.RouteStops.Remove(stop);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
