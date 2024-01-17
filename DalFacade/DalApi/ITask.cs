
namespace DalApi;

using DO;

public interface ITask: ICrud<Task>
{
    void Reset(); //erase all data values (in memory) and erase all data files (in xml) [xml not implemented yet]
}
