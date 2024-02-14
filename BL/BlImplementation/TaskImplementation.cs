using BlApi;
using BO;

namespace BlImplementation;

internal class TaskImplementation : ITask
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    //add logic for calculated fields

    public int Create(BO.Task boTask)
    {

        if(boTask is null)
            throw new BO.BlArgumentNullException("Task is null");

        if (boTask.Alias is null || boTask.Description is null || boTask.Milestone == null)
            throw new BO.BlNullPropertyException("Alias or Description is null");

        if (boTask.Id < 0 || boTask.Alias == "" || boTask.Description == "")
            throw new BO.BlBadInputDataException("Missing ID or name");

        //check if the dependent task ID's exist throw error if not
        //if it does exist then create a dependency and pass it down

        try
        {
            DO.Task doTask = new DO.Task
        (
            boTask.Id,
            boTask.Alias ?? "",
            boTask.DateCreated,
            boTask.Description ?? "",
            boTask!.RequiredEffortTime,
            boTask!.Deadline,
            boTask!.ProjectedStartDate,
            (DO.Enums.EngineerExperience?)boTask?.Complexity,
            boTask!.Engineer?.Id,
            boTask!.ActualEndDate,
            (boTask!.Milestone != null),
            boTask!.ActualStartDate,
            boTask!.Deliverable,
            boTask!.Remarks
        );  
            int idTask = _dal.Task.Create(doTask);
            return idTask;
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Task with ID={boTask!.Id} already exists", ex);
        }
    }

    public void Delete(int id)
    {

        try
        {
            _dal.Task.Delete(id);

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Task with ID={id} does not exist", ex);
        }   
        
    }

    public BO.Task? Read(int id)
    {
        if (id < 0)
            throw new BO.BlBadInputDataException("ID must be positive");

        DO.Task? doTask = _dal.Task.Read(id);

        if (doTask is null)
            throw new BO.BlDoesNotExistException($"Task with ID={id} does not exist");

        //status calculation
        BO.Enums.Status stat = StatusCalculator(doTask);

        //projected start date calculation
        DateTime? projectedStart = doTask.Deadline - doTask.Duration;

        //dependencies calculation
        IEnumerable<DO.Dependency?> dependencies = _dal.Dependency.ReadAll(dep => dep.DependentTaskId == doTask.Id);

        /*
        //if (dependencies is null)
        //{
        //    return new BO.Task()
        //    {
        //        Id = doTask.Id,
        //        Alias = doTask.Alias,
        //        DateCreated = doTask.DateCreated,
        //        Description = doTask.Description,
        //        Status = stat,
        //        RequiredEffortTime = doTask.Duration,
        //        ActualStartDate = doTask.ActualStartDate,
        //        ScheduledStartDate = doTask.ScheduledStartDate,
        //        ProjectedStartDate = projectedStart,
        //        Deadline = doTask.Deadline,
        //        ActualEndDate = doTask.ActualEndDate,
        //        Deliverable = doTask.Deliverable,
        //        Remarks = doTask.Notes,
        //        Complexity = (BO.Enums.EngineerExperience?)doTask.DegreeOfDifficulty
        //    };
        //}

        //else
        //{
        */
        IEnumerable<BO.TaskInList> dependenciesList = from DO.Dependency dep in dependencies
                                                        select new BO.TaskInList
                                                        {
                                                            Id = (int)dep.RequisiteID!,
                                                            Description = _dal.Task.Read((int)dep.RequisiteID)!.Description,
                                                            Alias = _dal.Task.Read((int)dep.RequisiteID)!.Alias,
                                                            Status = StatusCalculator(_dal.Task.Read((int)dep.RequisiteID)!)
                                                        };

        return new BO.Task()
        {
            Id = doTask.Id,
            Alias = doTask.Alias,
            DateCreated = doTask.DateCreated,
            Description = doTask.Description,
            Status = stat,
            Dependencies = dependenciesList.ToList<BO.TaskInList>() ?? null,
            RequiredEffortTime = doTask.Duration,
            ActualStartDate = doTask.ActualStartDate,
            ScheduledStartDate = doTask.ScheduledStartDate,
            ProjectedStartDate = projectedStart,
            Deadline = doTask.Deadline,
            ActualEndDate = doTask.ActualEndDate,
            Deliverable = doTask.Deliverable,
            Remarks = doTask.Notes,
            Complexity = (BO.Enums.EngineerExperience?)doTask.DegreeOfDifficulty
        };
    }

    public IEnumerable<BO.TaskInList> ReadAll(Func<BO.Task, bool>? filter = null)
    {
        IEnumerable<BO.TaskInList> tasks;

        if (filter != null) { 
        tasks = from DO.Task doTask in _dal.Task.ReadAll()
                where filter(Read(doTask.Id)!)
                select new BO.TaskInList
                {
                    Id = doTask.Id,
                    Description = doTask.Description,
                    Alias = doTask.Alias,
                    Status = Read(doTask.Id)?.Status ?? BO.Enums.Status.Unscheduled
                };
    }

        else { 
            tasks = from DO.Task doTask in _dal.Task.ReadAll()
                    select new BO.TaskInList
                    {
                        Id = doTask.Id,
                        Description = doTask.Description,
                        Alias = doTask.Alias,
                        Status = Read(doTask.Id)?.Status ?? BO.Enums.Status.Unscheduled
                    };
        }
        return tasks;
}

    //fix this, it doesn't make sense i.e let's say you wanted to update the status of a task it wouldn't save anywhere
    public void Update(BO.Task? boTask)
    {
        if (boTask is null)
            throw new BO.BlArgumentNullException("Task is null");

        //test logic of all the fields
        bool hasMilestone = (boTask.Milestone is not null);

        DO.Task doTask = new DO.Task
        (
            boTask.Id,
            boTask.Alias ?? "",
            boTask.DateCreated,
            boTask.Description ?? "",
            boTask?.RequiredEffortTime,
            boTask?.Deadline,
            boTask?.ProjectedStartDate,
            (DO.Enums.EngineerExperience?)boTask?.Complexity,
            boTask?.Engineer?.Id,
            boTask?.ActualEndDate,
            hasMilestone,
            boTask?.ActualStartDate,
            boTask?.Deliverable,
            boTask?.Remarks
        );

        try
        {
            _dal.Task.Update(doTask);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Task with ID={boTask!.Id} already exists", ex);
        }
    }


    private BO.Enums.Status StatusCalculator(DO.Task task)
    {
        if (task.ActualStartDate is null && task.ScheduledStartDate is null)
            return BO.Enums.Status.Unscheduled;
        else if (task.ActualStartDate is null && task.ScheduledStartDate is not null)
            return BO.Enums.Status.Scheduled;
        else if (task.ActualStartDate is not null && (task.ActualStartDate + task.Duration) <= task.Deadline)
            return BO.Enums.Status.OnTrack;
        else if (task.ActualEndDate is not null && (task.ActualStartDate + task.Duration) > task.Deadline)
            return BO.Enums.Status.InJeopardy;
        else if (task.ActualEndDate is not null)
            return BO.Enums.Status.Done;
        else
            throw new BlBadInputDataException("Status could not be calculated for task with ID=" + task.Id);
    }

}