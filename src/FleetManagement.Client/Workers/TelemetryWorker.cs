using FleetManagement.Client.Services;
using TelemetryService.API.Protos;

namespace FleetManagement.Client.Workers
{
    internal class TelemetryWorker : BackgroundService
    {
        private readonly ILogger<TelemetryWorker> _logger;
        private readonly TelemetryServiceClient _telemetryServiceClient;

        public TelemetryWorker(
            TelemetryServiceClient telemetryServiceClient,
            ILogger<TelemetryWorker> logger)
        {
            _telemetryServiceClient = telemetryServiceClient;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                //await GetTelemetryDataById();
                //await GetTelemetryDataByVehicleId();
                //await GetTelemetryDataByTimeRange();
                //await SendTelemetryData();
                //await SendBatchTelemetryData();
                //await GetLatestTelemetryForAllVehicles();
                await StreamVehicleTelemetry(stoppingToken);
                //await GetAlertThresholdsByVehicleId();
                //await GetAlertThresholdById();
                //await CreateAlertThreshold();
                //await UpdateAlertThreshold();
                //await DeleteAlertThreshold();
            }
        }

        private async Task GetTelemetryDataById()
        {
            string telemetryId = "A5E96FF4-E9A6-4FE3-7A7D-08DD85698132";
            try
            {
                _logger.LogInformation($"Fetching telemetry data with ID: {telemetryId}");
                var telemetryData = await _telemetryServiceClient.GetTelemetryDataByIdAsync(telemetryId);

                if (telemetryData != null)
                {
                    Console.WriteLine($"Telemetry Data Found: ID={telemetryData.Id}");
                    Console.WriteLine($"Vehicle ID: {telemetryData.VehicleId}");
                    Console.WriteLine($"Timestamp: {telemetryData.Timestamp}");
                    Console.WriteLine($"Location: ({telemetryData.Latitude}, {telemetryData.Longitude})");
                    Console.WriteLine($"Speed: {telemetryData.Speed} mph");
                    Console.WriteLine($"Fuel Level: {telemetryData.FuelLevel}%");
                    Console.WriteLine($"Engine Temp: {telemetryData.EngineTemperature}°C");
                    Console.WriteLine($"Battery Voltage: {telemetryData.BatteryVoltage}V");
                    Console.WriteLine($"Engine RPM: {telemetryData.EngineRpm}");
                    Console.WriteLine($"Check Engine Light: {telemetryData.CheckEngineLightOn}");
                    Console.WriteLine($"Odometer: {telemetryData.OdometerReading} miles");
                    Console.WriteLine($"Diagnostic Code: {telemetryData.DiagnosticCode ?? "None"}");
                }
                else
                {
                    Console.WriteLine($"Telemetry data with ID {telemetryId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving telemetry data with ID {telemetryId}");
                Console.WriteLine($"Error retrieving telemetry data: {ex.Message}");
            }
        }

        private async Task GetTelemetryDataByVehicleId()
        {
            string vehicleId = "8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C";
            int limit = 5;

            try
            {
                _logger.LogInformation($"Fetching telemetry data for vehicle ID: {vehicleId}, limit: {limit}");
                var telemetryDataList = await _telemetryServiceClient.GetTelemetryDataByVehicleIdAsync(vehicleId, limit);

                if (telemetryDataList.Any())
                {
                    Console.WriteLine($"Found {telemetryDataList.Count()} telemetry data entries for vehicle {vehicleId}:");
                    foreach (var data in telemetryDataList)
                    {
                        Console.WriteLine($"ID: {data.Id}");
                        Console.WriteLine($"Timestamp: {data.Timestamp}");
                        Console.WriteLine($"Speed: {data.Speed} mph");
                        Console.WriteLine($"Fuel Level: {data.FuelLevel}%");
                        Console.WriteLine($"Engine Temp: {data.EngineTemperature}°C");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine($"No telemetry data found for vehicle {vehicleId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving telemetry data for vehicle {vehicleId}");
                Console.WriteLine($"Error retrieving telemetry data: {ex.Message}");
            }
        }

        private async Task GetTelemetryDataByTimeRange()
        {
            string vehicleId = "8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C";
            DateTime startTime = DateTime.UtcNow.AddDays(-1);
            DateTime endTime = DateTime.UtcNow;

            try
            {
                _logger.LogInformation($"Fetching telemetry data for vehicle ID: {vehicleId}, from {startTime} to {endTime}");
                var telemetryDataList = await _telemetryServiceClient.GetTelemetryDataByTimeRangeAsync(vehicleId, startTime, endTime);

                if (telemetryDataList.Count() > 0)
                {
                    Console.WriteLine($"Found {telemetryDataList.Count()} telemetry data entries for vehicle {vehicleId}:");
                    foreach (var data in telemetryDataList)
                    {
                        Console.WriteLine($"ID: {data.Id}");
                        Console.WriteLine($"Timestamp: {data.Timestamp}");
                        Console.WriteLine($"Speed: {data.Speed} mph");
                        Console.WriteLine($"Fuel Level: {data.FuelLevel}%");
                        Console.WriteLine($"Engine Temp: {data.EngineTemperature}°C");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine($"No telemetry data found for vehicle {vehicleId} in time range.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving telemetry data for vehicle {vehicleId}");
                Console.WriteLine($"Error retrieving telemetry data: {ex.Message}");
            }
        }

        private async Task SendTelemetryData()
        {
            string vehicleId = "8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C";
            var telemetryData = new TelemetryDataRequest
            {
                VehicleId = vehicleId,
                //Timestamp = DateTime.UtcNow,
                Latitude = 40.7128,
                Longitude = -74.0060,
                Speed = 60,
                FuelLevel = 75,
                EngineTemperature = 85,
                BatteryVoltage = 12.5,
                EngineRpm = 2000,
                CheckEngineLightOn = false,
                OdometerReading = 50000.5,
                DiagnosticCode = null
            };

            try
            {
                _logger.LogInformation($"Sending telemetry data for vehicle ID: {vehicleId}");
                var response = await _telemetryServiceClient.SendTelemetryDataAsync(telemetryData);

                Console.WriteLine($"Telemetry Data Sent: ID={response.Id}");
                Console.WriteLine($"Vehicle ID: {response.VehicleId}");
                Console.WriteLine($"Timestamp: {response.Timestamp}");
                Console.WriteLine($"Speed: {response.Speed} mph");
                Console.WriteLine($"Fuel Level: {response.FuelLevel}%");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending telemetry data for vehicle {vehicleId}");
                Console.WriteLine($"Error sending telemetry data: {ex.Message}");
            }
        }

        private async Task SendBatchTelemetryData()
        {
            string vehicleId = "8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C";
            var telemetryDataList = new List<TelemetryDataRequest>
            {
                new TelemetryDataRequest
                {
                    VehicleId = vehicleId,
                   // Timestamp = new DateTimeOffset(DateTime.UtcNow.AddMinutes(-2)).ToUnixTimeSeconds(),
                    Latitude = 40.7128,
                    Longitude = -74.0060,
                    Speed = 55,
                    FuelLevel = 80,
                    EngineTemperature = 82,
                    BatteryVoltage = 12.4,
                    EngineRpm = 1800,
                    CheckEngineLightOn = false,
                    OdometerReading = 50000.2,
                    DiagnosticCode = null
                },
                new TelemetryDataRequest
                {
                    VehicleId = vehicleId,
                  //  Timestamp = DateTime.UtcNow.AddMinutes(-1),
                    Latitude = 40.7130,
                    Longitude = -74.0058,
                    Speed = 58,
                    FuelLevel = 78,
                    EngineTemperature = 84,
                    BatteryVoltage = 12.5,
                    EngineRpm = 1900,
                    CheckEngineLightOn = false,
                    OdometerReading = 50000.3,
                    DiagnosticCode = null
                }
            };

            try
            {
                _logger.LogInformation($"Sending batch telemetry data with {telemetryDataList.Count} entries");
                var response = await _telemetryServiceClient.SendBatchTelemetryDataAsync(telemetryDataList);

                if (response.Success)
                {
                    Console.WriteLine($"Batch Telemetry Data Sent: Processed {response.ProcessedCount} entries");
                    Console.WriteLine($"Message: {response.Message}");
                }
                else
                {
                    Console.WriteLine($"Failed to send batch telemetry data: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending batch telemetry data");
                Console.WriteLine($"Error sending batch telemetry data: {ex.Message}");
            }
        }

        private async Task GetLatestTelemetryForAllVehicles()
        {
            try
            {
                _logger.LogInformation($"Fetching latest telemetry data for all vehicles");
                var telemetryDataList = await _telemetryServiceClient.GetLatestTelemetryForAllVehiclesAsync();

                if (telemetryDataList.Count() > 0)
                {
                    Console.WriteLine($"Found {telemetryDataList.Count()} latest telemetry data entries:");
                    foreach (var data in telemetryDataList)
                    {
                        Console.WriteLine($"Vehicle ID: {data.VehicleId}");
                        Console.WriteLine($"Timestamp: {data.Timestamp}");
                        Console.WriteLine($"Speed: {data.Speed} mph");
                        Console.WriteLine($"Fuel Level: {data.FuelLevel}%");
                        Console.WriteLine($"Engine Temp: {data.EngineTemperature}°C");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine($"No telemetry data found for any vehicles.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving latest telemetry data for all vehicles");
                Console.WriteLine($"Error retrieving telemetry data: {ex.Message}");
            }
        }

        private async Task StreamVehicleTelemetry(CancellationToken cancellationToken)
        {
            string vehicleId = "8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C";
            try
            {
                _logger.LogInformation($"Starting telemetry stream for vehicle ID: {vehicleId}");
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                cts.CancelAfter(TimeSpan.FromSeconds(10));

                await _telemetryServiceClient.StreamVehicleTelemetryAsync(
                   vehicleId,
                   async data =>
                    {
                        Console.WriteLine($"Received Streamed Telemetry Data for Vehicle ID: {data.VehicleId}");
                        Console.WriteLine($"Timestamp: {data.Timestamp}");
                        Console.WriteLine($"Speed: {data.Speed} mph");
                        Console.WriteLine($"Fuel Level: {data.FuelLevel}%");
                        Console.WriteLine($"Engine Temp: {data.EngineTemperature}°C");
                        Console.WriteLine("---");
                    },
                    cts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error streaming telemetry data for vehicle {vehicleId}");
                Console.WriteLine($"Error streaming telemetry data: {ex.Message}");
            }
        }

        private async Task GetAlertThresholdsByVehicleId()
        {
            string vehicleId = "8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C";
            try
            {
                _logger.LogInformation($"Fetching alert thresholds for vehicle ID: {vehicleId}");
                var thresholds = await _telemetryServiceClient.GetAlertThresholdsByVehicleIdAsync(vehicleId);

                if (thresholds.Count() > 0)
                {
                    Console.WriteLine($"Found {thresholds.Count()} alert thresholds for vehicle {vehicleId}:");
                    foreach (var threshold in thresholds)
                    {
                        Console.WriteLine($"Threshold ID: {threshold.Id}");
                        Console.WriteLine($"Parameter: {threshold.ParameterName}");
                        Console.WriteLine($"Range: [{threshold.MinValue}, {threshold.MaxValue}]");
                        Console.WriteLine($"Enabled: {threshold.IsEnabled}");
                        Console.WriteLine($"Message: {threshold.AlertMessage}");
                        Console.WriteLine($"Severity: {threshold.Severity}");
                        Console.WriteLine("---");
                    }
                }
                else
                {
                    Console.WriteLine($"No alert thresholds found for vehicle {vehicleId}.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving alert thresholds for vehicle {vehicleId}");
                Console.WriteLine($"Error retrieving alert thresholds: {ex.Message}");
            }
        }

        private async Task GetAlertThresholdById()
        {
            string thresholdId = "";
            try
            {
                _logger.LogInformation($"Fetching alert threshold with ID: {thresholdId}");
                var threshold = await _telemetryServiceClient.GetAlertThresholdByIdAsync(thresholdId);

                if (threshold != null)
                {
                    Console.WriteLine($"Alert Threshold Found: ID={threshold.Id}");
                    Console.WriteLine($"Vehicle ID: {threshold.VehicleId}");
                    Console.WriteLine($"Parameter: {threshold.ParameterName}");
                    Console.WriteLine($"Range: [{threshold.MinValue}, {threshold.MaxValue}]");
                    Console.WriteLine($"Enabled: {threshold.IsEnabled}");
                    Console.WriteLine($"Message: {threshold.AlertMessage}");
                    Console.WriteLine($"Severity: {threshold.Severity}");
                }
                else
                {
                    Console.WriteLine($"Alert threshold with ID {thresholdId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving alert threshold with ID {thresholdId}");
                Console.WriteLine($"Error retrieving alert threshold: {ex.Message}");
            }
        }

        private async Task CreateAlertThreshold()
        {
            string vehicleId = "8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C";
            var threshold = new CreateAlertThresholdRequest
            {
                VehicleId = vehicleId,
                ParameterName = "EngineTemperature",
                MinValue = 70,
                MaxValue = 100,
                IsEnabled = true,
                AlertMessage = "Engine temperature out of safe range",
                Severity = AlertSeverity.High
            };

            try
            {
                _logger.LogInformation($"Creating alert threshold for vehicle ID: {vehicleId}, parameter: {threshold.ParameterName}");
                var createdThreshold = await _telemetryServiceClient.CreateAlertThresholdAsync(threshold);

                Console.WriteLine($"Alert Threshold Created: ID={createdThreshold.Id}");
                Console.WriteLine($"Vehicle ID: {createdThreshold.VehicleId}");
                Console.WriteLine($"Parameter: {createdThreshold.ParameterName}");
                Console.WriteLine($"Range: [{createdThreshold.MinValue}, {createdThreshold.MaxValue}]");
                Console.WriteLine($"Message: {createdThreshold.AlertMessage}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating alert threshold for vehicle {vehicleId}");
                Console.WriteLine($"Error creating alert threshold: {ex.Message}");
            }
        }

        private async Task UpdateAlertThreshold()
        {
            string thresholdId = "";
            string vehicleId = "8DB05DA5-9AE5-46D7-BC3C-10F260EAB20C";
            var threshold = new UpdateAlertThresholdRequest
            {
                Id = thresholdId,
                VehicleId = vehicleId,
                ParameterName = "EngineTemperature",
                MinValue = 65,
                MaxValue = 95,
                IsEnabled = true,
                AlertMessage = "Updated: Engine temperature out of safe range",
                Severity = AlertSeverity.Medium
            };

            try
            {
                _logger.LogInformation($"Updating alert threshold with ID: {thresholdId}");
                var updatedThreshold = await _telemetryServiceClient.UpdateAlertThresholdAsync(threshold);

                if (updatedThreshold != null)
                {
                    Console.WriteLine($"Alert Threshold Updated: ID={updatedThreshold.Id}");
                    Console.WriteLine($"Vehicle ID: {updatedThreshold.VehicleId}");
                    Console.WriteLine($"Parameter: {updatedThreshold.ParameterName}");
                    Console.WriteLine($"Range: [{updatedThreshold.MinValue}, {updatedThreshold.MaxValue}]");
                    Console.WriteLine($"Message: {updatedThreshold.AlertMessage}");
                }
                else
                {
                    Console.WriteLine($"Alert threshold with ID {thresholdId} not found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating alert threshold with ID {thresholdId}");
                Console.WriteLine($"Error updating alert threshold: {ex.Message}");
            }
        }

        private async Task DeleteAlertThreshold()
        {
            string thresholdId = "";
            try
            {
                _logger.LogInformation($"Deleting alert threshold with ID: {thresholdId}");
                var response = await _telemetryServiceClient.DeleteAlertThresholdAsync(thresholdId);

                if (response.Success)
                {
                    Console.WriteLine($"Alert Threshold Deleted: {response.Message}");
                }
                else
                {
                    Console.WriteLine($"Failed to delete alert threshold: {response.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting alert threshold with ID {thresholdId}");
                Console.WriteLine($"Error deleting alert threshold: {ex.Message}");
            }
        }
    }
}