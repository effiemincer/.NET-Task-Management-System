namespace Dal;
using DO;
using DalApi;

internal class TaskImplementation : ITask
{
    public int Create(Task task)
    {
        int Id = DataSource.Config.NextITaskId;
        if(DataSource.Tasks.Any(taskItem => taskItem.Id == Id))
        {
            throw new DalAlreadyExistsException("object with that id already exists!");
        }
        // just doing non-preset feilds here not sure if were supposed to do that
        // until we have more constructors

        Task taskCopy = new Task(
            Id,
            task.Nickname,
            task.DateCreated,
            task.Description,
            task?.Duration,
            task?.Deadline,
            task?.ProjectedStartDate,
            task?.DegreeOfDifficulty,
            task?.AssignedEngineerId,
            task?.ActualEndDate,
            task?.IsMilestone ?? false,
            task?.ActualStartDate,
            task?.Deliverable,
            task?.Notes
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

    public Task? Read(Func<Task, bool> filter)
    {
        if (filter == null)
        {
            return null;
        }
        return DataSource.Tasks.FirstOrDefault(filter);
    }

    //public List<Task> ReadAll()
    //{
    //    return new List<Task>(DataSource.Tasks.FindAll(i => i.Inactive is not true));
    //}

    public IEnumerable<Task> ReadAll(Func<Task, bool>? filter = null)
    {
        if (filter != null)
        {
            return from item in DataSource.Tasks
                   where filter(item) && !item.Inactive
                   select item;
        }
        return from item in DataSource.Tasks
               where !item.Inactive
               select item;
    }

    public void Update(Task task)
    {
        int index = DataSource.Tasks.FindIndex(t => t.Id == task.Id && t.Inactive == false);
        if (index == -1)
        {
            throw new DalDoesNotExistException($"object of type Task with identifier {task.Id} does not exist");
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
            throw new DalDoesNotExistException($"object of type Task with identifier {id} does not exist");
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

}
