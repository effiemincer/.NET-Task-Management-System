

namespace DalApi;

public interface IProject
{
    //set start date of porject
    void SetProjectStartDate(DateTime? newStartDate);

    //set end date of project
    void SetProjectEndDate(DateTime? newEndDate);

    //getters
    DateTime? GetProjectStartDate();

    DateTime? GetProjectEndDate();

    //reset everything
    void Reset();

}
