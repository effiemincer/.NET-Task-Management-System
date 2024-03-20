
using DalApi;

namespace Dal;


public class ConfigImplementation : IConfig
{

    /// <summary>
    /// set new KickStart Date of project
    /// </summary>
    /// <param name="newStartDate"></param>
    public void SetProjectStartDate(DateTime newStartDate)
    {

        DataSource.Config.kickstartDate = newStartDate;
    }

    /// <summary>
    /// Set new end date for project
    /// </summary>
    /// <param name="newEndDate"></param>
    public void SetProjectEndDate(DateTime newEndDate)
    {
        DataSource.Config.endDate = newEndDate;
    }

    //getters
    public DateTime? GetProjectStartDate()
    {
        return DataSource.Config.kickstartDate;
    }

    public DateTime? GetProjectEndDate()
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
        DataSource.Config.isScheduleGenerated = null;
        //clear start and end dates
    }

    void IConfig.SetIsScheduleGenerated(bool isSet)
    {
        DataSource.Config.isScheduleGenerated = isSet;
    }

    bool? IConfig.GetIsScheduleGenerated()
    {
        return DataSource.Config.isScheduleGenerated;
    }
}
