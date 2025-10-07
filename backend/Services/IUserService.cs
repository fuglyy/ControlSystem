public interface IUserService
{
    Task<List<User>> GetAsync();
    Task<User?> GetByUsernameAsync(string username);
    Task<UserSummaryDto?> GetUserSummaryByIdAsync(string id); 
    Task<User> CreateAsync(User user);
    Task<User?> GetByIdAsync(string id); 
    Task<bool> UpdateAsync(User user);
    Task<IEnumerable<UserSummaryDto>> GetUsersSummaryByIdsAsync(IEnumerable<string> ids);
}