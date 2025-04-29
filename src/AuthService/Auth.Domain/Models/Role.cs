namespace Auth.Domain.Models
{
    public class Role
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public List<Permission> Permissions { get; private set; } = new List<Permission>();

        public Role(string name, string description)
        {
            Id = Guid.NewGuid();
            Name = name;
            Description = description;
        }
    }
}
