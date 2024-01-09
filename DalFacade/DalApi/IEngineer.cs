
namespace DalApi;

using DO;


public interface IEngineer
{
    int Create(Engineer engineer);
    void Delete(int id);
    void Update(Engineer engineer);
    Engineer? Read(int id);
    List<Engineer> ReadAll();

    void Reset(); //erase all data values (in memory) and erase all data files (in xml) [xml not implemented yet]
}
