using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

public class Defect
{
[BsonId] // ключ документа
    [BsonRepresentation(BsonType.ObjectId)] 
    public string? Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public string Priority { get; set; } = "Medium"; 

    public string Status { get; set; } = "New"; 

    public string? AssignedToId { get; set; }
    
     public string? ProjectId { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DueDate { get; set; }
}
