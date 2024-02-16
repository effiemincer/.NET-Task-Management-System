

namespace BO;

/// <summary>
/// Class representing an Engineer in a Task
/// </summary>
public class EngineerInTask
{
    public int Id { get; init; }
    public string? Name { get; init; }
    public override string ToString() => this.ToStringProperty();
}
