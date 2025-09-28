using Microsoft.Extensions.Options;
using MongoDB.Driver;

public class DefectService
{
    private readonly IMongoCollection<Defect> _defects;

    public DefectService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _defects = database.GetCollection<Defect>("Defects");
    }

    public async Task<List<Defect>> GetAllAsync()
    {
        return await _defects.Find(_ => true).ToListAsync();
    }

    public async Task<Defect?> GetByIdAsync(string id)
    {
        return await _defects.Find(d => d.Id == id).FirstOrDefaultAsync();
    }

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
