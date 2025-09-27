using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class ProjectService
{
    private readonly IMongoCollection<Project> _projects;

    public ProjectService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _projects = database.GetCollection<Project>("Projects");
    }

    public async Task<List<Project>> GetAllAsync() =>
        await _projects.Find(_ => true).ToListAsync();

    public async Task<Project?> GetByIdAsync(string id) =>
        await _projects.Find(p => p.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Project project) =>
        await _projects.InsertOneAsync(project);

    public async Task<bool> AddStageAsync(string id, ProjectStage stage)
    {
        var update = Builders<Project>.Update.Push(p => p.Stages, stage);
        var result = await _projects.UpdateOneAsync(p => p.Id == id, update);
        return result.ModifiedCount > 0;
    }
}
