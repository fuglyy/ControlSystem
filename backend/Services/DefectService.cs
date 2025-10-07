using MongoDB.Driver;
using MongoDB.Bson;
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
    // 1. Получаем сырой объект Defect
    // Используем прямое обращение к коллекции, чтобы избежать лишней зависимости от GetByIdAsync
    var defect = await _defects.Find(d => d.Id == id).FirstOrDefaultAsync(); 
    
    if (defect == null) return null;

    // 2. Инициализация DTO для обогащения
    ProjectSummaryDto? projectDto = null;
    UserSummaryDto? userDto = null; // Используем UserSummaryDto вместо AssignedToUserDto

    // 3. ⭐ ПРОВЕРКА и ОБОГАЩЕНИЕ ДЛЯ ПРОЕКТА ⭐
    // Проверяем, что ID не пустой и является валидным MongoDB ObjectId
    if (!string.IsNullOrEmpty(defect.ProjectId) && ObjectId.TryParse(defect.ProjectId, out _))
    {
        projectDto = await _projectService.GetProjectSummaryByIdAsync(defect.ProjectId);
    }
    
    // 4. ⭐ ПРОВЕРКА и ОБОГАЩЕНИЕ ДЛЯ ПОЛЬЗОВАТЕЛЯ ⭐
    // Проверяем, что ID не пустой и является валидным MongoDB ObjectId
    if (!string.IsNullOrEmpty(defect.AssignedToId) && ObjectId.TryParse(defect.AssignedToId, out _))
    {
        userDto = await _userService.GetUserSummaryByIdAsync(defect.AssignedToId);
    }
    
    // 5. Маппинг в финальный DTO
    return new DefectDetailDto
    {
        Id = defect.Id,
        Title = defect.Title,
        Description = defect.Description,
        Status = defect.Status, // Не забудьте включить все необходимые поля!
        Priority = defect.Priority, 
        CreatedAt = defect.CreatedAt,
        DueDate = defect.DueDate,
        
        Project = projectDto,
        AssignedTo = userDto
    };
}

    public async Task<IEnumerable<DefectSummaryDto>> GetDefectListAsync()
{
    var defects = await _defects.Find(_ => true).ToListAsync();
    
    // ⭐ ИСПРАВЛЕНИЕ: Фильтруем все невалидные ID (null или не 24-значный hex) ⭐
    
    // 1. Собираем Project IDs
    var projectIds = defects.Select(d => d.ProjectId)
                            .Where(id => id != null && ObjectId.TryParse(id, out _)) // <-- Защита
                            .Distinct().ToList();
                            
    // 2. Собираем User IDs
    var userIds = defects.Select(d => d.AssignedToId)
                         .Where(id => id != null && ObjectId.TryParse(id, out _)) // <-- Защита
                         .Distinct().ToList();

    // Загружаем Summary-DTO для валидных ID
    var projects = (await _projectService.GetProjectsSummaryByIdsAsync(projectIds))
                    .ToDictionary(p => p.Id, p => p);
    var users = (await _userService.GetUsersSummaryByIdsAsync(userIds))
                 .ToDictionary(u => u.Id, u => u);

    // Маппим и обогащаем
    return defects.Select(d => new DefectSummaryDto
    {
        Id = d.Id!,
        Title = d.Title,
        Status = d.Status,
        Priority = d.Priority,
        // Обогащение: если ID невалиден, GetValueOrDefault вернёт null, что соответствует || 'Н/Д' во Vue
        Project = d.ProjectId != null ? projects.GetValueOrDefault(d.ProjectId!) : null,
        AssignedTo = d.AssignedToId != null ? users.GetValueOrDefault(d.AssignedToId!) : null
    }).ToList();
}
}