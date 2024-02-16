
using BO;

namespace BlApi;

/// <summary>
/// Interface for the Milestone entity
/// </summary>
public interface IMilestone
{
    /// <summary>
    /// Create a project Schedule
    /// </summary>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <returns> a string representing the schedule</returns>
    public string CreateSchedule(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Read a Milestone by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns> the Milestone with the given id</returns>
    public Milestone Read(int id);

    /// <summary>
    /// Updates select fields of a milestone
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="comments"></param>
    /// <returns> a Milestone object representing the updated milestone</returns>
    public Milestone Update(int id, string name, string description, string comments);
}
