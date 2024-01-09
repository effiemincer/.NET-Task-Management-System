
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
        return new List<Task>(DataSource.Tasks);
    }

    public void Update(Task task)
    {
        int index = DataSource.Tasks.FindIndex(t => t.Id == task.Id && t.Inactive == false);
        if (index == -1)
        {
            throw new Exception($"object of type Task with identifier {task.Id} does not exist");
        }

        //print out object
        Console.WriteLine(task);
        Console.WriteLine("\nEnter updated information below:\n");

        //Collects Updated information from User - if input is blank then do not change
        Console.WriteLine("Enter name of task: ");
        string? input = Console.ReadLine();
        string name = (input == "" || input is null) ? task.Nickname : input;

        Console.WriteLine("Date Created (mm/dd/yyyy): ");
        input = Console.ReadLine();
        DateTime dateCreated = input == "" ? task.DateCreated : DateTime.Parse(input!);

        Console.WriteLine("Enter description of task: ");
        input = Console.ReadLine();
        string? description = (input == "" || input is null) ? task.Description : input;

        Console.WriteLine("Enter duration of task (hours): ");
        input = Console.ReadLine();
        int? duration;

        if (input == "") duration = task.Duration;
        else duration = Convert.ToInt32(input);

        Console.WriteLine("Enter deadline of task (mm/dd/yyyy): ");
        input = Console.ReadLine();
        DateTime? deadline = (input == "" ? task.Deadline : DateTime.Parse(input!));

        Console.WriteLine("Enter projected start date of task (mm/dd/yyyy): ");
        input = Console.ReadLine();
        DateTime? projectedStart = (input == "" ? task.ProjectedStartDate : DateTime.Parse(input!));

        Console.WriteLine("Enter difficulty of task (0-4): ");
        //list of enums and variables 
        Enums.EngineerExperience[] allDifficulties = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));
        Enums.EngineerExperience? difficulty;

        //user input
        input = Console.ReadLine();

        if (input == "") difficulty = task.DegreeOfDifficulty;
        else difficulty = allDifficulties[Convert.ToInt32(input)];

        //DO WE NEED TO check if the engineer is in our list of engineers?
        Console.WriteLine("Enter ID of assigned Enginner of task: ");
        input = Console.ReadLine();
        int? assignedEng;

        if (input == "") assignedEng = task.AssignedEngineerId;
        else assignedEng = Convert.ToInt32(input);

        Console.WriteLine("Enter actual end date of task (mm/dd/yyyy): ");
        input = Console.ReadLine();
        DateTime? actualEnd = (input == "" ? task.ActualEndDate : DateTime.Parse(input!));

        Console.WriteLine("Enter whether task is a milestone (Y/N): ");
        input = Console.ReadLine();
        bool isMilestone = ((input! == "") ? task.IsMilestone : (input! == "Y"));   //if input is blank then leave as previous value otherwise based on new input


        Console.WriteLine("Enter actaul start date of task (mm/dd/yyyy): ");
        input = Console.ReadLine();
        DateTime? actualStart = (input == "" ? task.ActualStartDate : DateTime.Parse(input!));

        Console.WriteLine("Enter deliverable of task: ");
        input = Console.ReadLine();
        string? deliverable = (input == "" || input is null) ? task.Deliverable : input;

        Console.WriteLine("Enter notes for the task: ");
        input = Console.ReadLine();
        string? notes = (input == "" || input is null) ? task.Notes : input;

        Console.WriteLine("Enter whether task is inactive (Y/N): ");
        input = Console.ReadLine();
        bool inactive = ((input! == "") ? task.Inactive : (input! == "Y"));

        Task updatedTask = new Task(task.Id, name, dateCreated, description, duration, deadline,
                                projectedStart, difficulty, assignedEng, actualEnd,
                                isMilestone, actualStart, deliverable!, notes!, inactive);

        // Remove the old task
        DataSource.Tasks.RemoveAt(index);

        // Add the updated task
        DataSource.Tasks.Insert(index, updatedTask);

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
    
}
