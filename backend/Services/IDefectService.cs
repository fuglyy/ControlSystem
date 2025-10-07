public interface IDefectService
{
    Task<List<Defect>> GetAllAsync();
    Task<Defect?> GetByIdAsync(string id);
    Task<Defect> CreateAsync(Defect d);
    Task<bool> UpdateAsync(string id, Defect updated);
    Task<bool> DeleteAsync(string id);
    Task<DefectDetailDto?> GetDefectDetailsAsync(string id);
    Task<IEnumerable<DefectSummaryDto>> GetDefectListAsync();
}