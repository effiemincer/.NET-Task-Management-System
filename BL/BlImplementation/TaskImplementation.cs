using BlApi;
using static BO.Enums;

namespace BlImplementation;

internal class TaskImplementation : ITask
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public int Create(BO.Task boTask)
    {

        if(boTask is null)
            throw new BO.BlArgumentNullException("Task is null");

        if (boTask.Id < 0 || boTask.Alias is null || boTask.Alias == "")
            throw new BO.BlYOUGAVEME BADINFO YOU ARE EVIL("Missing ID or name");

        bool hasMilestone = (boTask.Milestone != null);

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
        DO.Task? doTask = _dal.Task.Read(id);

        if (doTask is null)
            throw new BO.BlDoesNotExistException($"Task with ID={id} does not exist");

        return new BO.Task()
        {
            Id = doTask.Id,
            Alias = doTask.Alias,
            DateCreated = doTask.DateCreated,
            Description = doTask.Description,
            Status = Read(doTask.Id)?.Status ?? BO.Enums.Status.Unscheduled,
            RequiredEffortTime = doTask.Duration,
            ActualStartDate = doTask.ActualStartDate,
            ScheduledStartDate = doTask.ScheduledStartDate,
            ProjectedStartDate = doTask.ScheduledStartDate,
            Deadline = doTask.Deadline,
            ActualEndDate = doTask.ActualEndDate,
            Deliverable = doTask.Deliverable,
            Remarks = doTask.Notes,
            Complexity = (BO.Enums.EngineerExperience?)doTask.DegreeOfDifficulty,
            Inactive = doTask.Inactive
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
}