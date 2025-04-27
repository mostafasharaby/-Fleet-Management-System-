using AutoMapper;
using Microsoft.Extensions.Logging;
using TelemetryService.Application.DTOs;
using TelemetryService.Domain.Models;
using TelemetryService.Domain.Repositories;

namespace TelemetryService.Application.Services
{
    public class AlertThresholdService : IAlertThresholdService
    {
        private readonly IAlertThresholdRepository _alertThresholdRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AlertThresholdService> _logger;

        public AlertThresholdService(
            IAlertThresholdRepository alertThresholdRepository,
            IMapper mapper,
            ILogger<AlertThresholdService> logger)
        {
            _alertThresholdRepository = alertThresholdRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<AlertThresholdDto>> GetAlertThresholdsByVehicleIdAsync(Guid vehicleId)
        {
            var thresholds = await _alertThresholdRepository.GetByVehicleIdAsync(vehicleId);
            return _mapper.Map<IEnumerable<AlertThresholdDto>>(thresholds);
        }

        public async Task<AlertThresholdDto> GetAlertThresholdByIdAsync(Guid id)
        {
            var threshold = await _alertThresholdRepository.GetByIdAsync(id);
            return _mapper.Map<AlertThresholdDto>(threshold);
        }

        public async Task<AlertThresholdDto> CreateAlertThresholdAsync(AlertThresholdDto alertThresholdDto)
        {
            try
            {
                var threshold = new AlertThreshold(
                    alertThresholdDto.VehicleId,
                    alertThresholdDto.ParameterName,
                    alertThresholdDto.MinValue,
                    alertThresholdDto.MaxValue,
                    alertThresholdDto.IsEnabled,
                    alertThresholdDto.AlertMessage,
                    alertThresholdDto.Severity);

                await _alertThresholdRepository.AddAsync(threshold);
                return _mapper.Map<AlertThresholdDto>(threshold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating alert threshold for vehicle {VehicleId}", alertThresholdDto.VehicleId);
                throw;
            }
        }

        public async Task UpdateAlertThresholdAsync(AlertThresholdDto alertThresholdDto)
        {
            try
            {
                var existingThreshold = await _alertThresholdRepository.GetByIdAsync(alertThresholdDto.Id);
                if (existingThreshold == null)
                {
                    throw new KeyNotFoundException($"Alert threshold with ID {alertThresholdDto.Id} not found");
                }

                _mapper.Map(alertThresholdDto, existingThreshold);
                await _alertThresholdRepository.UpdateAsync(existingThreshold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating alert threshold {ThresholdId}", alertThresholdDto.Id);
                throw;
            }
        }

        public async Task DeleteAlertThresholdAsync(Guid id)
        {
            try
            {
                await _alertThresholdRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting alert threshold {ThresholdId}", id);
                throw;
            }
        }
    }
}
