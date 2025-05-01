namespace TelemetryService.Domain.Messaging
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(string topic, T message) where T : class;
    }
}
