

namespace DalApi;

public interface IProject
{
    //set start date of porject
    void SetProjectStartDate(DateTime? newStartDate);

    //set end date of project
    void SetProjectEndDate(DateTime? newEndDate);

    //reset everything
    void Reset();

}
