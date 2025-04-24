namespace DriverService.Application.Models
{
    public class ScheduleConflict
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Reason { get; set; }
    }
}
