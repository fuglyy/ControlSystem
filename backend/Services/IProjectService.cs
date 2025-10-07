public interface IProjectService
{
    Task<List<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(string id);
    Task CreateAsync(Project project);
    Task<bool> AddStageAsync(string id, ProjectStage stage);
    
    Task<ProjectSummaryDto?> GetProjectSummaryByIdAsync(string id);
    Task<IEnumerable<ProjectSummaryDto>> GetProjectsSummaryByIdsAsync(IEnumerable<string> ids);
    
    Task<IEnumerable<ProjectSummaryDto>> GetAllSummariesAsync(); 
    Task<ProjectDetailDto?> GetDetailByIdAsync(string id);      
}