using NotificationService.Domain.Enums;
using NotificationService.Domain.Repositories;

namespace NotificationService.Application.Services
{
    public class NotificationChannelFactory
    {
        private readonly IEnumerable<INotificationChannel> _channels;

        public NotificationChannelFactory(IEnumerable<INotificationChannel> channels)
        {
            _channels = channels ?? throw new ArgumentNullException(nameof(channels));
        }

        public IEnumerable<INotificationChannel> GetChannels(IEnumerable<NotificationChannel> channelTypes)
        {
            if (channelTypes == null || !channelTypes.Any() || channelTypes.Contains(NotificationChannel.All))
            {
                return _channels;
            }

            return _channels.Where(c => channelTypes.Contains(c.ChannelType));
        }
    }
}
