using MongoDB.Driver;

public class DefectService
{
    private readonly IMongoCollection<Defect> _defects;

    public DefectService(IMongoDatabase database)
    {
        _defects = database.GetCollection<Defect>("Defects");
    }

    public async Task<List<Defect>> GetAllAsync() =>
        await _defects.Find(_ => true).ToListAsync();

    public async Task<Defect?> GetByIdAsync(string id) =>
        await _defects.Find(d => d.Id == id).FirstOrDefaultAsync();

    public async Task<Defect> CreateAsync(Defect d)
    {
        await _defects.InsertOneAsync(d);
        return d;
    }

    public async Task<bool> UpdateAsync(string id, Defect updated)
    {
        var result = await _defects.ReplaceOneAsync(d => d.Id == id, updated);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _defects.DeleteOneAsync(d => d.Id == id);
        return result.DeletedCount > 0;
    }
}
