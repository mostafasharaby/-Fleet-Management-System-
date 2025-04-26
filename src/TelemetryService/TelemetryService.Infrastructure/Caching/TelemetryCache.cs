using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TelemetryService.Domain.Models;

namespace TelemetryService.Infrastructure.Caching
{
    public class TelemetryCache : ITelemetryCache
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<TelemetryCache> _logger;
        private readonly DistributedCacheEntryOptions _cacheOptions;

        public TelemetryCache(
            IDistributedCache distributedCache,
            ILogger<TelemetryCache> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
            _cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };
        }

        public async Task<TelemetryData> GetLatestTelemetryDataAsync(Guid vehicleId)
        {
            try
            {
                var cacheKey = $"telemetry:latest:{vehicleId}";
                var cachedData = await _distributedCache.GetStringAsync(cacheKey);

                if (cachedData != null)
                {
                    return JsonSerializer.Deserialize<TelemetryData>(cachedData);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving telemetry data from cache for vehicle {VehicleId}", vehicleId);
                return null;
            }
        }

        public async Task CacheTelemetryDataAsync(TelemetryData telemetryData)
        {
            try
            {
                var cacheKey = $"telemetry:latest:{telemetryData.VehicleId}";
                var serializedData = JsonSerializer.Serialize(telemetryData);

                await _distributedCache.SetStringAsync(
                    cacheKey,
                    serializedData,
                    _cacheOptions);

                _logger.LogDebug("Cached telemetry data for vehicle {VehicleId}", telemetryData.VehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching telemetry data for vehicle {VehicleId}", telemetryData.VehicleId);
            }
        }

        public async Task CacheAlertThresholdsAsync(Guid vehicleId, IEnumerable<AlertThreshold> thresholds)
        {
            try
            {
                var cacheKey = $"telemetry:thresholds:{vehicleId}";
                var serializedData = JsonSerializer.Serialize(thresholds);

                await _distributedCache.SetStringAsync(
                    cacheKey,
                    serializedData,
                    _cacheOptions);

                _logger.LogDebug("Cached {Count} alert thresholds for vehicle {VehicleId}",
                    ((List<AlertThreshold>)thresholds).Count, vehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error caching alert thresholds for vehicle {VehicleId}", vehicleId);
            }
        }

        public async Task<IEnumerable<AlertThreshold>> GetAlertThresholdsAsync(Guid vehicleId)
        {
            try
            {
                var cacheKey = $"telemetry:thresholds:{vehicleId}";
                var cachedData = await _distributedCache.GetStringAsync(cacheKey);

                if (cachedData != null)
                {
                    return JsonSerializer.Deserialize<List<AlertThreshold>>(cachedData);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving alert thresholds from cache for vehicle {VehicleId}", vehicleId);
                return null;
            }
        }

        public async Task RemoveAlertThresholdAsync(Guid thresholdId, Guid vehicleId)
        {
            try
            {
                // For simplicity, we'll just invalidate the entire threshold cache for the vehicle
                var cacheKey = $"telemetry:thresholds:{vehicleId}";
                await _distributedCache.RemoveAsync(cacheKey);

                _logger.LogDebug("Removed alert threshold {ThresholdId} cache for vehicle {VehicleId}",
                    thresholdId, vehicleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing alert threshold {ThresholdId} from cache for vehicle {VehicleId}",
                    thresholdId, vehicleId);
            }
        }
    }
}
