
namespace DalApi;

using DO;


public interface IEngineer
{
    int Create(Engineer engineer);
    void Delete(int id);
    void Update(Engineer engineer);
    Engineer? Read(int id);
    List<Engineer> ReadAll();
}
