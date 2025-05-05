using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiGateWay.API.HealthChecks
{
    public class VehicleServiceHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;

        public VehicleServiceHealthCheck(HttpClient httpClienty)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7206"),
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
                    return HealthCheckResult.Healthy("Vehicle service is healthy");
                }

                return HealthCheckResult.Degraded("Vehicle service returned unhealthy status");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Vehicle service health check failed", ex);
            }
        }
    }
}

