
namespace BlImplementation;
using BlApi;
using BO;
using System.Collections.Generic;
using System.Diagnostics;

/// <summary>
/// MilestoneImplementation class implements IMilestone interface and is responsible for the logic of the Milestone entity
/// </summary>
internal class MilestoneImplementation : IMilestone
{
    /// <summary>
    /// Dal instance to be used by the class
    /// </summary>
    private static Dictionary<int, MilestoneDictItem> MilestoneDict = new Dictionary<int, MilestoneDictItem>();
    /// <summary>
    /// initialized is a flag to check if the MilestoneDict has been initialized
    /// </summary>
    private static bool initialized = false;
    
    /// <summary>
    /// Dal instance to be used by the class
    /// </summary>
    private DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// MilestoneDictItem is a class to store the data of the MilestoneDict
    /// </summary>
    private class MilestoneDictItem
    { 
        /// <summary>
        /// IdList is a list of task ids that this milestone points to
        /// </summary>
        public List<int> idList {  get; set; } 

        /// <summary>
        /// milestoneDef is a list of task ids that point to this milestone
        /// </summary>
        public List<int> milestoneDef {  get; set; }

        /// <summary>
        /// is the milestone a start milestone
        /// </summary>
        public bool isStart { get; set; }

        /// <summary>
        /// is the milestone an end milestone
        /// </summary>
        public bool isEnd { get; set; }
    }   

    /// <summary>
    /// Create Dependency Groups
    /// </summary>
    /// <returns></returns>
    /// <exception cref="BO.BlBadInputDataException"></exception>
    /// <exception cref="BO.BlArgumentNullException"></exception>
    private Dictionary<int, List<int>> CreateDependencyGroups()
    {
        IEnumerable<DO.Dependency>? dependencies = _dal.Dependency.ReadAll();
        Dictionary<int, List<int>> depGroups = new Dictionary<int, List<int>>();

        if (dependencies.Count() <= 0 )
        {
            throw new BO.BlBadInputDataException("no dependencies!");
        }

        foreach (var dep in dependencies)
        {
            if (dep.DependentTaskId == null) throw new BO.BlArgumentNullException("no dependent task id");
            if (dep.RequisiteID == null) throw new BO.BlArgumentNullException("no requisite task id");
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

    /// <summary>
    /// Initializes the Milestone Dictionary.
    /// </summary>
    private void InitMilestoneDict()
    {
        // Check if the MilestoneDict has already been initialized
        if (initialized) return;

        // Retrieve dependency groups
        Dictionary<int, List<int>> dependencyGroups = CreateDependencyGroups();
        if (dependencyGroups.Count == 0) throw new BO.BlBadInputDataException("No dependencies");

        // Retrieve all tasks
        IEnumerable<DO.Task>? tasks = _dal.Task.ReadAll();
        if (tasks.Count() == 0) throw new BO.BlBadInputDataException("No tasks");

        int marker = 1;

        // Create Milestone Dictionary items based on dependency groups
        foreach (var depTask in dependencyGroups)
        {
            if (isInMileStoneDictIdList(depTask.Key)) continue;

            MilestoneDict[marker] = new MilestoneDictItem()
            {
                idList = new List<int> { depTask.Key },
                milestoneDef = new List<int>(depTask.Value),
                isStart = false,
                isEnd = false
            };

            foreach (var depTask2 in dependencyGroups)
            {
                if (depTask.Key == depTask2.Key) continue;
                if (depTask.Value.SequenceEqual(depTask2.Value))
                {
                    MilestoneDict[marker].idList.Add(depTask2.Key);
                }
            }

            ++marker;
        }

        // Create start milestone dictionary item
        MilestoneDict[marker] = new MilestoneDictItem()
        {
            idList = new List<int>(),
            milestoneDef = new List<int>(),
            isStart = true,
            isEnd = false
        };

        // Create end milestone dictionary item
        MilestoneDict[marker + 1] = new MilestoneDictItem()
        {
            idList = new List<int>(),
            milestoneDef = new List<int>(),
            isStart = false,
            isEnd = true
        };

        // Add start and end milestones
        foreach (var task in tasks)
        {
            // Add tasks to idList of start milestone
            if (!isInMileStoneDictIdList(task.Id))
            {
                MilestoneDict[marker].idList.Add(task.Id);
            }
            // Add tasks to milestoneDef of end milestone
            if (!isInMileStoneDictDef(task.Id))
            {
                MilestoneDict[marker + 1].milestoneDef.Add(task.Id);
            }
        }

        // Check if milestones are already stored in the data layer
        IEnumerable<DO.Task>? milestoneTasks = tasks.Where(task => task.IsMilestone);

        if (milestoneTasks.Count() != 0)
        {
            // Associate milestone tasks with the dictionary data
            foreach (var task in milestoneTasks)
            {
                if (task.AssignedEngineerId == null) throw new BO.BlBadInputDataException("No milestone dictionary ID");
                MilestoneDict[task.Id] = MilestoneDict[(int)task.AssignedEngineerId];
                MilestoneDict.Remove((int)task.AssignedEngineerId);
            }
            initialized = true;
            return;
        }

        Dictionary<int, MilestoneDictItem> temp = new Dictionary<int, MilestoneDictItem>();

        // If not, create them
        foreach (var milestone in MilestoneDict)
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
                AssignedEngineerId = milestone.Key, // Store baseKey in MilestoneDict since this field is not necessary for milestones
                ActualEndDate = calcActualEndDate(milestone),
                IsMilestone = true,
                ActualStartDate = calcActualStartDate(milestone),
                Deliverable = null,
                Notes = null,
                Inactive = false
            });
            temp[id] = MilestoneDict[milestone.Key];
        }

        MilestoneDict = temp;

        initialized = true;
    }


    /// <summary>
    /// sortList sorts a list of integers
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    private List<int> sortList (List<int> list)
    {
        List<int> sortedList = new List<int>(list);

        // Sort the copied list
        sortedList.Sort();

        return sortedList;
    }

    /// <summary>
    /// check if a task is in the MilestoneDict idList
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private bool isInMileStoneDictIdList(int id)
    {
        bool res = false;
        foreach (var kvp in MilestoneDict)
        {
            if (kvp.Value.idList.Contains(id)) res = true;
        }
        return res;
    }


    /// <summary>
    /// check if a task is in the MilestoneDict milestoneDef
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    private bool isInMileStoneDictDef(int id)
    {
        bool res = false;
        foreach (var kvp in MilestoneDict)
        {
            if (kvp.Value.milestoneDef.Contains(id)) res = true;
        }
        return res;
    }

    /// <summary>
    /// getTasks returns a list of tasks from a list of task ids
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    /// <exception cref="BO.BlDoesNotExistException"></exception>
    private List<TaskInList> getTasks(List<int> list)
    {
        List<TaskInList> res = new List<TaskInList>();
        
        foreach (var id in list) {
            DO.Task? task = _dal.Task.Read(id);
            if (task == null) throw new BO.BlDoesNotExistException("task does not exist"); 
            res.Add(new TaskInList() {
                Id = task.Id,
                Description = task.Description,
                Alias = task.Alias,
                Status = classTools.StatusCalculator(task)
            });

        }
        
        return res;
    }

    /// <summary>
    /// calcDescription returns a string of the description of a milestone
    /// </summary>
    /// <param name="milestone"></param>
    /// <returns></returns>
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

    /// <summary>
    /// calcDuration returns the duration of a milestone
    /// </summary>
    /// <param name="milestone"></param>
    /// <returns></returns>
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

    /// <summary>
    /// calcDeadline returns the deadline of a milestone
    /// </summary>
    /// <param name="milestone"></param>
    /// <returns></returns>
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

    /// <summary>
    /// calcProjectedStartDate returns the projected start date of a milestone
    /// </summary>
    /// <param name="milestone"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Calculates the actual end date of a milestone
    /// </summary>
    /// <param name="milestone"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Calculates the actual start date of a milestone
    /// </summary>
    /// <param name="milestone"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Calculates the completion percentage of a milestone
    /// </summary>
    /// <param name="tasks"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Create a schedule for the project using the milestones
    /// </summary>
    /// <param name="startDate">the start date of the project</param>
    /// <param name="endDate">the end date of the project</param>
    /// <returns>a string that says whether task creation was successful</returns>
    /// <exception cref="BO.BlNullPropertyException">one of the milestones duration is null</exception>
    /// <exception cref="BO.BlBadInputDataException">Milestones don't fit within schedule</exception>
    public string CreateSchedule(DateTime startDate, DateTime endDate)
    {
        InitMilestoneDict();

        TimeSpan? projectTimeSpan = TimeSpan.Zero;
        foreach(var milestone in MilestoneDict)
        {
            var taskMilestone = _dal.Task.Read(milestone.Key);
            if (taskMilestone.Duration == null && !MilestoneDict[taskMilestone.Id].isStart) throw new BO.BlNullPropertyException("one of the milestone durations is null");

            if (taskMilestone.Duration > taskMilestone.Deadline - taskMilestone.ProjectedStartDate && !MilestoneDict[taskMilestone.Id].isStart)
                throw new BO.BlBadInputDataException($"Milestone with id={taskMilestone.Id} has an impossible duration!");
            
            projectTimeSpan += taskMilestone.Duration;
        }
        if(projectTimeSpan > endDate - startDate)
            throw new BO.BlBadInputDataException("Project cannot fit within the given schedule!");
        return "all milestones and the project fit within the schedule!";
    }

    /// <summary>
    /// Read a milestone by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns>a Milestone</returns>
    /// <exception cref="BO.BlDoesNotExistException">task does not exist</exception>
    /// <exception cref="BO.BlBadInputDataException">inoput doesn't return a task</exception>
    /// <exception cref="BO.BlAlreadyExistsException">the Milestone already exists</exception>
    public Milestone Read(int id)
    {
        InitMilestoneDict();

        if (id < 0) throw new BO.BlBadInputDataException("id < 0");

        DO.Task? task = _dal.Task.Read(id);
        if (task == null) throw new BO.BlDoesNotExistException("task is null");
        if (!task.IsMilestone) throw new BO.BlBadInputDataException("task is not a milestone");

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
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException("A task/milestone with that ID already exists", ex);
        }
    }

    /// <summary>
    /// Update a milestone by ID
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="comments"></param>
    /// <returns>the updated Milestone</returns>
    /// <exception cref="BO.BlBadInputDataException"> Data doesn't work for Milestone</exception>
    /// <exception cref="BO.BlDoesNotExistException"> Milestone ID doesn't exist</exception>
    public Milestone Update(int id, string name = "" , string description = "", string comments = "")
    {
        InitMilestoneDict();

        if (id < 0) throw new BO.BlBadInputDataException("id < 0");
        if (name == "" || name == null) throw new BO.BlBadInputDataException("name provided is null");

        DO.Task? task = _dal.Task.Read(id);
        if (task == null) throw new BO.BlDoesNotExistException("task is null");

        if (task?.Alias == "" || task?.Alias == null) throw new BO.BlBadInputDataException("task name is empty");

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

    public IEnumerable<MilestoneInList> ReadAll(Func<BO.MilestoneInList, bool>? filter = null)
    {
        IEnumerable<MilestoneInList> milestones = from DO.Task m_task in _dal.Task.ReadAll(task => task.IsMilestone)
                    select new BO.MilestoneInList
                    {
                        Id = m_task.Id,
                        Description = m_task.Description,
                        Alias = m_task.Alias,
                        Status = classTools.StatusCalculator(m_task),
                        CompletionPercentage = calcCompletionPercent(getTasks(MilestoneDict[m_task.Id].idList))
                    };
        if (filter != null)
        {
            milestones = milestones.Where(filter);

        }

        return milestones;
    }
}
