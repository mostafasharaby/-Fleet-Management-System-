namespace Auth.Domain.Models
{
    public class RefreshToken
    {

        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
        public bool IsRevoked { get; set; } = false;
        public bool IsUsed { get; set; } = false;
        public string? ReplacedByToken { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }

        public RefreshToken(string token, DateTime expiryTime, bool isRevoked, bool isUsed, string? appUserId)
        {
            Token = token;
            ExpiryTime = expiryTime;
            IsRevoked = isRevoked;
            IsUsed = isUsed;
            AppUserId = appUserId;
        }

        public void Revoke()
        {
            IsRevoked = true;
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow >= ExpiryTime;
        }
    }
}
