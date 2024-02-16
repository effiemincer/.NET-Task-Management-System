

namespace BO;

/// <summary>
/// Class representing a Task in a List
/// </summary>
public class TaskInList
{
    public int Id { get; set; }
    public string? Description { get; init; }
    public string? Alias { get; init; }
    public BO.Enums.Status Status { get; set; }
    public override string ToString() => this.ToStringProperty();
}
