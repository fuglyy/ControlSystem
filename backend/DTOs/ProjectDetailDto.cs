
public class ProjectDetailDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required List<ProjectStage> Stages { get; set; } // Включая этапы
    public DateTime CreatedAt { get; set; }
}