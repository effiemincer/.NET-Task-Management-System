namespace BlImplementation;
using BlApi;

/// <summary>
/// Implementation of the Business Logic layer
/// </summary>
public class Bl : IBl
{
    public IEngineer Engineer => new EngineerImplementation();

    public ITask Task => new TaskImplementation();

    public IMilestone Milestone => new MilestoneImplementation();

}
