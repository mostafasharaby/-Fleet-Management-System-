using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiGateWay.API.HealthChecks
{
    public class RouteServiceHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;

        public RouteServiceHealthCheck()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7183"),
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
                    return HealthCheckResult.Healthy("Route service is healthy");
                }

                return HealthCheckResult.Degraded("Route service returned unhealthy status");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Route service health check failed", ex);
            }
        }
    }
}
