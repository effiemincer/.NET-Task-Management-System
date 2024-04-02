namespace DalTest;

using DO;
using DalApi;
using Dal;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

static public class DalTest
{

    /// <summary>
    /// Initializaton of all the lists with test information
    /// </summary>
    public static class Initialization
    {
        //private static IConfig? s_dalConfig;    //stage 1
        //private static ITask? s_dalTask; //stage 1
        //private static IEngineer? s_dalEngineer; //stage 1
        //private static IDependency? s_dalDependency; //stage 1
        private static IDal? s_dal;

        private static readonly Random s_random = new Random();

        private static void createTasksAndDependencies(int size) {

            XElement root = Dal.XMLTools.LoadListFromXMLElement("data-config");
            int nextId = root.ToIntNullable("NextTaskId") ?? throw new FormatException($"can't convert id.  data-config, NextTaskId");
            root.Element("NextTaskId")?.SetValue((8000).ToString());
            XMLTools.SaveListToXMLElement(root, "data-config");

            string[] TaskNames =
            {
                "Fix thing", "Create Ticket", "Give up", "Change Code", "Update Module", 
                "Resolve Issue", "Report Bug", "Implement Feature", "Optimize Function", 
                "Debug Error", "Refactor Code", "Assign Task", "Close Ticket", 
                "Test Functionality", "Enhance Feature", "Review Code", "Document Change", 
                "Deploy Update", "Discuss Solution", "Evaluate Performance"
            };

            //number of total dependencies
            int totalNumDependencies = 40, dependecyCount = 0;
            DateTime? previousDate = s_dal?.Config.GetProjectStartDate();

            foreach (string var_task in TaskNames)
            {
                //initializing many (but not all, yet) of the fields
                int _creationDateSubtraciton = s_random.Next(-365, -1);
                DateTime _createdDate = DateTime.Now.AddDays(_creationDateSubtraciton);

                string _description = "This is the description for task " + var_task;

                int hours, mins, secs;
                TimeSpan _duration;
                DateTime? _startDate, _deadline;

                //small project
                if (size == 0)
                {
                    hours = s_random.Next(1, 72);
                    mins = s_random.Next(0, 60);
                    secs = s_random.Next(0, 60);
                    _duration = new TimeSpan(hours, mins, secs);

                    _startDate = previousDate + TimeSpan.FromDays(s_random.Next(1, 2));     //sets projected start date less than deadline
                    previousDate = _startDate;
                    _deadline = _startDate + _duration;
                }
                else if (size == 1)
                {
                    hours = s_random.Next(72, 168);
                    mins = s_random.Next(0, 60);
                    secs = s_random.Next(0, 60);
                    _duration = new TimeSpan(hours, mins, secs);

                    _startDate = previousDate + TimeSpan.FromDays(s_random.Next(3, 7));     //sets projected start date less than deadline
                    previousDate = _startDate;
                    _deadline = _startDate + _duration;
                }
                else
                {
                    hours = s_random.Next(600, 744);
                    mins = s_random.Next(0, 60);
                    secs = s_random.Next(0, 60);
                    _duration = new TimeSpan(hours, mins, secs);

                    _startDate = previousDate + TimeSpan.FromDays(s_random.Next(14, 21));     //sets projected start date less than deadline
                    previousDate = _startDate;
                    _deadline = _startDate + _duration;
                }
          

              // _deadline.AddDays(_deadlineAddition);               //sets deadline in the future

                
                //DateTime? _startDate = null;

                //picks a random difficulty level from the enum
                Enums.EngineerExperience[] _allDifficulties = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));
                int _randIndex = s_random.Next(0, _allDifficulties.Length);

                //int _assignedEngId = s_random.Next(2000000, 4000000); //Teduat zehut
                int? _assignedEngId = null;

                int newestTaskId = s_dal!.Task.Create(new Task(0, var_task, _createdDate, _description, _duration, _deadline, _startDate, _allDifficulties[_randIndex], _assignedEngId));

                //loop to create dependecnsies for the task
                //creating dependencies for the task now prevents circular dependencies
                if (dependecyCount <= totalNumDependencies)
                {
                    IEnumerable<DO.Task?> taskList = s_dal!.Task.ReadAll(task => task.Id != newestTaskId);

                    //if the task list is empty then skip the rest
                    if (taskList == null)
                        continue;
                    //create a random number of dependencies for this task
                    //for (int i = 0; i < s_random.Next(0, int.Min(5, taskList.Count())) && dependecyCount <= totalNumDependencies; ++i)
                    for (int i = 0; i < s_random.Next(0, taskList.Count()) && dependecyCount <= totalNumDependencies; ++i)
                    {
                        //find a random task to make this task dependent on
                        int index = s_random.Next(0, taskList.Count() - 1);
                        Task? task = taskList.ElementAt(index);

                        IEnumerable<DO.Dependency?> dependencies = s_dal!.Dependency.ReadAll(dep => dep.DependentTaskId == newestTaskId);
                        bool depAlreadyExists = false;
                        foreach(DO.Dependency? dep in dependencies)
                        {
                            if (dep != null && dep.RequisiteID == task!.Id)
                            {
                                depAlreadyExists = true; //this task is already dependent on the task we picked
                                break;
                            }
                        }
                        //if it doesn't already exist then make a new one
                        if (!depAlreadyExists)
                        {
                            s_dal!.Dependency.Create(new Dependency(0, newestTaskId, task!.Id));
                            dependecyCount++;
                        }
                        else
                        {
                            i--; 
                            continue;
                        }
                    }
                }

            }

        }

        //private static void createDependencies()
        //{
        //    int numDependencies = 40;
        //    for (int i = 0; i <= numDependencies; ++i)
        //    {


        //        //pick a random task and assign it to these 2 ids:
        //        int taskID = s_random.Next(8000, 8020);
        //        int reqID = s_random.Next(8000, 8020);

        //        Task? boTask = s_dal!.Task.Read(reqID);

        //        if (boTask is null)
        //            throw new DO.DalDoesNotExistException("Task with ID=" + reqID + " does not exist");

        //        IEnumerable<DO.Dependency?> dependencies = s_dal!.Dependency.ReadAll(dep => dep.DependentTaskId == boTask.Id);

        //        while (reqID == taskID || isCircularDep(taskID, reqID, i, dependencies))
        //        {
        //            taskID = s_random.Next(8000, 8020);
        //        }

        //        s_dal!.Dependency.Create(new Dependency(0, taskID, reqID));
        //    }

        //}
        private static bool isCircularDep(int depId, int reqId, int n, IEnumerable<DO.Dependency?> dependencies)
        {
            if (n == 0)
                return false;

            if (dependencies == null)
                return false;

            foreach (DO.Dependency? dep in dependencies)
            {
                if (dep != null && dep.DependentTaskId == depId)
                    return true;
            }

            //foreach (DO.Dependency? dep in dependencies)
            // {
            if (isCircularDep(depId, reqId, n - 1, dependencies))
                return true;
            //}

            return false;
        }

        private static void createEngineers()
        {

            string[] IdentityNumbers =
            {
            "346291982", "278901234", "289012345", "290123456", "302345678"
            };

            string[] EngineerNames =
            {
                "John Smith", "Alice Johnson", "David Miller", "Emily White", "Michael Davis",
            };
            

            string[] EmailAddresses =
            {
                "john.smith@example.com",
                "alice.johnson@example.com",
                "david.miller@example.com",
                "emily.white@example.com",
                "michael.davis@example.com",

            };
            for (int i = 0; i < EngineerNames.Length; ++i) {

                double _cost  = s_random.Next(30, 60);

                //picks a random difficulty level from the enum
                Enums.EngineerExperience[] _allLevels = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));
                int _randIndex = s_random.Next(0, _allLevels.Length);


                s_dal!.Engineer.Create(new Engineer(Convert.ToInt32(IdentityNumbers[i]), EngineerNames[i], EmailAddresses[i], _cost, _allLevels[_randIndex]));

            }
        }


        private static void createStartAndEndDateForProject(int size)
        {
            int startDateAdd=0;
            int endDateAdd=0;

            //small project
            if (size == 0)
            {
                startDateAdd = s_random.Next(0, 7);
                endDateAdd = s_random.Next(29, 35);
            }
            //medium project
            else if (size == 1)
            {
                startDateAdd = s_random.Next(0, 7);
                endDateAdd = s_random.Next(120, 150);
            }
            //large project
            else
            {
                startDateAdd = s_random.Next(0, 7);
                endDateAdd = s_random.Next(360, 364);
            }



            DateTime startDate = DateTime.Now.AddDays(startDateAdd);  
            DateTime endDate = DateTime.Now.AddDays(endDateAdd+startDateAdd);

            s_dal!.Config.SetProjectStartDate(startDate);
            s_dal!.Config.SetProjectEndDate(endDate);
            s_dal!.Config.SetIsScheduleGenerated();

        }

        //Definition and implementation of the Do driver method
        //public static void Do (ITask? dalTask, IEngineer? dalEngineer, IDependency? dalDependency, IProject? dalConfig)
        //{
        //    s_dalTask = dalTask ?? throw new NullReferenceException("DAL cannot be null!");
        //    s_dalEngineer = dalEngineer ?? throw new NullReferenceException("DAL cannot be null!");
        //    s_dalDependency = dalDependency ?? throw new NullReferenceException("DAL cannot be null!");
        //    s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL cannot be null!");

        //    createStartAndEndDateForProject();
        //    createTasks();
        //    createEngineers();
        //    createDependencies();
        //}

        public static void reset()
        {
            s_dal!.Config.Reset();
        }

        public static void Do(int size)
        {
            s_dal = DalApi.Factory.Get;
            //s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL cannot be null!");

            reset();

            createStartAndEndDateForProject(size);
            createTasksAndDependencies(size);
            createEngineers();
            //createDependencies();
        }
    }
}
