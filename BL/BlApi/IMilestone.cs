
using BO;

namespace BlApi;

public interface IMilestone
{
    public string CreateSchedule(DateTime startDate, DateTime endDate);
    public Milestone Read(int id);
    public Milestone Update(int id, string name, string description, string comments);
}
