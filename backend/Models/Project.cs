using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

public class Project{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string? Description { get; set; }

    public List<ProjectStage> Stages { get; set; } = new();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}