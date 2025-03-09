namespace Application.DTOs.Request.Account
{
    public class RefreshTokenRequestDTO
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
