
namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using System.Collections.Generic;
using System.Text;

internal class MilestoneImplementation : IMilestone
{
    private static Dictionary<int, MilestoneDictItem> MilestoneDict = new Dictionary<int, MilestoneDictItem>();
    public static List<Milestone> MilestoneList = new List<Milestone>();
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

        //not sure which id is for what ask effie
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
        //two things #1 when a new task or dependancy are created milestone has to be cleared and initialized = false
        // #2 when creating the milestone dict do we need the ids of the milestones in memory or do we not care?
        // additional question when a milestone task is put into the memory do we replace its dependants' dependency list with this id?
        // or another way to do it is to always build the milestone regularly  but then at the end check milestone objects and replace the keys in our milestone dict with the appropriate id
        // it seems like they want the first option based on what i wrote down on the bottom however i may end up just ignoring that anyways for flexability reasons it might be better to do
        // it my way this way if they create an entirely new task even after initializing milestone everything will be fine 
        // another wrinkle i just thought of is what if the milestone structure changes ----> i have thought about it and i think its best everytime you initialize Milestone you delete all
        // milestone objects in the dal and then recreate them i know this is a bitof a waste of time algorithmically however trying to handle every different change will be difficult 
        // this way we can avoid problems of needing to update milestone tasks and 

        if (initialized) return;

        Dictionary<int, List<int>> dependencyGroups = CreateDependencyGroups();
        if (dependencyGroups.Count == 0) throw new Exception("no dependencies");


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
                    isStart= false,
                    isEnd= false,
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
                    isEnd= false,
                };
            }
            if (!isInMileStoneDictDef(task.Id))
            {
                MilestoneDict[marker] = new MilestoneDictItem()
                {
                    idList = new List<int>(task.Id),
                    milestoneDef = new List<int>(),
                    isStart = false,
                    isEnd= true,
                };
            }
        }

        //creating actual milestone intances from 'backbone dictionary'
        foreach(var milestoneDef in MilestoneDict)
        {
            //where to get the rest of the milestone info from?
            //add logic for start and end milestones --> depends on above questions answer
            
            // is the milestone obj necessary here??
            // maybe instead we put a task (isMilestone = true) into DAL
            // only use milestone obj when passing back through read func
            // remember that when creating the task in DAL id will be changed need to store that change in DICTIONARY
            try
            {
                
                MilestoneList.Add(
                    new Milestone()
                    {
                        Id = milestoneDef.Key,
                        Description = "",
                        Alias = "m" + milestoneDef.Key,
                        DateCreated = new DateTime(),
                        Status = new BO.Enums.Status(),
                        ProjectedStartDate = new DateTime(),
                        Deadline = new DateTime(),
                        ActualEndDate = new DateTime(),
                        CompletionPercentage = 0.0,
                        Remarks = "",
                        Dependencies = getTasks(milestoneDef.Value.idList),
                    }
                );
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        //get rid of task dependencies instead have tasks with milestones ??
        //how to get BO.Tasks here ??
        //how does the user access BO logic ??

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


    public List<BO.Task> CreateSchedule(DateTime startDate, DateTime endDate, List<TaskInList> tasks, List<MilestoneInList> milestones, List<DO.Dependency> dependencies)
    {
        InitMilestoneDict();

        return new List<Task>();
    }

    public Milestone Read(int id)
    {
        InitMilestoneDict();

        if (id < 0) throw new Exception("id < 0");

        DO.Task? task = _dal.Task.Read(id);
        if (task == null) throw new Exception("task is null");
        if (!task.IsMilestone) throw new Exception("task is not a milestone");
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
                CompletionPercentage = 0.0, //what to do here?
                Remarks = task.Notes,
                Dependencies = getTasks(MilestoneDict[task.Id].idList),
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

        //very confusing language not sure what this is asking for 
        //check whether the order exists in the DL, AND that the name is nonempty.

        if (id < 0) throw new Exception("id < 0");
        if (name == "" || name == null) throw new Exception("name provided is null");

        DO.Task? task = _dal.Task.Read(id);
        if (task == null) throw new Exception("task is null");

        if (task?.Alias == "" || task?.Alias == null) throw new Exception("task name is empty");
        
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
            CompletionPercentage = 0.0, //what to do here?
            Remarks = comments,
            Dependencies = getTasks(MilestoneDict[task.Id].idList),
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

    public List<Milestone> ReadAll()
    {
        InitMilestoneDict();
        return MilestoneList;
    }
}
