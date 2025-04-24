using RouteService.Domain.Enums;

namespace RouteService.Domain.Models
{
    public class RouteStop
    {
        public Guid Id { get; private set; }
        public Guid RouteId { get; private set; }
        public int SequenceNumber { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public DateTime PlannedArrivalTime { get; private set; }
        public DateTime? ActualArrivalTime { get; private set; }
        public DateTime? DepartureTime { get; private set; }
        public StopStatus Status { get; private set; }
        public int EstimatedDurationMinutes { get; private set; }
        public string Notes { get; private set; }

        private RouteStop() { }

        public RouteStop(
            int sequenceNumber,
            string name,
            string address,
            double latitude,
            double longitude,
            DateTime plannedArrivalTime,
            int estimatedDurationMinutes,
            string notes = null)
        {
            Id = Guid.NewGuid();
            SequenceNumber = sequenceNumber;
            Name = name;
            Address = address;
            Latitude = latitude;
            Longitude = longitude;
            PlannedArrivalTime = plannedArrivalTime;
            EstimatedDurationMinutes = estimatedDurationMinutes;
            Notes = notes;
            Status = StopStatus.Planned;
        }

        public void ArriveAt(DateTime arrivalTime)
        {
            if (Status != StopStatus.Planned)
                throw new InvalidOperationException("Cannot arrive at a stop that is not in planned state");

            ActualArrivalTime = arrivalTime;
            Status = StopStatus.InProgress;
        }

        public void Complete(DateTime departureTime)
        {
            if (Status != StopStatus.InProgress)
                throw new InvalidOperationException("Cannot complete a stop that is not in progress");

            DepartureTime = departureTime;
            Status = StopStatus.Completed;
        }

        public void Skip(string reason)
        {
            if (Status == StopStatus.Completed)
                throw new InvalidOperationException("Cannot skip a completed stop");

            Status = StopStatus.Skipped;
            Notes = string.IsNullOrEmpty(Notes) ? reason : $"{Notes}\nSkipped: {reason}";
        }
    }

}
