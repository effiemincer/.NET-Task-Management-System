
namespace BO;

public class Milestone
{
    public int Id { get; init; }
    public string Description { get; init; }
    public string Alias { get; init; }
    public DateTime CreatedAt { get; init; }
    public BO.Status? Status { get; set; }
    public DateTime? ForecastDate { get; set; }
    public DateTime? DeadlineDate { get; set; }
    public DateTime? CompleteDate{ get; set; }
    public double? CompletionPercentage { get; set; }
    public string? Remarks { get; set; }
    public List<BO.TaskInList>? Dependencies { get; set; }
}
