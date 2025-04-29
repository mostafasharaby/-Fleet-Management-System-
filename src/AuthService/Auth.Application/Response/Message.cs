namespace Auth.Application.Response
{
    public record Message(IEnumerable<string> To, string Subject, string Body);
}
