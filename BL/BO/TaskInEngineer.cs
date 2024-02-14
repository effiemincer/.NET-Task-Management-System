
namespace BO;

public class TaskInEngineer
{
    public int Id { get; init; }
    public string? Alias { get; init; }
    public override string ToString() => this.ToStringProperty();

    public TaskInEngineer(int id, string? alias)
    {
        Id = id;
        Alias = alias;
    }   
}
