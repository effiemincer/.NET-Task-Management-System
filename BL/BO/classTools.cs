

using DalApi;
using System.Reflection;
using System.Text;

namespace BO;

public static class classTools
{
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



    public static BO.Enums.Status StatusCalculator(DO.Task task)
    {
        if (task.ActualEndDate is not null)
            return BO.Enums.Status.Done;
        else if (task.ActualStartDate is null && task.ProjectedStartDate is null)
            return BO.Enums.Status.Unscheduled;
        else if (task.ActualStartDate is null && task.ProjectedStartDate is not null)
            return BO.Enums.Status.Scheduled;
        else if (task.ActualStartDate is not null && (task.ActualStartDate + task.Duration) <= task.Deadline)
            return BO.Enums.Status.OnTrack;
        else if (task.ActualEndDate is not null && (task.ActualStartDate + task.Duration) > task.Deadline)
            return BO.Enums.Status.InJeopardy;
        else
            throw new BlBadInputDataException("Status could not be calculated for task with ID=" + task.Id);
    }

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
