

namespace BlApi;

public interface IMilestone
{
    public int Create(BO.Milestone item);
    public BO.Milestone Read(int id);
    public IEnumerable<BO.MilestoneInList> ReadAll();
    public void Update(BO.Milestone item);
    public void Delete(int id);
}
