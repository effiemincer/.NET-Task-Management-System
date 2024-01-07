
namespace DalApi;

using DO;

public interface IDependency
{
    int Create(Dependency dependency);
    void Delete(Dependency dependency);
    void Update(Dependency dependency);
    Dependency? Read(int id);
    List<Dependency> ReadAll();
    void Reset();
}
