using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiGateWay.API.HealthChecks
{
    public class MaintenanceServiceHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;

        public MaintenanceServiceHealthCheck(HttpClient httpClienty)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7292"),
                Timeout = TimeSpan.FromSeconds(5)
            };
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _httpClient.GetAsync("health", cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    return HealthCheckResult.Healthy("Maintenance  service is healthy");
                }

                return HealthCheckResult.Degraded("Maintenance  service returned unhealthy status");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Maintenance  service health check failed", ex);
            }
        }
    }
}

