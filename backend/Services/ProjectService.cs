using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class ProjectService : IProjectService 
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
    public async Task<ProjectSummaryDto?> GetProjectSummaryByIdAsync(string id)
    {
        var project = await _projects.Find(p => p.Id == id)
            .Project(p => new ProjectSummaryDto
            {
                Id = p.Id,
                Name = p.Name
            })
            .FirstOrDefaultAsync();

        return project;
    }
    public async Task<IEnumerable<ProjectSummaryDto>> GetProjectsSummaryByIdsAsync(IEnumerable<string> ids)
    {
        return await _projects.Find(p => ids.Contains(p.Id))
            .Project(p => new ProjectSummaryDto // Проецируем только нужные поля сразу в DTO
            {
                Id = p.Id,
                Name = p.Name
            })
            .ToListAsync();
    }
        public async Task<ProjectDetailDto?> GetDetailByIdAsync(string id)
    {
        var project = await GetByIdAsync(id); // Используем существующий метод для загрузки
        if (project == null) return null;

        // Маппим полную модель Project в ProjectDetailDto
        var detailDto = new ProjectDetailDto
        {
            Id = project.Id!,
            Name = project.Name,
            Description = project.Description,
            Stages = project.Stages, // Передаем список этапов
            CreatedAt = project.CreatedAt
        };

        return detailDto;
    }
    public async Task<IEnumerable<ProjectSummaryDto>> GetAllSummariesAsync()
    {
        // Загружаем все проекты и проецируем в ProjectSummaryDto
        return await _projects.Find(_ => true)
            .Project(p => new ProjectSummaryDto
            {
                Id = p.Id,
                Name = p.Name
            })
            .ToListAsync();
    }
}
