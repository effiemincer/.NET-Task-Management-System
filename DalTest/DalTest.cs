

namespace DalTest;

using DO;
using DalApi;

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

        }

        private static void createEngineers()
        {

        }

        private static void createDependencies()
        {

        }
    }
}
