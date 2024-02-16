
namespace DalTest;

using DO;
using DalApi;


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

        private static void createTasks() {
            string[] TaskNames =
            {
                "Fix thing", "Create Ticket", "Give up", "Change Code", "Update Module", 
                "Resolve Issue", "Report Bug", "Implement Feature", "Optimize Function", 
                "Debug Error", "Refactor Code", "Assign Task", "Close Ticket", 
                "Test Functionality", "Enhance Feature", "Review Code", "Document Change", 
                "Deploy Update", "Discuss Solution", "Evaluate Performance"
            };

            foreach (string var_task in TaskNames)
            {
                //initializing many (but not all, yet) of the fields
                int _creationDateSubtraciton = s_random.Next(-365, -1);
                DateTime _createdDate = DateTime.Now.AddDays(_creationDateSubtraciton);

                string _description = "This is the description for task " + var_task;


                int hours = s_random.Next(0, 24);
                int mins = s_random.Next(0, 60);
                int secs = s_random.Next(0, 60);
                TimeSpan _duration = new TimeSpan(hours, mins, secs);

                DateTime _deadline = DateTime.Now;
                int _deadlineAddition = s_random.Next(366, 1459);
               _deadline.AddDays(_deadlineAddition);               //sets deadline in the future

                DateTime _startDate= DateTime.Now.AddDays(s_random.Next(365, _deadlineAddition));     //sets projected start date less than deadline

                //picks a random difficulty level from the enum
                Enums.EngineerExperience[] _allDifficulties = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));
                int _randIndex = s_random.Next(0, _allDifficulties.Length);

                int _assignedEngId = s_random.Next(2000000, 4000000); //Teduat zehut

                s_dal!.Task.Create(new Task(0, var_task, _createdDate, _description, _duration, _deadline, _startDate, _allDifficulties[_randIndex], _assignedEngId));

            }

        }

        private static void createEngineers()
        {

            string[] IdentityNumbers =
            {
            "267890123", "278901234", "289012345", "290123456", "302345678"
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

                s_dal!.Engineer.Create(new Engineer(Convert.ToInt32(IdentityNumbers[i]), EngineerNames[i], EmailAddresses[i], _cost, _allLevels[_randIndex] ));
            }
        }

        private static void createDependencies()
        {


            for (int i = 0; i < 15; ++i) {

                
                //pick a random task and assign it to these 2 ids:
                int taskID = s_random.Next(8000, 8020);
                int reqID = s_random.Next(8000, 8020);


                while (reqID == taskID || isCircularDep(taskID, reqID, 15)) reqID = s_random.Next(8000, 8020); // to avoid circular dependency

                s_dal!.Dependency.Create(new Dependency(0, taskID, reqID));
            }

        }
        private static bool isCircularDep(int depId, int reqId, int n)
        {
            if (n == 0)
                return false;

            Task? boTask = s_dal!.Task.Read(reqId);

            if (boTask is null)
                throw new DO.DalDoesNotExistException("Task with ID=" + reqId + " does not exist");

            IEnumerable<DO.Dependency?> dependencies = s_dal!.Dependency.ReadAll(dep => dep.DependentTaskId == boTask.Id);

            if (dependencies == null)
                return false;

            foreach (DO.Dependency? dep in dependencies)
            {
                if (dep != null && dep.Id == depId)
                    return true;
            }

            foreach (DO.Dependency? dep in dependencies)
            {
                if (isCircularDep(depId, (int)dep!.DependentTaskId!, n - 1))
                    return true;
            }

            return false;
        }

        private static void createStartAndEndDateForProject()
        {
            int startDateAdd = s_random.Next(1, 365);
            int endDateAdd = s_random.Next(1460, 1785); //4 to 5 years after today

            DateTime startDate = DateTime.Now.AddDays(startDateAdd);  
            DateTime endDate = DateTime.Now.AddDays(endDateAdd);

            s_dal!.Config.SetProjectStartDate(startDate);
            s_dal!.Config.SetProjectEndDate(endDate);

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

        public static void Do()
        {
            s_dal = DalApi.Factory.Get;
            //s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL cannot be null!");

            reset();

            createStartAndEndDateForProject();
            createTasks();
            createEngineers();
            createDependencies();
        }
    }
}
