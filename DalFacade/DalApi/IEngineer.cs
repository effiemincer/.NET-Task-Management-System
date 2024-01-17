
namespace DalApi;

using DO;


public interface IEngineer : ICrud<Engineer>
{
    void Reset(); //erase all data values (in memory) and erase all data files (in xml) [xml not implemented yet]
}
