using RouteService.Domain.Enums;
using RouteService.Domain.Models;

namespace RouteService.Domain.Repositories
{
    public interface IRouteRepository
    {
        Task<Route> GetByIdAsync(Guid id);
        Task<IEnumerable<Route>> GetAllAsync();
        Task<IEnumerable<Route>> GetByVehicleIdAsync(Guid vehicleId);
        Task<IEnumerable<Route>> GetByDriverIdAsync(Guid driverId);
        Task<IEnumerable<Route>> GetByStatusAsync(RouteStatus status);
        Task<IEnumerable<Route>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task AddAsync(Route route);
        Task UpdateAsync(Route route);
        Task DeleteAsync(Guid id);
    }
}
