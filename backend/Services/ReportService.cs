public class ReportService
{
    private readonly ProjectService _projects;
    private readonly DefectService _defects;

    public ReportService(ProjectService projects, DefectService defects)
    {
        _projects = projects;
        _defects = defects;
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
            Planned = p.Stages.Count(s => s.Status == "Planned")
        });
    }

    public async Task<IEnumerable<object>> GetDefectReportAsync()
    {
        var defects = await _defects.GetAllAsync();
        return defects.GroupBy(d => d.Status).Select(g => new { Status = g.Key, Count = g.Count() });
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
            string escape(string? s) => string.IsNullOrEmpty(s) ? "" : $"\"{s.Replace("\"","\"\"")}\"";
            sb.AppendLine($"{d.Id},{escape(d.Title)},{escape(d.Status)},{escape(d.Priority)},{d.ProjectId},{d.AssignedToId},{d.CreatedAt:o},{d.DueDate:o}");
        }
        return System.Text.Encoding.UTF8.GetBytes(sb.ToString());
    }
}
