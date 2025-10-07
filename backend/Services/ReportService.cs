// ReportService.cs
public class ReportService : IReportService
{
    // ⭐ ИСПОЛЬЗУЕМ ИНТЕРФЕЙСЫ ⭐
    private readonly IProjectService _projects;
    private readonly IDefectService _defects;
    private readonly IUserService _users; // Внедряем UserService

    // Обновлённый конструктор
    public ReportService(
        IDefectService defects, 
        IProjectService projects, 
        IUserService users) // 👈 Получаем все зависимости через DI
    {
        _defects = defects;
        _projects = projects;
        _users = users;
    }

    public async Task<IEnumerable<object>> GetProjectReportAsync()
    {
        var projects = await _projects.GetAllAsync();
        return projects.Select(p => new
        {
            p.Id,
            p.Name,
            TotalStages = p.Stages.Count,
            Completed = p.Stages.Count(s => s.Status == "Done"),
            InProgress = p.Stages.Count(s => s.Status == "InProgress"),
            // Статус "Planned" обычно называется "New" или "Todo", 
            // но используем то, что у вас есть
            Planned = p.Stages.Count(s => s.Status == "Planned") 
        });
    }

    public async Task<IEnumerable<object>> GetDefectReportAsync()
    {
        var defects = await _defects.GetAllAsync();
        return defects.GroupBy(d => d.Status).Select(g => new { Status = g.Key, Count = g.Count() });
    }
    
    // ⭐ НОВЫЙ МЕТОД: Отчет по исполнителям, связывающий Defect и User
    public async Task<IEnumerable<object>> GetAssigneeReportAsync()
    {
        var defects = await _defects.GetAllAsync();
        var users = await _users.GetAsync();

        // 1. Группируем дефекты по AssignedToId
        var defectGroups = defects.Where(d => d.AssignedToId != null)
                                  .GroupBy(d => d.AssignedToId!)
                                  .Select(g => new 
                                  { 
                                      UserId = g.Key, 
                                      DefectCount = g.Count() 
                                  });

        // 2. Объединяем с данными пользователя, чтобы получить Username/Role
        return defectGroups.Join(
            users,
            d => d.UserId,
            u => u.Id,
            (d, u) => new 
            {
                u.Username,
                u.Role,
                d.DefectCount
            });
    }

    public async Task<object> GetSummaryAsync()
    {
        var projects = await _projects.GetAllAsync();
        var defects = await _defects.GetAllAsync();

        return new
        {
            TotalProjects = projects.Count,
            TotalDefects = defects.Count,
            DefectsByStatus = defects.GroupBy(d => d.Status).Select(g => new { Status = g.Key, Count = g.Count() }),
            ProjectsProgress = projects.Select(p => new
            {
                p.Id,
                p.Name,
                CompletedStages = p.Stages.Count(s => s.Status == "Done"),
                TotalStages = p.Stages.Count
            })
        };
    }

    public async Task<byte[]> ExportDefectsCsvAsync()
    {
        var defects = await _defects.GetAllAsync();
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Id,Title,Status,Priority,ProjectId,AssignedToId,CreatedAt,DueDate");
        foreach (var d in defects)
        {
            string escape(string? s) => string.IsNullOrEmpty(s) ? "" : $"\"{s.Replace("\"", "\"\"")}\"";
            sb.AppendLine($"{d.Id},{escape(d.Title)},{escape(d.Status)},{escape(d.Priority)},{d.ProjectId},{d.AssignedToId},{d.CreatedAt:o},{d.DueDate:o}");
        }
        return System.Text.Encoding.UTF8.GetBytes(sb.ToString());
    }
        public async Task<object> GetSummaryReportAsync()
    {
       
        
        var projects = await _projects.GetAllAsync();
        var defects = await _defects.GetAllAsync();

        var summary = new
        {
            TotalProjects = projects.Count(),
            TotalDefects = defects.Count(),
            DefectsByStatus = defects.GroupBy(d => d.Status).Select(g => new { Status = g.Key, Count = g.Count() }),
            ProjectsProgress = projects.Select(p => new
            {
                p.Name,
                CompletedStages = p.Stages.Count(s => s.Status == "Done"),
                TotalStages = p.Stages.Count
            })
        };

        return summary;
    }
}