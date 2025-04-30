namespace Auth.Domain.Models
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Resource { get; set; }
        public string Action { get; set; }

        public Permission(string name, string resource, string action)
        {
            Id = Guid.NewGuid();
            Name = name;
            Resource = resource;
            Action = action;
        }
    }
}
