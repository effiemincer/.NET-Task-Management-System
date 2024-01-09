
namespace DalApi;

using DO;
using System.ComponentModel.DataAnnotations;

public interface IDependency
{
    int Create(Dependency dependency);
    void Delete(int id);
    void Update(Dependency dependency);
    Dependency? Read(int id);
    List<Dependency> ReadAll();

    void Reset(); //erase all data values (in memory) and erase all data files (in xml) [xml not implemented yet]

}
