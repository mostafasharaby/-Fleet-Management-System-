namespace Auth.Domain.Models
{
    public class Permission
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Resource { get; private set; }
        public string Action { get; private set; }

        public Permission(string name, string resource, string action)
        {
            Id = Guid.NewGuid();
            Name = name;
            Resource = resource;
            Action = action;
        }
    }
}
