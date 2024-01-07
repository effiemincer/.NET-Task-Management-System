
namespace DalApi;

using DO;

public interface ITask
{
    int Create(Task task);
    void Delete(Task task);
    void Update(Task task);
    Task? Read(int id);
    List<Task> ReadAll();
    void Reset();

}
