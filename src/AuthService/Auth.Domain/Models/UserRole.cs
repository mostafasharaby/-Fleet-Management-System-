namespace Auth.Domain.Models
{
    public class UserRole
    {
        public Guid UserId { get; private set; }
        public AppUser User { get; private set; }
        public Guid RoleId { get; private set; }
        public Role Role { get; private set; }

        public UserRole(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}
