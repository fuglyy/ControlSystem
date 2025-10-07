
public interface IReportService
{
    Task<IEnumerable<object>> GetProjectReportAsync();
    Task<IEnumerable<object>> GetDefectReportAsync();
    Task<object> GetSummaryAsync();
    Task<byte[]> ExportDefectsCsvAsync();
    Task<object> GetSummaryReportAsync(); 
    Task<IEnumerable<object>> GetAssigneeReportAsync(); 
}