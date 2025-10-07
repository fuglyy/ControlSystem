

public class DefectDetailDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; }
    public string Status { get; set; }
    
    public ProjectSummaryDto? Project { get; set; }
    public UserSummaryDto? AssignedTo { get; set; } 

    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
}