namespace MaintenanceService.Domain.Models
{
    public class MaintenanceReportResult
    {
        public bool Success { get; set; }
        public string ReportUrl { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
