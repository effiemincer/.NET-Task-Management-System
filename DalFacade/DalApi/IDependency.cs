
namespace DalApi;

using DO;


public interface IDependency : ICrud<Dependency>
{
    void Reset(); //erase all data values (in memory) and erase all data files (in xml) [xml not implemented yet]
}
