namespace Dal;
using DalApi;

sealed internal class DalList: IDal
{
    public IDependency Dependency => new DependencyImplementation();
    public IEngineer Engineer => new EngineerImplementation();
    public ITask Task => new TaskImplementation();
    public IConfig Config => new ConfigImplementation();

    public static IDal Instance { get; } = new DalList();
    private DalList() { }

}
