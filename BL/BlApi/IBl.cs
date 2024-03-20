

using DalApi;

namespace BlApi;

/// <summary>
/// Interface for the Business Logic layer
/// </summary>
public interface IBl
{
    public IEngineer Engineer { get; }
    public ITask Task { get; }
    public IMilestone Milestone { get; }
    public IConfig Config { get; }
    public DateTime Clock { get; }
    public void travelForwardsDay();
    public void travelForwardsHour();
    public void travelBackwardDay();
    public void travelBackwardHour();
    public void resetClock();

}
