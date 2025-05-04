using AutoMapper;
using Grpc.Core;
using Grpc.Net.Client;
using NotificationService.API.Protos;
using NotificationService.Domain.Models;

namespace FleetManagement.Client.Services
{
    public class NotificationServiceClient : IDisposable
    {
        private readonly GrpcChannel _channel;
        private readonly NotificationService.API.Protos.NotificationService.NotificationServiceClient _client;
        private readonly ILogger<NotificationServiceClient> _logger;
        private readonly IMapper _mapper;

        public NotificationServiceClient(string serviceUrl, ILogger<NotificationServiceClient> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _channel = GrpcChannel.ForAddress(serviceUrl);
            _client = new NotificationService.API.Protos.NotificationService.NotificationServiceClient(_channel);
            _logger.LogInformation($"NotificationServiceClient initialized with endpoint: {serviceUrl}");
        }

        public async Task<SendNotificationResponse> SendNotificationAsync(SendNotificationRequest request)
        {
            try
            {
                _logger.LogDebug($"Sending notification to recipients: {string.Join(", ", request.RecipientIds)}");
                var protoRequest = _mapper.Map<SendNotificationRequest>(request);
                var response = await _client.SendNotificationAsync(protoRequest);
                return _mapper.Map<SendNotificationResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid notification request: {ex.Status.Detail}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to recipients: {string.Join(", ", request.RecipientIds)}");
                throw;
            }
        }

        public async Task<CreateTemplateResponse> CreateTemplateAsync(NotificationTemplate template)
        {
            try
            {
                _logger.LogDebug($"Creating notification template: {template.Name}");
                var request = new CreateTemplateRequest
                {
                    Name = template.Name,
                    Description = template.Description,
                    Type = (NotificationType)template.Type,
                    TitleTemplate = template.TitleTemplate ?? string.Empty,
                    BodyTemplate = template.BodyTemplate ?? string.Empty,
                    DefaultMetadata = { template.DefaultMetadata ?? new Dictionary<string, string>() }
                };
                var response = await _client.CreateTemplateAsync(request);
                return _mapper.Map<CreateTemplateResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid template request: {ex.Status.Detail}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating notification template: {template.Name}");
                throw;
            }
        }

        public async Task<GetNotificationHistoryResponse> GetNotificationHistoryAsync(GetNotificationHistoryRequest request)
        {
            try
            {
                _logger.LogDebug($"Getting notification history for user ID: {request.UserId}");
                var response = await _client.GetNotificationHistoryAsync(request);
                return _mapper.Map<GetNotificationHistoryResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid history request: {ex.Status.Detail}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting notification history for user {request.UserId}");
                throw;
            }
        }

        public async Task<MarkAsReadResponse> MarkAsReadAsync(MarkAsReadRequest request)
        {
            try
            {
                _logger.LogDebug($"Marking notifications as read for user ID: {request.UserId}");
                var protoRequest = _mapper.Map<MarkAsReadRequest>(request);
                var response = await _client.MarkAsReadAsync(protoRequest);
                return _mapper.Map<MarkAsReadResponse>(response);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid mark as read request: {ex.Status.Detail}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking notifications as read for user {request.UserId}");
                throw;
            }
        }

        public async Task SubscribeToNotificationsAsync(SubscriptionRequest request, Action<NotificationMessage> onNotificationReceived, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogDebug($"Starting notification subscription for user ID: {request.UserId}");
                var protoRequest = _mapper.Map<SubscriptionRequest>(request);
                using var streamingCall = _client.SubscribeToNotifications(protoRequest, cancellationToken: cancellationToken);

                await foreach (var response in streamingCall.ResponseStream.ReadAllAsync(cancellationToken))
                {
                    var notification = _mapper.Map<NotificationMessage>(response);
                    onNotificationReceived(notification);
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
            {
                _logger.LogWarning($"Invalid subscription request: {ex.Status.Detail}");
                throw;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                _logger.LogInformation($"Notification subscription for user {request.UserId} was canceled");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error subscribing to notifications for user {request.UserId}");
                throw;
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
        }
    }
}