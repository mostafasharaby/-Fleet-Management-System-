namespace NotificationService.Domain.ValueObjects
{
    public class NotificationContent
    {
        public string Title { get; private set; }
        public string Body { get; private set; }
        public Dictionary<string, string> Metadata { get; private set; }

        public NotificationContent(string title, string body, Dictionary<string, string> metadata = null)
        {
            Title = !string.IsNullOrEmpty(title) ? title : throw new ArgumentException("Title cannot be null or empty");
            Body = !string.IsNullOrEmpty(body) ? body : throw new ArgumentException("Body cannot be null or empty");
            Metadata = metadata ?? new Dictionary<string, string>();
        }
    }
}
