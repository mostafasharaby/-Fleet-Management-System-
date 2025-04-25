using RouteService.Domain.Models;

namespace RouteService.Domain.Repositories
{
    public interface IRouteStopRepository
    {
        Task<RouteStop> GetByIdAsync(Guid id);
        Task<IEnumerable<RouteStop>> GetByRouteIdAsync(Guid routeId);
        Task AddAsync(RouteStop stop);
        Task UpdateAsync(RouteStop stop);
        Task DeleteAsync(Guid id);
    }
}
