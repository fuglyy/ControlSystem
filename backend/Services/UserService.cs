using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class UserService : IUserService 
{
    private readonly IMongoCollection<User> _users;

    public UserService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _users = database.GetCollection<User>("Users");
    }

    public async Task<List<User>> GetAsync() =>
        await _users.Find(_ => true).ToListAsync();

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _users.Find(u => u.Username == username).FirstOrDefaultAsync();

    public async Task<User> CreateAsync(User user)
    {
        await _users.InsertOneAsync(user);
        return user;
    }
    public async Task<User?> GetByIdAsync(string id) =>
        await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public async Task<bool> UpdateAsync(User user)
    {
        var result = await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        return result.ModifiedCount > 0;
    }

    public async Task<UserSummaryDto?> GetUserSummaryByIdAsync(string id)
    {
        var userDto = await _users.Find(u => u.Id == id)
            .Project(u => new UserSummaryDto
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role
            })
            .FirstOrDefaultAsync();

        return userDto;
    }
    // UserService.cs

    public async Task<IEnumerable<UserSummaryDto>> GetUsersSummaryByIdsAsync(IEnumerable<string> ids)
    {
        // Находим всех пользователей, ID которых входят в список
        return await _users.Find(u => ids.Contains(u.Id))
            .Project(u => new UserSummaryDto // Проецируем только нужные поля сразу в DTO
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role
            })
            .ToListAsync();
    }

}
