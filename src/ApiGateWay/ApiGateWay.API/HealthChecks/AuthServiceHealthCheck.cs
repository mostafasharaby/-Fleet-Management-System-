using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ApiGateWay.API.HealthChecks
{
    public class AuthServiceHealthCheck : IHealthCheck
    {
        private readonly HttpClient _httpClient;

        public AuthServiceHealthCheck()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7056"),
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
                    return HealthCheckResult.Healthy("Auth service is healthy");
                }

                return HealthCheckResult.Degraded("Auth service returned unhealthy status");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("Auth service health check failed", ex);
            }
        }
    }
}
