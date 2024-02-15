
using BO;

namespace BlApi;

public interface IMilestone
{
    public List<BO.Task> CreateSchedule(DateTime startDate, DateTime endDate, List<TaskInList> tasks, List<MilestoneInList> milestones, List<DO.Dependency> dependencies);
    public Milestone Read(int id);
    public Milestone Update(int id, string name, string description, string comments);
    public List<Milestone> ReadAll();
}
