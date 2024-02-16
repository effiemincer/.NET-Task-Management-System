
namespace BO;

/// <summary>
/// Class representing a Milestone in a Task
/// </summary>
public class MilestoneInTask
{
    public int Id { get; init; }
    public string? Alias { get; init; }
    public override string ToString() => this.ToStringProperty();
}
