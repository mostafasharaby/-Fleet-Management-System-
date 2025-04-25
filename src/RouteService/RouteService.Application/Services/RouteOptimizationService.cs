using Microsoft.Extensions.Logging;
using RouteService.Domain.Models;
using RouteService.Domain.Services;

namespace RouteService.Infrastructure.Services
{
    public class RouteOptimizationService : IRouteOptimizationService
    {
        private readonly ILogger<RouteOptimizationService> _logger;

        public RouteOptimizationService(ILogger<RouteOptimizationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<RouteStop>> OptimizeStopsOrderAsync(List<RouteStop> stops)
        {
            if (stops == null || stops.Count <= 2)
            {
                _logger.LogInformation("Not enough stops to optimize, returning original order");
                return stops;
            }

            _logger.LogInformation($"Optimizing route with {stops.Count} stops");

            try
            {
                // This is a basic implementation using a greedy nearest neighbor algorithm
                // For a more advanced solution, you might use a TSP solver or external service

                // Assuming the first stop is the starting point (depot)
                var optimizedStops = new List<RouteStop>
                {
                    stops[0]
                };

                var remainingStops = new List<RouteStop>(stops.Skip(1));

                // Current position starts at the first stop
                var currentStop = optimizedStops[0];

                // While we have stops to visit
                while (remainingStops.Count > 0)
                {
                    // Find the nearest unvisited stop
                    var nearestStop = FindNearestStop(currentStop, remainingStops);

                    // Add it to our optimized route
                    optimizedStops.Add(nearestStop);

                    // Remove it from the remaining stops
                    remainingStops.Remove(nearestStop);

                    // Update current position
                    currentStop = nearestStop;
                }

                // Reset sequence numbers
                for (int i = 0; i < optimizedStops.Count; i++)
                {
                    // Create a new RouteStop with updated sequence number
                    // Note: In a real application, you'd need a more robust way to update the sequence
                    // without creating new objects, possibly through a method on the RouteStop class
                    var stop = optimizedStops[i];
                    var updatedStop = new RouteStop(
                        i + 1,  // New sequence number
                        stop.Name,
                        stop.Address,
                        stop.Latitude,
                        stop.Longitude,
                        stop.PlannedArrivalTime,
                        stop.EstimatedDurationMinutes,
                        stop.Notes
                    );

                    // For simplicity, we're not handling the ID or RouteID here
                    // In a real implementation, you would preserve these values
                    optimizedStops[i] = updatedStop;
                }

                _logger.LogInformation("Route optimization completed successfully");
                return optimizedStops;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error optimizing route stops");
                throw;
            }
        }

        public async Task<(double distance, TimeSpan duration)> CalculateRouteMetricsAsync(List<RouteStop> stops)
        {
            if (stops == null || stops.Count < 2)
            {
                _logger.LogInformation("Not enough stops to calculate metrics");
                return (0, TimeSpan.Zero);
            }

            try
            {
                double totalDistance = 0;
                TimeSpan totalDuration = TimeSpan.Zero;

                // Calculate distance between consecutive stops
                for (int i = 0; i < stops.Count - 1; i++)
                {
                    var currentStop = stops[i];
                    var nextStop = stops[i + 1];

                    // Calculate distance using Haversine formula
                    double distance = CalculateDistance(
                        currentStop.Latitude, currentStop.Longitude,
                        nextStop.Latitude, nextStop.Longitude);

                    totalDistance += distance;

                    // Estimate driving time based on average speed of 50 km/h
                    double hoursTaken = distance / 50.0;
                    totalDuration += TimeSpan.FromHours(hoursTaken);

                    // Add time spent at the stop
                    totalDuration += TimeSpan.FromMinutes(nextStop.EstimatedDurationMinutes);
                }

                _logger.LogInformation($"Route metrics calculated: {totalDistance:F2} km, {totalDuration:hh\\:mm}");
                return (totalDistance, totalDuration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating route metrics");
                throw;
            }
        }

        private RouteStop FindNearestStop(RouteStop current, List<RouteStop> candidateStops)
        {
            double shortestDistance = double.MaxValue;
            RouteStop nearestStop = null;

            foreach (var stop in candidateStops)
            {
                double distance = CalculateDistance(
                    current.Latitude, current.Longitude,
                    stop.Latitude, stop.Longitude);

                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    nearestStop = stop;
                }
            }

            return nearestStop;
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double earthRadius = 6371; // Earth radius in kilometers

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return earthRadius * c;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}