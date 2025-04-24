using DriverService.Domain.Enums;

namespace DriverService.Domain.Models
{
    public class ScheduleEntry
    {
        public Guid Id { get; set; }
        public Guid DriverId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ScheduleType Type { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
