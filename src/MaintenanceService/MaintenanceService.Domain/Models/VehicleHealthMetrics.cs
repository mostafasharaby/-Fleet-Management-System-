namespace MaintenanceService.Domain.Models
{
    public class VehicleHealthMetrics
    {
        public Guid VehicleId { get; set; }
        public double OverallHealthScore { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDue { get; set; }
        public int MaintenanceEventsCount { get; set; }
        public List<ComponentHealthMetric> ComponentHealths { get; set; } = new List<ComponentHealthMetric>();
    }

    public class ComponentHealthMetric
    {
        public string ComponentName { get; set; }
        public double HealthScore { get; set; }
        public string Status { get; set; }
        public DateTime LastChecked { get; set; }
    }
}
