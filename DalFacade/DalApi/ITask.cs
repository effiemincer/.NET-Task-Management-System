
namespace DalApi;

using DO;

public interface ITask
{
    int Create(Task task);
    void Delete(int id);
    void Update(Task task);
    Task? Read(int id);
    List<Task> ReadAll();

}
