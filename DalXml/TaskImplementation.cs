using DalApi;
using DO;
using System.Threading.Tasks;

namespace Dal;

internal class TaskImplementation : ITask
{
    // XML file name for tasks
    readonly string s_tasks_xml = "tasks";

    // Create a new Task in the system
    public int Create(DO.Task entity)
    {
        // Load existing tasks from XML
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");

        // Generate a new ID for the Task
        int Id = Config.NextTaskId;

        // Check if a Task with the generated ID already exists
        if (Tasks.Any(taskItem => taskItem.Id == Id))
        {
            throw new DalAlreadyExistsException("Object with that id already exists!");
        }

        // Create a copy of the Task with the new ID
        DO.Task taskCopy = new DO.Task(
            Id,
            entity.Alias,
            entity.DateCreated,
            entity.Description,
            entity?.Duration,
            entity?.Deadline,
            entity?.ScheduledStartDate,
            entity?.DegreeOfDifficulty,
            entity?.AssignedEngineerId,
            entity?.ActualEndDate,
            entity?.IsMilestone ?? false,
            entity?.ActualStartDate,
            entity?.Deliverable,
            entity?.Notes
        );

        // Add the new Task to the list
        Tasks.Add(taskCopy);

        // Save the updated list to XML
        XMLTools.SaveListToXMLSerializer<DO.Task>(Tasks, "tasks");

        // Return the generated Task's ID
        return Id;
    }

    // Soft delete a Task by marking it as inactive
    public void Delete(int id)
    {
        // Load existing tasks from XML
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");

        // Find the index of the Task to delete
        int index = Tasks.FindIndex(t => t.Id == id);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"Object of type Task with identifier {id} does not exist");
        }

        // Create a new inactive Task
        DO.Task inactiveTask = Tasks[index] with { Inactive = true };

        // Remove the old Task
        Tasks.RemoveAt(index);

        // Add the new inactive Task
        Tasks.Add(inactiveTask);

        // Save the empty list to XML
        XMLTools.SaveListToXMLSerializer<DO.Task>(Tasks, "tasks");

    }

    // Read a specific Task by ID
    public DO.Task? Read(int id)
    {
        // Load existing tasks from XML
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");

        // Find the Task with the specified ID that is not inactive
        DO.Task? foundTask = Tasks.FirstOrDefault(task => task.Id == id && task.Inactive == false);

        // Return the found Task or null if not found
        return foundTask;
    }

    // Read a Task based on a filter predicate
    public DO.Task? Read(Func<DO.Task, bool> filter)
    {
        // Load existing tasks from XML
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");

        // Check if the filter is null
        if (filter == null)
        {
            return null;
        }

        // Return the first Task that matches the filter
        return Tasks.FirstOrDefault(filter);
    }

    // Read all Tasks based on an optional filter
    public IEnumerable<DO.Task?> ReadAll(Func<DO.Task, bool>? filter = null)
    {
        // Load existing tasks from XML
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");

        // Check if a filter is provided
        if (filter != null)
        {
            // Return filtered Tasks that are not inactive
            return from item in Tasks
                   where filter(item) && !item.Inactive
                   select item;
        }

        // Return all Tasks that are not inactive
        return from item in Tasks
               where !item.Inactive
               select item;
    }

    // Reset the list of Tasks, clearing all entries
    public void Reset()
    {
        // Load existing tasks from XML
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");

        // Clear the list of Tasks
        Tasks.Clear();

        // Save the empty list to XML
        XMLTools.SaveListToXMLSerializer<DO.Task>(Tasks, "tasks");
    }

    // Update an existing Task
    public void Update(DO.Task entity)
    {
        // Load existing tasks from XML
        List<DO.Task> Tasks = XMLTools.LoadListFromXMLSerializer<DO.Task>("tasks");

        // Find the index of the Task to update
        int index = Tasks.FindIndex(t => t.Id == entity.Id && t.Inactive == false);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"Object of type Task with identifier {entity.Id} does not exist");
        }

        // Remove the old Task
        Tasks.RemoveAt(index);

        // Add the updated Task
        Tasks.Insert(index, entity);

        // Save the updated list to XML
        XMLTools.SaveListToXMLSerializer<DO.Task>(Tasks, "tasks");
    }
}
