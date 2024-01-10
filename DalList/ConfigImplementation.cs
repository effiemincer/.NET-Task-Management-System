
using DalApi;

namespace Dal;

public class ConfigImplementation : IConfig
{
    public void Reset()
    {
        DataSource.Engineers.Clear();
        DataSource.Tasks.Clear();  
        DataSource.Dependencies.Clear();
        //clear start and end dates
    }
}
