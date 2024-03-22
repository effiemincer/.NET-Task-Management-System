

using DalApi;
using System.Reflection;
using System.Text;

namespace BO;

/// <summary>
/// Helper class for the Business Logic layer
/// </summary>
public static class classTools
{

    /// <summary>
    /// String representation of an object's properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns> a string representing the object's properties</returns>
    public static string ToStringProperty<T>(this T obj)
    {
        if (obj == null)
        {
            return "null object";
        }

        StringBuilder stringBuilder = new StringBuilder();
        Type type = typeof(T);
        PropertyInfo[] properties = type.GetProperties();

        stringBuilder.Append($"\n{type.Name} properties:\n");

        foreach (PropertyInfo property in properties)
        {
            object value = property.GetValue(obj) ?? "";
            stringBuilder.Append($"{property.Name}: {value ?? "null"}\n");
        }

        return stringBuilder.ToString();
    }


    /// <summary>
    /// Status Calculator for a Task
    /// </summary>
    /// <param name="task"><param>
    /// <returns> the status of the task</returns>
    /// <exception cref="BlBadInputDataException"></exception>
    public static BO.Enums.Status StatusCalculator(DO.Task task)
    {
        if (task.ActualEndDate is not null)
            return BO.Enums.Status.Done;
        else if (task.ActualStartDate is null && task.ProjectedStartDate is null)
        {
            return BO.Enums.Status.Unscheduled;
        }
            
        else if (task.ActualStartDate is null && task.ProjectedStartDate is not null)
        {
            return BO.Enums.Status.Scheduled;
        } 
        else if (task.ActualStartDate is not null && (task.ActualStartDate + task.Duration) <= task.Deadline)
            return BO.Enums.Status.OnTrack;
        else if (task.ActualStartDate is not null && (task.ActualStartDate + task.Duration) > task.Deadline)
            return BO.Enums.Status.InJeopardy;
        else if (task.ActualStartDate is not null && task.Deadline is null)
            return BO.Enums.Status.OnTrack;
        else
            throw new BlBadInputDataException("Status could not be calculated for task with ID=" + task.Id);



    }

    /// <summary>
    /// Dependencies Calculator for a Task
    /// </summary>
    /// <param name="doTask"></param>
    /// <param name="_dal"></param>
    /// <returns> a list of tasks that the given task is dependent on</returns>
    public static List<BO.TaskInList> DependenciesCalculator(DO.Task doTask, DalApi.IDal _dal)
    {
        //dependencies calculation
        IEnumerable<DO.Dependency?> dependencies = _dal.Dependency.ReadAll(dep => dep.DependentTaskId == doTask.Id);

        IEnumerable<BO.TaskInList?> dependenciesList = from DO.Dependency? dep in dependencies
                                                       where (dep != null && dep.RequisiteID != null)
                                                      select new BO.TaskInList
                                                      {
                                                          Id = (int)dep.RequisiteID!,
                                                          Description = _dal.Task.Read((int)dep.RequisiteID)!.Description,
                                                          Alias = _dal.Task.Read((int)dep.RequisiteID)!.Alias,
                                                          Status = StatusCalculator(_dal.Task.Read((int)dep.RequisiteID)!)
                                                      };

        return dependenciesList!.ToList<BO.TaskInList>();
    }
}
