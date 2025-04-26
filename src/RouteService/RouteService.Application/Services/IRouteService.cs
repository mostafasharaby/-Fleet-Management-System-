using RouteService.Domain.Enums;
using RouteService.Domain.Models;

namespace RouteService.Application.Services
{
    public interface IRouteService
    {
        Task<Route> GetRouteAsync(Guid id);
        Task<(IEnumerable<Route> Routes, int TotalCount, int PageCount)> ListRoutesAsync(
            int pageSize, int pageNumber, string filter = null, RouteStatus? status = null);
        Task<IEnumerable<Route>> GetRoutesByVehicleIdAsync(Guid vehicleId);
        Task<IEnumerable<Route>> GetRoutesByDriverIdAsync(Guid driverId);
        Task<IEnumerable<Route>> GetRoutesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Route> CreateRouteAsync(
            string name,
            Guid vehicleId,
            Guid driverId,
            DateTime startTime,
            List<RouteStop> stops);
        Task<Route> UpdateRouteAsync(
            Guid id,
            string name,
            Guid vehicleId,
            Guid driverId,
            DateTime startTime);
        Task<Route> OptimizeRouteAsync(Guid id);
        Task<Route> StartRouteAsync(Guid id);
        Task<Route> CompleteRouteAsync(Guid id);
        Task<Route> CancelRouteAsync(Guid id, string reason);
        Task<bool> DeleteRouteAsync(Guid id);
        Task<Route> AddStopToRouteAsync(Guid routeId, RouteStop stop);
        Task<Route> UpdateStopAsync(Guid routeId, RouteStop stop);
        Task<Route> CompleteStopAsync(Guid routeId, Guid stopId, DateTime departureTime);
        Task<Route> ArriveAtStopAsync(Guid routeId, Guid stopId, DateTime arrivalTime);
        Task<Route> SkipStopAsync(Guid routeId, Guid stopId, string reason);
        Task<RouteStop> GetStopAsync(Guid stopId);
        Task<IEnumerable<RouteStop>> GetRouteStopsAsync(Guid routeId);
        Task<Route> ReorderStopsAsync(Guid routeId, IEnumerable<Guid> stopIdsInOrder);
    }
}
