
using DalApi;

namespace BlImplementation;

internal class ConfigImplementation : BlApi.IConfig
{
    static readonly IDal? s_dal = Factory.Get;

    public DateTime? GetProjectEndDate()
    {
        return s_dal?.Config.GetProjectEndDate();
    }

    public DateTime? GetProjectStartDate()
    {
        return s_dal?.Config.GetProjectStartDate();
    }

    public void Reset()
    {
        s_dal?.Config.Reset();

    }

    public void SetProjectEndDate(DateTime newEndDate)
    {
        s_dal?.Config.SetProjectEndDate(newEndDate);
    }

    public void SetProjectStartDate(DateTime newStartDate)
    {
        s_dal?.Config.SetProjectStartDate(newStartDate);
    }

    public bool? GetIsScheduleGenerated()
    {
        return s_dal?.Config?.GetIsScheduleGenerated();
    }

    public void SetIsScheduleGenerated(bool isScheduleGenerated=false)
    {
        s_dal?.Config.SetIsScheduleGenerated(isScheduleGenerated);
    }
}
