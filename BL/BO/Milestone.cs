

namespace BO;

public class Milestone
{
    public int Id { get; init; }
    public string? Description { get; init; } 
    public string? Alias { get; init; } 
    public DateTime DateCreated { get; init; }
    public BO.Enums.Status Status { get; init; }
    public DateTime? ProjectedStartDate { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public double? CompletionPercentage { get; set; }
    public string? Remarks { get; set; }
    public List<BO.TaskInList>? Dependencies { get; set; }
    public override string ToString() => this.ToStringProperty();
}
