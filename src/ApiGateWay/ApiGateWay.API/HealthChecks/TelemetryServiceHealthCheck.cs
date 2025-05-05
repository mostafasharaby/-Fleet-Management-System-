using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiGateWay.API.HealthChecks
{
    public class TelemetryServiceHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;

        public TelemetryServiceHealthCheck()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7280"),
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
                    return HealthCheckResult.Healthy("Telemetry service is healthy");
                }

                return HealthCheckResult.Degraded("Telemetry service returned unhealthy status");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Telemetry service health check failed", ex);
            }
        }
    }
}
