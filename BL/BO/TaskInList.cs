
namespace BO;

public class TaskInList
{
    public int Id { get; init; }
    public string Description { get; init; }
    public string Alias { get; init; }
    public BO.Status? Status { get; set; }
}
