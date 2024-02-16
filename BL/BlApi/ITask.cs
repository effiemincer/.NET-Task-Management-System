

namespace BlApi;

public interface ITask
{
    public int Create(BO.Task item);
    public BO.Task? Read(int id);
    public IEnumerable<BO.TaskInList> ReadAll(Func<BO.Task, bool>? filter = null);
    public BO.Task Update(BO.Task? item);
    public void Delete(int id);

    public void UpdateProjectedStartDate(int id, DateTime? newDateTime);

}
