
//for manipulating the project start 

namespace BlApi;

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
    void Reset();

    public void SetIsScheduleGenerated(bool isScheduleGenerated=false);
    public bool? GetIsScheduleGenerated();

}

