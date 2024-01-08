

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

                Enums.Difficulty _difficulty = Enums.Difficulty.Novice;

                int _assignedEngId = s_random.Next(2000000, 4000000); //Teduat zehut

                //how do I initialize this without an ID???
                Task newTask = new Task(0, var_task, DateTime.Now, _description, _duration,_deadline, _startDate, _difficulty,_assignedEngId);

                s_dalTask!.Create(newTask);

            }

        }

        private static void createEngineers()
        {

        }

        private static void createDependencies()
        {

        }
    }
}
