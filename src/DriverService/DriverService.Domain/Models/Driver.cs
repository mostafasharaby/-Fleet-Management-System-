using DriverService.Domain.Enums;

namespace DriverService.Domain.Models
{
    public class Driver
    {
        public Guid Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string LicenseNumber { get; private set; }
        public string LicenseState { get; private set; }
        public DateTime LicenseExpiry { get; private set; }
        public DriverStatus Status { get; private set; }
        public DriverType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public List<Assignment> Assignments { get; private set; } = new List<Assignment>();
        public List<ScheduleEntry> Schedule { get; private set; } = new List<ScheduleEntry>();

        private Driver() { }

        public static Driver Create(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string licenseNumber,
            string licenseState,
            DateTime licenseExpiry,
            DriverType type)
        {
            var driver = new Driver
            {
                Id = Guid.NewGuid(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                LicenseNumber = licenseNumber,
                LicenseState = licenseState,
                LicenseExpiry = licenseExpiry,
                Status = DriverStatus.ACTIVE,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return driver;
        }

        public void Update(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string licenseNumber,
            string licenseState,
            DateTime licenseExpiry,
            DriverStatus status,
            DriverType type)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            LicenseNumber = licenseNumber;
            LicenseState = licenseState;
            LicenseExpiry = licenseExpiry;
            Status = status;
            Type = type;
            UpdatedAt = DateTime.UtcNow;
        }

        public Assignment AssignVehicle(
            Guid vehicleId,
            DateTime startTime,
            DateTime endTime,
            string notes)
        {
            if (Status != DriverStatus.ACTIVE)
            {
                throw new InvalidOperationException($"Cannot assign vehicle to driver with status {Status}");
            }

            // Check for overlapping assignments
            foreach (var assignment in Assignments)
            {
                if (assignment.Status == AssignmentStatus.PENDING || assignment.Status == AssignmentStatus.IN_PROGRESS)
                {
                    if (assignment.StartTime < endTime && startTime < assignment.EndTime)
                    {
                        throw new InvalidOperationException("Driver already has an overlapping assignment");
                    }
                }
            }

            var newAssignment = new Assignment
            {
                Id = Guid.NewGuid(),
                DriverId = Id,
                VehicleId = vehicleId,
                StartTime = startTime,
                EndTime = endTime,
                Notes = notes,
                Status = AssignmentStatus.PENDING,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            Assignments.Add(newAssignment);
            UpdatedAt = DateTime.UtcNow;

            return newAssignment;
        }

        public Assignment CompleteAssignment(
            Guid assignmentId,
            float finalOdometer,
            float fuelLevel,
            string notes)
        {
            var assignment = Assignments.Find(a => a.Id == assignmentId);
            if (assignment == null)
            {
                throw new InvalidOperationException($"Assignment with ID {assignmentId} not found");
            }

            if (assignment.Status != AssignmentStatus.IN_PROGRESS)
            {
                throw new InvalidOperationException($"Cannot complete assignment with status {assignment.Status}");
            }

            assignment.Status = AssignmentStatus.COMPLETED;
            assignment.EndOdometer = finalOdometer;
            assignment.FuelLevel = fuelLevel;
            assignment.Notes = string.IsNullOrEmpty(notes) ? assignment.Notes : notes;
            assignment.UpdatedAt = DateTime.UtcNow;

            UpdatedAt = DateTime.UtcNow;
            return assignment;
        }

        public void SetStatus(DriverStatus status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
        }

        public List<ScheduleEntry> AddScheduleEntries(List<ScheduleEntry> entries)
        {
            foreach (var entry in entries)
            {
                // Check for overlapping schedule entries
                foreach (var existingEntry in Schedule)
                {
                    if (entry.StartTime < existingEntry.EndTime && existingEntry.StartTime < entry.EndTime)
                    {
                        throw new InvalidOperationException($"Schedule entry overlaps with existing entry from {existingEntry.StartTime} to {existingEntry.EndTime}");
                    }
                }

                // Check for overlapping assignments
                foreach (var assignment in Assignments)
                {
                    if (assignment.Status == AssignmentStatus.PENDING || assignment.Status == AssignmentStatus.IN_PROGRESS)
                    {
                        if (entry.StartTime < assignment.EndTime && assignment.StartTime < entry.EndTime)
                        {
                            throw new InvalidOperationException($"Schedule entry overlaps with assignment from {assignment.StartTime} to {assignment.EndTime}");
                        }
                    }
                }

                entry.Id = Guid.NewGuid();
                entry.DriverId = Id;
                entry.CreatedAt = DateTime.UtcNow;
                Schedule.Add(entry);
            }

            UpdatedAt = DateTime.UtcNow;
            return entries;
        }

        public List<(DateTime Start, DateTime End, bool IsAvailable, Guid? AssignmentId)> GetAvailability(
            DateTime fromDate,
            DateTime toDate)
        {
            var result = new List<(DateTime Start, DateTime End, bool IsAvailable, Guid? AssignmentId)>();

            // Start with the entire period being available
            var currentPeriod = (Start: fromDate, End: toDate, IsAvailable: true, AssignmentId: (Guid?)null);
            result.Add(currentPeriod);

            // Process schedule entries
            foreach (var entry in Schedule)
            {
                if (entry.EndTime <= fromDate || entry.StartTime >= toDate)
                    continue;

                // Adjust result with this unavailable period
                result = AdjustAvailabilityWithPeriod(result, entry.StartTime, entry.EndTime, false, null);
            }

            // Process assignments
            foreach (var assignment in Assignments)
            {
                if (assignment.Status == AssignmentStatus.CANCELLED)
                    continue;

                if (assignment.EndTime <= fromDate || assignment.StartTime >= toDate)
                    continue;

                // Adjust result with this unavailable period
                result = AdjustAvailabilityWithPeriod(result, assignment.StartTime, assignment.EndTime, false, assignment.Id);
            }

            return result;
        }

        private List<(DateTime Start, DateTime End, bool IsAvailable, Guid? AssignmentId)> AdjustAvailabilityWithPeriod(
            List<(DateTime Start, DateTime End, bool IsAvailable, Guid? AssignmentId)> availabilityPeriods,
            DateTime startTime,
            DateTime endTime,
            bool isAvailable,
            Guid? assignmentId)
        {
            var result = new List<(DateTime Start, DateTime End, bool IsAvailable, Guid? AssignmentId)>();

            foreach (var period in availabilityPeriods)
            {
                // If this period doesn't overlap with the new period, keep it as is
                if (period.End <= startTime || period.Start >= endTime)
                {
                    result.Add(period);
                    continue;
                }

                // If this period is already unavailable, keep it as is
                if (!period.IsAvailable)
                {
                    result.Add(period);
                    continue;
                }

                // This period overlaps with the new period and is currently available

                // Add the part before the new period, if any
                if (period.Start < startTime)
                {
                    result.Add((period.Start, startTime, true, null));
                }

                // Add the overlapping part with the new availability
                var overlapStart = period.Start > startTime ? period.Start : startTime;
                var overlapEnd = period.End < endTime ? period.End : endTime;
                result.Add((overlapStart, overlapEnd, isAvailable, assignmentId));

                // Add the part after the new period, if any
                if (period.End > endTime)
                {
                    result.Add((endTime, period.End, true, null));
                }
            }

            return result;
        }

        public string FullName => $"{FirstName} {LastName}";
    }

}
