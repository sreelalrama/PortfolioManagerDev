namespace StockTradePro.NotificationService.API.Services
{
    public interface IUserService
    {
        Task<string?> GetUserEmailAsync(string userId);
        Task<UserDto?> GetUserAsync(string userId);
        Task<IEnumerable<string>> GetAllUserIdsAsync();
    }

    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
