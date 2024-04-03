

namespace BlApi;

/// <summary>
/// Interface for the Task Business Logic
/// </summary>
public interface ITask
{
    /// <summary>
    /// Create a new Task
    /// </summary>
    /// <param name="item"></param>
    /// <returns> the id of the task</returns>
    public int Create(BO.Task item);

    /// <summary>
    /// Read a Task by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns> the task with the given id</returns>
    public BO.Task? Read(int id);

    /// <summary>
    /// Read all Tasks
    /// </summary>
    /// <param name="filter"></param>
    /// <returns> a collection of Task objects</returns>
    public IEnumerable<BO.TaskInList> ReadAll(Func<BO.Task, bool>? filter = null);

    /// <summary>
    /// Updates a task
    /// </summary>
    /// <param name="item"></param>
    /// <returns> a Task object representing the updated task</returns>
    public BO.Task Update(BO.Task? item);

    /// <summary>
    /// DEletes a task by ID
    /// </summary>
    /// <param name="id"></param>
    public void Delete(int id);

    /// <summary>
    /// Updates the projected start date of a task
    /// </summary>
    /// <param name="id"></param>
    /// <param name="newDateTime"></param>
    public void UpdateProjectedStartDate(int id, DateTime? newDateTime);


    public List<int> findDependants(int id);

    public void finishTask(int engID, int taskId);

    public void assignEng(int engId, int taskId);

}
