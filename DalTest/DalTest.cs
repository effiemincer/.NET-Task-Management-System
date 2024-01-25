
namespace DalTest;

using DO;
using DalApi;


static internal class DalTest
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

                s_dal!.Engineer.Create(new Engineer(Convert.ToInt32(IdentityNumbers[i]), EngineerNames[i], EmailAddresses[i], _allLevels[_randIndex], _cost));
            }
        }

        private static void createDependencies()
        {
            /*
            string[] EmailAddresses =
            {
                "john.smith@example.com",
                "alice.johnson@example.com",
                "david.miller@example.com",
                "emily.white@example.com",
                "michael.davis@example.com",
                "sophia.turner@example.com",
                "brian.robinson@example.com",
                "olivia.hall@example.com",
                "daniel.carter@example.com",
                "grace.taylor@example.com",
                "james.brown@example.com",
                "linda.green@example.com",
                "william.clark@example.com",
                "susan.wilson@example.com",
                "robert.moore@example.com",
                "laura.harris@example.com",
                "mark.young@example.com",
                "patricia.king@example.com",
                "steven.lee@example.com",
                "jennifer.allen@example.com",
                "janedoe@gmail.com",
                "samwilson@yahoo.com",
                "emily.jones@hotmail.com",
                "alexsmith@outlook.com",
                "nataliebrown@gmail.com",
                "andrewjohnson@yahoo.com",
                "olivia.adams@hotmail.com",
                "davidclark@outlook.com",
                "sophiewilson@gmail.com",
                "michaelroberts@yahoo.com",
                "laurasmith@hotmail.com",
                "chriscarter@outlook.com",
                "elizabethking@gmail.com",
                "nathanielscott@yahoo.com",
                "hannahmiller@hotmail.com",
                "ethanwalker@outlook.com",
                "annabellewhite@gmail.com",
                "williamjones@yahoo.com",
                "victoriabaker@hotmail.com",
                "jacobevans@outlook.com"
            };

            string[] ShippingAddresses =
            {
                "123 Main St, Apt 4, Cityville, State, 12345",
                "456 Oak Ln, Unit 8, Townsville, State, 54321",
                "789 Pine Rd, Suite 12, Villagetown, State, 67890",
                "101 Elm St, Lot 3, Boroughville, State, 98765",
                "202 Maple Ave, Building 6, Hamletsville, State, 24680",
                "303 Cedar Blvd, Floor 2, Countryside, State, 13579",
                "404 Birch Ct, Room 15, Ruralville, State, 86420",
                "505 Redwood Dr, Box 7, Outskirts, State, 97531",
                "606 Spruce Lane, Trailer 9, Farmland, State, 46852",
                "707 Fir Street, Loft 21, Wilderness, State, 35714",
                "808 Walnut Loop, Suite 33, Valleytown, State, 95173",
                "909 Pinecone Way, Apartment 10, Hillsburg, State, 26473",
                "010 Cherry Lane, Lot 5, Riverside, State, 84729",
                "111 Birchwood Rd, Box 12, Mountaintop, State, 57284",
                "212 Cedar Court, Building 7, Lakeside, State, 32984",
                "313 Elm Street, Floor 3, Harborville, State, 46729",
                "414 Maple Ave, Room 18, Hillside, State, 82934",
                "515 Oakwood Dr, Trailer 11, Meadows, State, 36572",
                "616 Pine St, Loft 25, Summitville, State, 15837",
                "717 Spruce Lane, Unit 14, Brookside, State, 73526",
                "123 Spring Street, Apt 5, Cityville, State, 12345",
                "456 Maple Lane, Unit 9, Townsville, State, 54321",
                "789 Oakwood Rd, Suite 13, Villagetown, State, 67890",
                "101 Pine St, Lot 4, Boroughville, State, 98765",
                "202 Cedar Ave, Building 7, Hamletsville, State, 24680",
                "303 Elmwood Blvd, Floor 3, Countryside, State, 13579",
                "404 Birchwood Ct, Room 16, Ruralville, State, 86420",
                "505 Redwood Lane, Box 8, Outskirts, State, 97531",
                "606 Oak St, Trailer 10, Farmland, State, 46852",
                "707 Spruce Lane, Loft 22, Wilderness, State, 35714",
                "808 Cherry Lane, Suite 34, Valleytown, State, 95173",
                "909 Maple Ave, Apartment 11, Hillsburg, State, 26473",
                "010 Pinecone Way, Lot 6, Riverside, State, 84729",
                "111 Walnut Rd, Box 13, Mountaintop, State, 57284",
                "212 Cedar Loop, Building 8, Lakeside, State, 32984",
                "313 Pine St, Floor 4, Harborville, State, 46729",
                "414 Oakwood Dr, Room 19, Hillside, State, 82934",
                "515 Birch Ct, Trailer 12, Meadows, State, 36572",
                "616 Redwood Blvd, Loft 26, Summitville, State, 15837",
                "717 Maple Lane, Unit 15, Brookside, State, 73526"
            };
            */


            for (int i = 0; i < 40; ++i) {
                /*
                DateTime _creation = DateTime.Now;
                int _creationAddition = s_random.Next(365, 1400);
                _creation.AddDays(_creationAddition);

                
                int _shippingAddition = s_random.Next(1 , 365);
                DateTime _shipping = _creation.AddDays(_shippingAddition);

                int _deliveryAddition = s_random.Next(1, 7);
                DateTime _delivery = _shipping.AddDays(_deliveryAddition); */

                //pick a random task and assign it to these 2 ids:
                int taskID = s_random.Next(8000, 8020);
                int reqID = s_random.Next(8000, 8020);

                while (reqID == taskID) reqID = s_random.Next(8000, 8020); // to avoid circular dependency

                s_dal!.Dependency.Create(new Dependency(0, taskID, reqID));
            }

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

        public static void Do(IDal? Dal)
        {
            s_dal = Dal ?? throw new NullReferenceException("DAL obj can't be null");
            //s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL cannot be null!");

            reset();

            createStartAndEndDateForProject();
            createTasks();
            createEngineers();
            createDependencies();
        }
    }
}
