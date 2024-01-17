using DalApi;
using DO;
using System.Threading.Tasks;

namespace Dal;

internal class TaskImplementation : ITask
{
    readonly string s_tasks_xml = "tasks";

    public int Create(DO.Task entity)
    {
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");
        int Id = Config.NextTaskId;
        if (Tasks.Any(taskItem => taskItem.Id == Id))
        {
            throw new DalAlreadyExistsException("object with that id already exists!");
        }
        // just doing non-preset feilds here not sure if were supposed to do that
        // until we have more constructors

        DO.Task taskCopy = new DO.Task(
            Id,
            entity.Nickname,
            entity.DateCreated,
            entity.Description,
            entity?.Duration,
            entity?.Deadline,
            entity?.ProjectedStartDate,
            entity?.DegreeOfDifficulty,
            entity?.AssignedEngineerId,
            entity?.ActualEndDate,
            entity?.IsMilestone ?? false,
            entity?.ActualStartDate,
            entity?.Deliverable,
            entity?.Notes
        );

        Tasks.Add(taskCopy);

        XMLTools.SaveListToXMLSerializer<DO.Task>(Tasks, "tasks");
        return Id;
    }

    public void Delete(int id)
    {
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");
        int index = Tasks.FindIndex(t => t.Id == id);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"object of type Task with identifier {id} does not exist");
        }

        DO.Task inactiveTask = Tasks[index] with { Inactive = true };

        Tasks.RemoveAt(index);

        // Add the inactive task
        Tasks.Add(inactiveTask);
    }

    public DO.Task? Read(int id)
    {
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");
        DO.Task? foundTask = Tasks.FirstOrDefault(task => task.Id == id && task.Inactive == false);

        // If an object with the specified Id exists, return a reference to the object; otherwise, return null
        return foundTask;
    }

    public DO.Task? Read(Func<DO.Task, bool> filter)
    {
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");
        if (filter == null)
        {
            return null;
        }
        return Tasks.FirstOrDefault(filter);
    }

    public IEnumerable<DO.Task?> ReadAll(Func<DO.Task, bool>? filter = null)
    {
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");
        if (filter != null)
        {
            return from item in Tasks
                   where filter(item) && !item.Inactive
                   select item;
        }
        return from item in Tasks
               where !item.Inactive
               select item;
    }

    public void Reset()
    {
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");
        Tasks.Clear();
        XMLTools.SaveListToXMLSerializer<DO.Task>(Tasks, "tasks");
    }

    public void Update(DO.Task entity)
    {
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");
        int index = Tasks.FindIndex(t => t.Id == entity.Id && t.Inactive == false);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"object of type Task with identifier {entity.Id} does not exist");
        }

        // Removes the old task
        Tasks.RemoveAt(index);

        // Add the updated task
        Tasks.Insert(index, entity);
        XMLTools.SaveListToXMLSerializer<DO.Task>(Tasks, "tasks");
    }
}
