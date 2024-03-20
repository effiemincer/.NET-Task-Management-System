
//for manipulating the project start 

namespace DalApi;

public interface IConfig
{
    //set start date of porject
    void SetProjectStartDate(DateTime newStartDate);

    //set end date of project
    void SetProjectEndDate(DateTime newEndDate);

    //getters
    public DateTime? GetProjectStartDate();

    public DateTime? GetProjectEndDate();

    //reset everything
    public void Reset();

    public void SetIsScheduleGenerated(bool isSet=false);
    public bool? GetIsScheduleGenerated();

}

