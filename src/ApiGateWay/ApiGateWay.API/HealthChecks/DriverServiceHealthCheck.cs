using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiGateWay.API.HealthChecks
{
    public class DriverServiceHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;

        public DriverServiceHealthCheck(HttpClient httpClienty)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7240"),
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
                    return HealthCheckResult.Healthy("Driver  service is healthy");
                }

                return HealthCheckResult.Degraded("Driver  service returned unhealthy status");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Driver  service health check failed", ex);
            }
        }
    }
}

