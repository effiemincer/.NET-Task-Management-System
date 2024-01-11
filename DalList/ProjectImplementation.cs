
using DalApi;

namespace Dal;


public class ProjectImplementation : IProject
{

    /// <summary>
    /// set new KickStart Date of project
    /// </summary>
    /// <param name="newStartDate"></param>
    public void SetProjectStartDate(DateTime? newStartDate)
    {

        DataSource.Config.kickstartDate = newStartDate;
    }

    /// <summary>
    /// Set new end date for project
    /// </summary>
    /// <param name="newEndDate"></param>
    public void SetProjectEndDate(DateTime? newEndDate)
    {
        DataSource.Config.endDate = newEndDate;
    }

    //getters
    DateTime? GetProjectStartDate()
    {
        return DataSource.Config.kickstartDate;
    }

    DateTime? GetProjectEndDate()
    {
        return DataSource.Config.endDate;
    }

    /// <summary>
    /// Resets all data
    /// </summary>
    public void Reset()
    {
        DataSource.Engineers.Clear();
        DataSource.Tasks.Clear();  
        DataSource.Dependencies.Clear();
        DataSource.Config.kickstartDate = null;
        DataSource.Config.endDate = null;
        //clear start and end dates
    }
}
