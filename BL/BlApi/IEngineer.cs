

using System.Data;

namespace BlApi;

/// <summary>
/// Interface for the Engineer entity
/// </summary>
public interface IEngineer
{
    /// <summary>
    /// Create a new Engineer
    /// </summary>
    /// <param name="e"></param>
    /// <returns>the id of the engineer</returns>
    public int Create(BO.Engineer e);

    /// <summary>
    /// Read an Engineer by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns> the engineer with the given id</returns>
    public BO.Engineer? Read(int id);

    /// <summary>
    /// Read all Engineers
    /// </summary>
    /// <param name="filter"></param>
    /// <returns> a collection of Engineer objects</returns>
    public IEnumerable<BO.Engineer> ReadAll(Func<BO.Engineer, bool>? filter = null);

    /// <summary>
    /// Updates an engineer
    /// </summary>
    /// <param name="e"></param>
    public void Update(BO.Engineer e);

    /// <summary>
    /// Deletes an engineer by ID
    /// </summary>
    /// <param name="id"></param>
    public void Delete(int id);
}