
namespace DalApi;

using DO;

public interface ITask: ICrud<Task>
{
    //void TasksReset(); //erase all data values (in memory) and erase all data files (in xml) [xml not implemented yet]
}
