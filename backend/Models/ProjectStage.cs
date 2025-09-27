public class ProjectStage
{
    public string Title { get; set; } // название этапа (например "Проектирование")
    public string Status { get; set; } = "New"; // New / InProgress / Done
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
