using RouteService.Domain.Models;

namespace RouteService.Domain.Services
{
    public interface IRouteOptimizationService
    {
        Task<List<RouteStop>> OptimizeStopsOrderAsync(List<RouteStop> stops);
        Task<(double distance, TimeSpan duration)> CalculateRouteMetricsAsync(List<RouteStop> stops);
    }
}
