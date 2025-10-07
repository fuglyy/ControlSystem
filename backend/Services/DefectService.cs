using MongoDB.Driver;
public class DefectService : IDefectService 
{
    private readonly IMongoCollection<Defect> _defects;
    
    
    private readonly IUserService _userService;
    private readonly IProjectService _projectService; 

    
    public DefectService(
        IMongoDatabase database, 
        IUserService userService, 
        IProjectService projectService) 
    {
        _defects = database.GetCollection<Defect>("Defects");
        _userService = userService;
        _projectService = projectService;
    }

    public async Task<List<Defect>> GetAllAsync() =>
        await _defects.Find(_ => true).ToListAsync();

    public async Task<Defect?> GetByIdAsync(string id) => // 👈 ВОТ ЭТОГО НЕ ХВАТАЕТ
        await _defects.Find(d => d.Id == id).FirstOrDefaultAsync();

    public async Task<Defect> CreateAsync(Defect d) // 👈 ВОТ ЭТОГО НЕ ХВАТАЕТ
    {
        await _defects.InsertOneAsync(d);
        return d;
    }

    public async Task<bool> UpdateAsync(string id, Defect updated) // 👈 ВОТ ЭТОГО НЕ ХВАТАЕТ
    {
        var result = await _defects.ReplaceOneAsync(d => d.Id == id, updated);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id) // 👈 ВОТ ЭТОГО НЕ ХВАТАЕТ
    {
        var result = await _defects.DeleteOneAsync(d => d.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<DefectDetailDto?> GetDefectDetailsAsync(string id)
    {
        var defect = await GetByIdAsync(id);
        if (defect == null) return null;

        var projectDto = defect.ProjectId != null 
            ? await _projectService.GetProjectSummaryByIdAsync(defect.ProjectId) 
            : null;
            
        var userDto = defect.AssignedToId != null 
            ? await _userService.GetUserSummaryByIdAsync(defect.AssignedToId) 
            : null;

        return new DefectDetailDto
        {
            Id = defect.Id,
            Title = defect.Title,
            Description = defect.Description,
            CreatedAt = defect.CreatedAt,
            DueDate = defect.DueDate,
            
            Project = projectDto,
            AssignedTo = userDto
        };
    }

    public async Task<IEnumerable<DefectSummaryDto>> GetDefectListAsync()
    {
        var defects = await _defects.Find(_ => true).ToListAsync();
        
        var projectIds = defects.Select(d => d.ProjectId).Distinct().ToList();
        var userIds = defects.Select(d => d.AssignedToId).Distinct().ToList();

        var projects = (await _projectService.GetProjectsSummaryByIdsAsync(projectIds))
                        .ToDictionary(p => p.Id, p => p);
        var users = (await _userService.GetUsersSummaryByIdsAsync(userIds))
                    .ToDictionary(u => u.Id, u => u);

        return defects.Select(d => new DefectSummaryDto
        {
            Id = d.Id!,
            Title = d.Title,
            Status = d.Status,
            Priority = d.Priority,
            Project = projects.GetValueOrDefault(d.ProjectId),
            AssignedTo = users.GetValueOrDefault(d.AssignedToId)
        }).ToList();
    }
}