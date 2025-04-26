using MaintenanceService.Domain.Enums;

namespace MaintenanceService.Domain.Models
{
    public class MaintenanceSubscription
    {
        public string SubscriberId { get; set; }
        public List<Guid> VehicleIds { get; set; } = new List<Guid>();
        public List<MaintenanceAlertType> AlertTypes { get; set; } = new List<MaintenanceAlertType>();
    }
}
