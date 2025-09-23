namespace StockTradePro.BlazorUI.Models.Auth
{

    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserDto User { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}

