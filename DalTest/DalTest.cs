

namespace DalTest;

using DO;
using DalApi;
using System.Data.Common;
using System.Runtime.CompilerServices;

static internal class DalTest
{

    public static class Initialization
    {
        private static ITask? s_dalTask; //stage 1
        private static IEngineer? s_dalEngineer; //stage 1
        private static IDependency? s_dalDependency; //stage 1

        private static readonly Random s_random = new Random();

        private static void createTasks() {
            string[] TaskNames =
            {
                "one", "two", "three", "four", "five", "six"
            };

            foreach (string var_task in TaskNames)
            {
                //initializing many (but not all, yet) of the fields
                string _description = "This is the description for task " + var_task;

                int _duration = s_random.Next(5, 40);

                DateTime _deadline = DateTime.Now;
                int _deadlineAddition = s_random.Next(2, 365);
                _deadline.AddDays(_deadlineAddition);               //sets deadline in the future

                DateTime _startDate= DateTime.Now.AddDays(s_random.Next(2, _deadlineAddition));     //sets projected start date less than deadline

                //picks a random difficulty level from the enum
                Enums.EngineerExperience[] _allDifficulties = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));
                int _randIndex = s_random.Next(0, _allDifficulties.Length);

                int _assignedEngId = s_random.Next(2000000, 4000000); //Teduat zehut

                s_dalTask!.Create(new Task(0, var_task, DateTime.Now, _description, _duration, _deadline, _startDate, _allDifficulties[_randIndex], _assignedEngId));

            }

        }

        private static void createEngineers()
        {
            string[] EngineerNames =
            {
                "John Smith", "Alice Johnson", "David Miller", "Emily White", "Michael Davis",
                "Sophia Turner", "Brian Robinson", "Olivia Hall", "Daniel Carter", "Grace Taylor"
            };

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
                "grace.taylor@example.com"
            };

            for (int i = 0; i < EngineerNames.Length; ++i) {

                double _cost  = s_random.Next(30, 60);

                //picks a random difficulty level from the enum
                Enums.EngineerExperience[] _allLevels = (Enums.EngineerExperience[])Enum.GetValues(typeof(Enums.EngineerExperience));
                int _randIndex = s_random.Next(0, _allLevels.Length);

                s_dalEngineer!.Create(new Engineer(0, EngineerNames[i], EmailAddresses[i], _allLevels[_randIndex], _cost));
            }
        }

        private static void createDependencies()
        {
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
                "grace.taylor@example.com"
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
                "707 Fir Street, Loft 21, Wilderness, State, 35714"
            };


            for (int i = 0; i < EmailAddresses.Length; ++i) {

                DateTime _creation = DateTime.Now;
                
                DateTime _shipping = DateTime.Now;
                int _shippingAddition = s_random.Next(2, 365);
                _shipping.AddDays(_shippingAddition);

                int _deliveryAddition = s_random.Next(1, 7);
                DateTime _delivery = _shipping.AddDays(_deliveryAddition);

                int taskID = s_random.Next(9000, 9999);
                int reqID = s_random.Next(9000, 9999);

                s_dalDependency!.Create(new Dependency(0, taskID, reqID, EmailAddresses[i], ShippingAddresses[i], _creation, _shipping, _delivery));
            }
        }

        //Definition and implementation of the Do driver method
        public static void Do (ITask? dalTask, IEngineer? dalEngineer, IDependency? dalDependency)
        {
            s_dalTask = dalTask ?? throw new NullReferenceException("DAL cannot be null!");
            s_dalEngineer = dalEngineer ?? throw new NullReferenceException("DAL cannot be null!");
            s_dalDependency = dalDependency ?? throw new NullReferenceException("DAL cannot be null!");

            createTasks();
            createEngineers();
            createDependencies();
        }
    }
}
