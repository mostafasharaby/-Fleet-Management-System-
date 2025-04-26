using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using TelemetryService.Application.DTOs;
using TelemetryService.Domain.Events;
using TelemetryService.Domain.Models;
using TelemetryService.Domain.Repositories;

namespace TelemetryService.Application.Services
{
    public class TelemetryService : ITelemetryService
    {
        private readonly ITelemetryRepository _telemetryRepository;
        private readonly IAlertThresholdRepository _alertThresholdRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<TelemetryService> _logger;

        public TelemetryService(
            ITelemetryRepository telemetryRepository,
            IAlertThresholdRepository alertThresholdRepository,
            IMapper mapper,
            IMediator mediator,
            ILogger<TelemetryService> logger)
        {
            _telemetryRepository = telemetryRepository;
            _alertThresholdRepository = alertThresholdRepository;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<TelemetryDataDto> GetTelemetryDataByIdAsync(Guid id)
        {
            var telemetryData = await _telemetryRepository.GetByIdAsync(id);
            return _mapper.Map<TelemetryDataDto>(telemetryData);
        }

        public async Task<IEnumerable<TelemetryDataDto>> GetTelemetryDataByVehicleIdAsync(Guid vehicleId, int limit = 100)
        {
            var telemetryData = await _telemetryRepository.GetByVehicleIdAsync(vehicleId, limit);
            return _mapper.Map<IEnumerable<TelemetryDataDto>>(telemetryData);
        }

        public async Task<IEnumerable<TelemetryDataDto>> GetTelemetryDataByTimeRangeAsync(Guid vehicleId, DateTime startTime, DateTime endTime)
        {
            var telemetryData = await _telemetryRepository.GetByVehicleIdTimeRangeAsync(vehicleId, startTime, endTime);
            return _mapper.Map<IEnumerable<TelemetryDataDto>>(telemetryData);
        }

        public async Task ProcessTelemetryDataAsync(TelemetryDataDto telemetryDataDto)
        {
            try
            {
                var telemetryData = _mapper.Map<TelemetryData>(telemetryDataDto);
                await _telemetryRepository.AddAsync(telemetryData);

                // Notify about new telemetry data
                await _mediator.Publish(new TelemetryDataReceivedEvent(
                    telemetryData.VehicleId,
                    telemetryData.Latitude,
                    telemetryData.Longitude,
                    telemetryData.Speed));

                // Check thresholds
                await CheckThresholdsAsync(telemetryData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing telemetry data for vehicle {VehicleId}", telemetryDataDto.VehicleId);
                throw;
            }
        }

        public async Task ProcessBatchTelemetryDataAsync(IEnumerable<TelemetryDataDto> telemetryDataBatch)
        {
            var telemetryDataEntities = _mapper.Map<IEnumerable<TelemetryData>>(telemetryDataBatch).ToList();
            await _telemetryRepository.AddRangeAsync(telemetryDataEntities);

            // Process each item for alerts and notifications
            foreach (var telemetryData in telemetryDataEntities)
            {
                // Notify about new telemetry data
                await _mediator.Publish(new TelemetryDataReceivedEvent(
                    telemetryData.VehicleId,
                    telemetryData.Latitude,
                    telemetryData.Longitude,
                    telemetryData.Speed));

                // Check thresholds
                await CheckThresholdsAsync(telemetryData);
            }
        }

        public async Task<IEnumerable<TelemetryDataDto>> GetLatestTelemetryForAllVehiclesAsync()
        {
            var latestTelemetryData = await _telemetryRepository.GetLatestForAllVehiclesAsync();
            return _mapper.Map<IEnumerable<TelemetryDataDto>>(latestTelemetryData);
        }

        private async Task CheckThresholdsAsync(TelemetryData telemetryData)
        {
            var thresholds = await _alertThresholdRepository.GetByVehicleIdAsync(telemetryData.VehicleId);

            foreach (var threshold in thresholds.Where(t => t.IsEnabled))
            {
                double parameterValue = GetParameterValue(telemetryData, threshold.ParameterName);

                if (parameterValue < threshold.MinValue || parameterValue > threshold.MaxValue)
                {
                    double thresholdValue = parameterValue < threshold.MinValue
                        ? threshold.MinValue
                        : threshold.MaxValue;

                    await _mediator.Publish(new ThresholdAlertEvent(
                        telemetryData.VehicleId,
                        threshold.ParameterName,
                        parameterValue,
                        thresholdValue,
                        threshold.Severity,
                        threshold.AlertMessage));
                }
            }
        }

        private double GetParameterValue(TelemetryData telemetryData, string parameterName)
        {
            return parameterName switch
            {
                "Speed" => telemetryData.Speed,
                "FuelLevel" => telemetryData.FuelLevel,
                "EngineTemperature" => telemetryData.EngineTemperature,
                "BatteryVoltage" => telemetryData.BatteryVoltage,
                "EngineRpm" => telemetryData.EngineRpm,
                "OdometerReading" => telemetryData.OdometerReading,
                _ => throw new ArgumentException($"Unknown parameter name: {parameterName}")
            };
        }
    }
}
