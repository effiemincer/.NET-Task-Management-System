namespace Dal;
using DO;
using DalApi;

public class TaskImplementation : ITask
{
    public int Create(Task task)
    {
        int Id = DataSource.Config.NextITaskId;
        if(DataSource.Tasks.Any(taskItem => taskItem.Id == Id))
        {
            throw new Exception("object with that id already exists!");
        }
        // just doing non-preset feilds here not sure if were supposed to do that
        // until we have more constructors

        Task taskCopy = new Task(
            Id,
            task.Nickname,
            task.DateCreated,
            task?.Description ?? "",
            task?.Duration ?? 0,
            task?.Deadline,
            task?.ProjectedStartDate,
            task?.DegreeOfDifficulty,
            task?.AssignedEngineerId ?? 0,
            task?.ActualEndDate ?? DateTime.MinValue,
            task?.IsMilestone ?? false,
            task?.ActualStartDate ?? DateTime.MinValue,
            task?.Deliverable ?? "",
            task?.Notes ?? ""
        );
        DataSource.Tasks.Add( taskCopy );
        return Id;

        // throw new NotImplementedException();
    }

    public Task? Read(int id)
    {
        Task? foundTask= DataSource.Tasks.FirstOrDefault(task => task.Id == id && task.Inactive == false);

        // If an object with the specified Id exists, return a reference to the object; otherwise, return null
        return foundTask;
    }

    public List<Task> ReadAll()
    {
        return new List<Task>(DataSource.Tasks.FindAll(i => i.Inactive is not true));
    }

    public void Update(Task task)
    {
        int index = DataSource.Tasks.FindIndex(t => t.Id == task.Id && t.Inactive == false);
        if (index == -1)
        {
            throw new Exception($"object of type Task with identifier {task.Id} does not exist");
        }

        // Removes the old task
        DataSource.Tasks.RemoveAt(index);

        // Add the updated task
        DataSource.Tasks.Insert(index, task);

    }

    public void Delete(int id)
    {
        int index = DataSource.Tasks.FindIndex(t => t.Id == id);
        if (index == -1)
        {
            throw new Exception($"object of type Task with identifier {id} does not exist");
        }

        Task inactiveTask = DataSource.Tasks[index] with { Inactive = true };

        DataSource.Tasks.RemoveAt(index);

        // Add the inactive task
        DataSource.Tasks.Add(inactiveTask);

    }

    public void Reset()
    {
        DataSource.Tasks.Clear();
    }

    public void ProjectKickStartDate(int id, DateTime? kickStartDate)
    {
        int index = DataSource.Tasks.FindIndex(t => t.Id == id);
        if (index == -1)
        {
            throw new Exception($"object of type Task with identifier {id} does not exist");
        }
        
        //variable just for easy name
        List<DO.Task> listOfTasks = DataSource.Tasks;
        Task l = listOfTasks[index];

        //creating new task with only dateStart changed
        Task updatedTask = new Task(l.Id, l.Nickname, l.DateCreated, l.Description, l.Duration, l.Deadline, kickStartDate, l.DegreeOfDifficulty, l.AssignedEngineerId,
            l.ActualEndDate, l.IsMilestone, l.ActualStartDate, l.Deliverable, l.Notes, l.Inactive); ;

        //remove old dated task
        DataSource.Tasks.RemoveAt(index);

        // Add the updated task
        DataSource.Tasks.Insert(index, updatedTask);

    }

    public void ProjectEndDate(int id, DateTime? endDate)
    {
        int index = DataSource.Tasks.FindIndex(t => t.Id == id);
        if (index == -1)
        {
            throw new Exception($"object of type Task with identifier {id} does not exist");
        }

        //variable just for easy name
        List<DO.Task> listOfTasks = DataSource.Tasks;
        Task l = listOfTasks[index];

        //creating new task with only dateStart changed
        Task updatedTask = new Task(l.Id, l.Nickname, l.DateCreated, l.Description,l.Duration, l.Deadline, l.ProjectedStartDate, 
            l.DegreeOfDifficulty, l.AssignedEngineerId, endDate, l.IsMilestone, l.ActualStartDate, l.Deliverable,l.Notes, l.Inactive);

        //remove old dated task
        DataSource.Tasks.RemoveAt(index);

        // Add the updated task
        DataSource.Tasks.Insert(index, updatedTask);
    }
}
