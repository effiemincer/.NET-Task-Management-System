

namespace BO;

public class MilestoneInList
{
    public int Id { get; set; }
    public string? Description { get; init; }
    public string? Alias { get; init; }
    public BO.Enums.Status Status { get; set; }
    public double? CompletionPercentage { get; set; }
    public override string ToString() => this.ToStringProperty();
}
