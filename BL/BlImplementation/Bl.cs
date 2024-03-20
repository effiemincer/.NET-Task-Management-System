namespace BlImplementation;
using BlApi;

/// <summary>
/// Implementation of the Business Logic layer
/// </summary>
public class Bl : IBl
{
    public IEngineer Engineer => new EngineerImplementation();

    public ITask Task => new TaskImplementation(this);

    public IMilestone Milestone => new MilestoneImplementation(this);

    public IConfig Config => new ConfigImplementation();

    private static DateTime s_Clock = DateTime.Now;
    public DateTime Clock { get { return s_Clock; } private set { s_Clock = value; } }

    void IBl.travelForwardsDay()
    {
        Clock = Clock.AddDays(1);
    }

    void IBl.travelForwardsHour()
    {
        Clock = Clock.AddHours(1);
    }

    void IBl.travelBackwardDay()
    {
        Clock = Clock.AddDays(-1);
    }

    void IBl.travelBackwardHour()
    {
        Clock = Clock.AddHours(-1);
    }

    void IBl.resetClock()
    {
        Clock = DateTime.Now;
    }
}
