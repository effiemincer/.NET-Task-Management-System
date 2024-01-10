
namespace DalApi;

using DO;

public interface ITask
{
    int Create(Task task);
    void Delete(int id);
    void Update(Task task);
    Task? Read(int id);
    List<Task> ReadAll();

    void Reset(); //erase all data values (in memory) and erase all data files (in xml) [xml not implemented yet]


    /*
    //WE SHOULD PROBABLY DELETE THESE
    void ProjectKickStartDate(int Id, DateTime? kickStartDate);
    void ProjectEndDate(int Id, DateTime? endDate);
    */
}
