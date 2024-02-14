using BlApi;

namespace BlImplementation;

internal class TaskImplementation : ITask
{
    private DalApi.IDal _dal = DalApi.Factory.Get;

    public int Create(BO.Task item)
    {

        //test logic of all the fields

        DO.Task boTask = new DO.Task
        {

        };
        //{
        //    Id = item.Id,
        //    Alias = item.Alias,
        //    DateCreated = item.DateCreated,
        //    Description = item.Description,
        //    Status = item.Status,
        //    Dependencies = item?.Dependencies,
        //    Milestone = item?.Milestone,
        //    RequiredEffortTime = item?.RequiredEffortTime,
        //    ActualStartDate = item?.ActualStartDate,
        //    ScheduledStartDate = item?.ScheduledStartDate,
        //    ProjectedStartDate = item?.ProjectedStartDate,
        //    Deadline = item?.Deadline,
        //    ActualEndDate = item?.ActualEndDate,
        //    Deliverable = item?.Deliverable,
        //    Remarks = item?.Remarks,
        //    Engineer = item?.Engineer,
        //    Complexity = item?.Complexity,
        //    Inactive = item!.Inactive
        //};

        try
        {
            int idTask = _dal.Task.Create(newTask);
        }

        return newTask.Id;
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public BO.Task Read(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.TaskInList> ReadAll()
    {
        throw new NotImplementedException();
    }

    public void Update(BO.Task item)
    {
        throw new NotImplementedException();
    }
}
