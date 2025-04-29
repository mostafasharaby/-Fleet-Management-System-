namespace Auth.Application.DTOs
{
    public class RefreshTokenRequest
    {
        public string ExpiredToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
