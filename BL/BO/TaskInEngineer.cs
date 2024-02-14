
namespace BO;

public class TaskInEngineer
{
    public int Id { get; init; }
    public string? Alias { get; init; }

    public TaskInEngineer(int id, string? alias)
    {
        Id = id;
        Alias = alias;
    }   
}
