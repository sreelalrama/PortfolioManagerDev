namespace StockTradePro.BlazorUI.Models.Auth
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string Bio { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}
