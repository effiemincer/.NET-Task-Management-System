
namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using System.Collections.Generic;
using System.Text;

internal class MilestoneImplementation : IMilestone
{
    private static Dictionary<int, MilestoneDictItem> MilestoneDict = new Dictionary<int, MilestoneDictItem>();
    private static bool initialized = false;
    
    private DalApi.IDal _dal = DalApi.Factory.Get;
    private class MilestoneDictItem
    { 
        public List<int> idList {  get; set; }
        public List<int> milestoneDef {  get; set; }
        public bool isStart { get; set; }
        public bool isEnd { get; set; }
    }   

    private Dictionary<int, List<int>> CreateDependencyGroups()
    {
        IEnumerable<DO.Dependency>? dependencies = _dal.Dependency.ReadAll();
        Dictionary<int, List<int>> depGroups = new Dictionary<int, List<int>>();

        if (dependencies.Count() <= 0 )
        {
            throw new Exception("no dependencies!");
        }

        foreach (var dep in dependencies)
        {
            if (dep.DependentTaskId == null) throw new Exception("no dependent task id");
            if (dep.RequisiteID == null) throw new Exception("no requisite task id");
            if (!depGroups.ContainsKey(((int)dep.DependentTaskId)))
            {
                depGroups[(int)dep.DependentTaskId] = new List<int>();
                depGroups[(int)dep.DependentTaskId].Add((int)dep.RequisiteID);
            }
           else
            {
                depGroups[(int)dep.DependentTaskId].Add((int)dep.RequisiteID);
                depGroups[(int)dep.DependentTaskId] = sortList(depGroups[(int)dep.DependentTaskId]);
            }
        }

        return depGroups;
    }

    private void InitMilestoneDict()
    {
        // two things #1 when a new task or dependancy are created milestone has to be cleared and initialized = false
        // #2 when creating the milestone dict do we need the ids of the milestones in memory or do we not care?
        // additional question when a milestone task is put into the memory do we replace its dependants' dependency list with this id?
        // or another way to do it is to always build the milestone regularly  but then at the end check milestone objects and replace the keys in our milestone dict with the appropriate id
        // it seems like they want the first option based on what i wrote down on the bottom however i may end up just ignoring that anyways for flexability reasons it might be better to do

        if (initialized) return;

        Dictionary<int, List<int>> dependencyGroups = CreateDependencyGroups();
        if (dependencyGroups.Count == 0) throw new Exception("no dependencies");
        // rewrite this to handle case of no dependencies

        //this needs a filter to filter out tasks that are milestones
        IEnumerable<DO.Task>? tasks = _dal.Task.ReadAll();
        if (tasks.Count() == 0) throw new Exception("no Tasks");

        

        int marker = 1;

        foreach (var depTask in dependencyGroups)
        {
            if (isInMileStoneDictIdList(depTask.Key)) continue;

            MilestoneDict[marker] = new MilestoneDictItem()
            {
                idList = new List<int>(depTask.Key),
                milestoneDef = new List<int>(depTask.Value),
                isStart = false,
                isEnd = false
            };

            foreach (var depTask2 in dependencyGroups)
            {
                if (depTask.Key == depTask2.Key) continue;
                if (depTask.Value == depTask2.Value)
                {
                    MilestoneDict[marker].idList.Add(depTask2.Key);
                }
            }

            ++marker;
        }

        //start and end milestones
        foreach(var task in tasks)
        {
            if(!isInMileStoneDictIdList(task.Id))
            {
                MilestoneDict[0] = new MilestoneDictItem()
                {
                    idList = new List<int>(task.Id),
                    milestoneDef = new List<int>(),
                    isStart = true,
                    isEnd= false
                };
            }
            if (!isInMileStoneDictDef(task.Id))
            {
                MilestoneDict[marker] = new MilestoneDictItem()
                {
                    idList = new List<int>(),
                    milestoneDef = new List<int>(task.Id),
                    isStart = false,
                    isEnd= true
                };
            }
        }

        IEnumerable<DO.Task>? milestoneTasks = tasks.Where(task => task.IsMilestone);

        //if the milestones already stored in data layer
        if (milestoneTasks.Count() != 0)
        {
            foreach(var task in milestoneTasks)
            {
                // since MilestoneDict baseKey is stored in AssignedEngineerId use it to associate the task with dict data 
                if (task.AssignedEngineerId == null) throw new Exception("no milestone dict id");
                MilestoneDict[task.Id] = MilestoneDict[(int)task.AssignedEngineerId]; 
                MilestoneDict.Remove((int)task.AssignedEngineerId);
            }
            return;
        }

        //if not make them
        foreach(var milestone in MilestoneDict) 
        {
            int id = _dal.Task.Create(new DO.Task()
            {
                Id = milestone.Key,
                Alias = "m" + milestone.Key,
                DateCreated = DateTime.Now,
                Description = calcDescription(milestone),
                Duration = calcDuration(milestone),
                Deadline = calcDeadline(milestone),
                ProjectedStartDate = calcProjectedStartDate(milestone),
                DegreeOfDifficulty = null,
                AssignedEngineerId = milestone.Key, //since this field is nto necessary for milestones use it store baseKey in MilestoneDict
                ActualEndDate = calcActualEndDate(milestone),
                IsMilestone = true,
                ActualStartDate = calcActualStartDate(milestone),
                Deliverable = null,
                Notes = null,
                Inactive = false
            });
            MilestoneDict[id] = MilestoneDict[milestone.Key];
            MilestoneDict.Remove(milestone.Key);
        }

        initialized = true;
    }

    private List<int> sortList (List<int> list)
    {
        List<int> sortedList = new List<int>(list);

        // Sort the copied list
        sortedList.Sort();

        return sortedList;
    }

    private bool isInMileStoneDictIdList(int id)
    {
        bool res = false;
        foreach (var kvp in MilestoneDict)
        {
            if (kvp.Value.idList.Contains(id)) res = true;
        }
        return res;
    }

    private bool isInMileStoneDictDef(int id)
    {
        bool res = false;
        foreach (var kvp in MilestoneDict)
        {
            if (kvp.Value.milestoneDef.Contains(id)) res = true;
        }
        return res;
    }

    private List<TaskInList> getTasks(List<int> list)
    {
        List<TaskInList> res = new List<TaskInList>();
        
        foreach (var id in list) {
            DO.Task? task = _dal.Task.Read(id);
            if (task == null) throw new Exception("task does not exist"); 
            res.Add(new TaskInList() {
                Id = task.Id,
                Description = task.Description,
                Alias = task.Alias,
                Status = classTools.StatusCalculator(task)
            });

        }
        
        return res;
    }

    private string? calcDescription(KeyValuePair<int, MilestoneDictItem> milestone)
    {
        string res = "";

        if (milestone.Value.isStart)
        {
            res += "This milestone is a start ";
        }
        
        if ( milestone.Value.isEnd)
        {
            res += "This milestone is an end ";
        }
        return res;
    }
    private TimeSpan? calcDuration(KeyValuePair<int, MilestoneDictItem> milestone) 
    {
        TimeSpan? res = TimeSpan.Zero;
        if (milestone.Value.isStart) return null;
        foreach (var id in milestone.Value.milestoneDef)
        {
            res += _dal.Task.Read(id).Duration ?? TimeSpan.Zero;
        }
        
        return res;
    }
    private DateTime? calcDeadline(KeyValuePair<int, MilestoneDictItem> milestone)
    {
        if (milestone.Value.isStart) return null;
        DateTime? res = DateTime.MaxValue;
        foreach (var id in milestone.Value.milestoneDef)
        {
            if (_dal.Task.Read(id).Deadline != null && _dal.Task.Read(id).Deadline < res)
                res = _dal.Task.Read(id).Deadline;
        }
        if (res ==  DateTime.MaxValue) return null;
        return res;
    }
    private DateTime? calcProjectedStartDate(KeyValuePair<int, MilestoneDictItem> milestone)
    {
        if (milestone.Value.isStart) return null;
        DateTime? res = DateTime.MaxValue;
        foreach (var id in milestone.Value.milestoneDef)
        {
            if (_dal.Task.Read(id).ProjectedStartDate != null && _dal.Task.Read(id).ProjectedStartDate < res)
                res = _dal.Task.Read(id).ProjectedStartDate;
        }
        if (res == DateTime.MaxValue) return null;
        return res;
    }
    private DateTime? calcActualEndDate(KeyValuePair<int, MilestoneDictItem> milestone)
    {
        if (milestone.Value.isStart) return null;
        DateTime? res = DateTime.MinValue;
        foreach (var id in milestone.Value.milestoneDef)
        {
            if (_dal.Task.Read(id).ActualEndDate != null && _dal.Task.Read(id).ActualEndDate > res)
                res = _dal.Task.Read(id).ActualEndDate;
        }
        if (res == DateTime.MinValue) return null;
        return res;
    }
    private DateTime? calcActualStartDate(KeyValuePair<int, MilestoneDictItem> milestone)
    {
        if (milestone.Value.isStart) return null;
        DateTime? res = DateTime.MaxValue;
        foreach (var id in milestone.Value.milestoneDef)
        {
            if (_dal.Task.Read(id).ActualStartDate != null && _dal.Task.Read(id).ActualStartDate < res)
                res = _dal.Task.Read(id).ActualStartDate;
        }
        if (res == DateTime.MaxValue) return null;
        return res;
    }

    private double calcCompletionPercent(List<TaskInList> tasks)
    {
        if (tasks.Count == 0)   return 0;
        double denominator = 4 * tasks.Count();
        double numerator = 0;
        foreach (var task in tasks)
        {
            numerator += (int)task.Status;
        }

        return numerator / denominator;
    }


    public string CreateSchedule(DateTime startDate, DateTime endDate)
    {
        InitMilestoneDict();

        TimeSpan? projectTimeSpan = TimeSpan.Zero;
        foreach(var milestone in MilestoneDict)
        {
            var taskMilestone = _dal.Task.Read(milestone.Key);
            if (taskMilestone.Duration == null) throw new Exception("one of the milestone durations is null");

            if (taskMilestone.Duration > taskMilestone.Deadline - taskMilestone.ProjectedStartDate)
                throw new Exception($"Milestone with id={taskMilestone.Id} has an impossible duration!");
            
            projectTimeSpan += taskMilestone.Duration;
        }
        if(projectTimeSpan > endDate - startDate)
            throw new Exception("Project cannot fit within the given schedule!");
        return "all milestones and the project fit within the schedule!";
    }

    public Milestone Read(int id)
    {
        InitMilestoneDict();

        if (id < 0) throw new Exception("id < 0");

        DO.Task? task = _dal.Task.Read(id);
        if (task == null) throw new Exception("task is null");
        if (!task.IsMilestone) throw new Exception("task is not a milestone");

        var taskList = getTasks(MilestoneDict[task.Id].idList);

        try
        {
            return new Milestone()
            {
                Id = task.Id,
                Description = task.Description,
                Alias = task.Alias,
                DateCreated = task.DateCreated,
                Status = classTools.StatusCalculator(task),
                ProjectedStartDate = task.ProjectedStartDate,
                Deadline = task.Deadline,
                ActualEndDate = task.ActualEndDate,
                CompletionPercentage = calcCompletionPercent(taskList),
                Remarks = task.Notes,
                Dependencies = taskList,
            };
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public Milestone Update(int id, string name = "" , string description = "", string comments = "")
    {
        InitMilestoneDict();

        if (id < 0) throw new Exception("id < 0");
        if (name == "" || name == null) throw new Exception("name provided is null");

        DO.Task? task = _dal.Task.Read(id);
        if (task == null) throw new Exception("task is null");

        if (task?.Alias == "" || task?.Alias == null) throw new Exception("task name is empty");

        var taskList = getTasks(MilestoneDict[task.Id].idList);

        Milestone res = new Milestone()
        {
            Id = task.Id,
            Description = description,
            Alias = name,
            DateCreated = task.DateCreated,
            Status = classTools.StatusCalculator(task),
            ProjectedStartDate = task.ProjectedStartDate,
            Deadline = task.Deadline,
            ActualEndDate = task.ActualEndDate,
            CompletionPercentage = calcCompletionPercent(taskList), 
            Remarks = comments,
            Dependencies = taskList,
        };

        _dal.Task.Update(new DO.Task()
        {
            Id = task.Id,
            Alias = name,
            DateCreated = task.DateCreated,
            Description = description,
            Duration = task.Duration,
            Deadline = task.Deadline,
            ProjectedStartDate = task.ProjectedStartDate,
            DegreeOfDifficulty = task.DegreeOfDifficulty,
            AssignedEngineerId = task.AssignedEngineerId,
            ActualEndDate = task.ActualEndDate,
            IsMilestone = task.IsMilestone,
            ActualStartDate = task.ActualStartDate,
            Deliverable = task.Deliverable,
            Notes = comments,
            Inactive = false,
        });

        return res;
    }

}
