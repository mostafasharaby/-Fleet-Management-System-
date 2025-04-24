using RouteService.Domain.Enums;

namespace RouteService.Domain.Models
{
    public class Route
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public Guid VehicleId { get; private set; }
        public Guid DriverId { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public RouteStatus Status { get; private set; }
        public List<RouteStop> Stops { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public double TotalDistance { get; private set; } // in kilometers
        public TimeSpan EstimatedDuration { get; private set; }

        private Route()
        {
            Stops = new List<RouteStop>();
        }

        public Route(string name, Guid vehicleId, Guid driverId, DateTime startTime, IEnumerable<RouteStop> stops)
        {
            Id = Guid.NewGuid();
            Name = name;
            VehicleId = vehicleId;
            DriverId = driverId;
            StartTime = startTime;
            Status = RouteStatus.Planned;
            Stops = new List<RouteStop>(stops);
            CreatedAt = DateTime.UtcNow;

            CalculateRouteMetrics();
        }

        public void StartRoute()
        {
            if (Status != RouteStatus.Planned)
                throw new InvalidOperationException("Cannot start a route that is not in planned state");

            Status = RouteStatus.InProgress;
            UpdatedAt = DateTime.UtcNow;
        }

        public void CompleteRoute()
        {
            if (Status != RouteStatus.InProgress)
                throw new InvalidOperationException("Cannot complete a route that is not in progress");

            Status = RouteStatus.Completed;
            EndTime = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void CancelRoute(string reason)
        {
            if (Status == RouteStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed route");

            Status = RouteStatus.Cancelled;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddStop(RouteStop stop)
        {
            if (Status != RouteStatus.Planned)
                throw new InvalidOperationException("Cannot modify stops for a route that is not in planning state");

            Stops.Add(stop);
            UpdatedAt = DateTime.UtcNow;
            CalculateRouteMetrics();
        }

        public void UpdateStop(RouteStop updatedStop)
        {
            var existingStop = Stops.Find(s => s.Id == updatedStop.Id);
            if (existingStop == null)
                throw new InvalidOperationException("Stop not found in route");

            // Update stop properties
            var index = Stops.IndexOf(existingStop);
            Stops[index] = updatedStop;

            UpdatedAt = DateTime.UtcNow;
            CalculateRouteMetrics();
        }

        public void OptimizeRoute()
        {
            if (Status != RouteStatus.Planned)
                throw new InvalidOperationException("Cannot optimize a route that is not in planning state");

            // In a real implementation, this would use a route optimization algorithm
            // For now, we'll just update the timestamp
            UpdatedAt = DateTime.UtcNow;
            CalculateRouteMetrics();
        }

        private void CalculateRouteMetrics()
        {
            // In a real implementation, this would calculate actual distance and duration
            // based on the stops and routing algorithms
            TotalDistance = 0;
            EstimatedDuration = TimeSpan.Zero;

            if (Stops.Count < 2)
                return;

            for (int i = 0; i < Stops.Count - 1; i++)
            {
                var currentStop = Stops[i];
                var nextStop = Stops[i + 1];

                // Calculate distance between stops using the Haversine formula
                double distance = CalculateDistance(
                    currentStop.Latitude, currentStop.Longitude,
                    nextStop.Latitude, nextStop.Longitude);

                TotalDistance += distance;

                // Estimate duration based on average speed of 50 km/h
                double hoursTaken = distance / 50.0;
                EstimatedDuration += TimeSpan.FromHours(hoursTaken);

                // Add estimated time at each stop
                EstimatedDuration += TimeSpan.FromMinutes(nextStop.EstimatedDurationMinutes);
            }
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
