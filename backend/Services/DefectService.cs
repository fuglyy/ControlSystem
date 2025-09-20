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

    public async Task<List<Defect>> GetAsync() =>
        await _defects.Find(_ => true).ToListAsync();

    public async Task<Defect?> GetAsync(string id) =>
        await _defects.Find(d => d.Id == id).FirstOrDefaultAsync();

    public async Task<Defect> CreateAsync(Defect d)
    {
        await _defects.InsertOneAsync(d);
        return d;
    }
}
