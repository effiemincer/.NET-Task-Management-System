
namespace DalApi;

using DO;


public interface IEngineer
{
    int Create(Engineer engineer);
    void Delete(Engineer engineer);
    void Update(Engineer engineer);
    Engineer? Read(int id);
    List<Engineer> ReadAll();
    void Reset();
}
