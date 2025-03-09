namespace Application.DTOs.Response
{
    public class LoginResponse
    {
        public bool Flag { get; set; } = false;
        public string Message { get; set; } = null;
        public string Token { get; set; } = null;
        public string RefreshToken { get; set; } = null;
        public string Expiration { get; set; } = null;
    }
}
