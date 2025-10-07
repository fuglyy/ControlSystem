public class DefectSummaryDto
{
    // Обязательные поля
    public required string Id { get; set; } 
    public required string Title { get; set; }
    public required string Status { get; set; }
    public required string Priority { get; set; }

    // Обогащенные поля
    public required ProjectSummaryDto Project { get; set; } // Используем ваш существующий ProjectSummaryDto
    public required UserSummaryDto AssignedTo { get; set; } // Используем ваш существующий UserSummaryDto
}