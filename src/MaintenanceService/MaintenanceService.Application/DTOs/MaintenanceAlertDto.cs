using MaintenanceService.Domain.Enums;
using MaintenanceService.Domain.Models;

namespace MaintenanceService.Application.DTOs
{
    public class MaintenanceAlertDto
    {
        public Guid AlertId { get; set; }
        public Guid VehicleId { get; set; }
        public MaintenanceAlertType AlertType { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public int Severity { get; set; }
        public MaintenanceTask RelatedTask { get; set; }
    }
}
