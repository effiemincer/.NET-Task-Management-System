
namespace Dal;

static internal class DataSource
{

    internal static class Config
    {
        //Project start and end dates
        internal static DateTime? kickstartDate { get; set; } = null;
        internal static DateTime? endDate { get; set; } = null;

        
        // Dependency
        internal const int startDependencyId = 9000;
        private static int nextDependencyId = startDependencyId;
        internal static int NextDependencyId { get => nextDependencyId++; }

        // Task
        internal const int startTaskId = 8000;
        private static int nextTaskId = startTaskId;
        internal static int NextITaskId { get => nextTaskId++; }
    }

    //readonly static Random R = new Random();
    internal static List<DO.Dependency> Dependencies { get; } = new();
    internal static List<DO.Task> Tasks { get; } = new();
    internal static List<DO.Engineer> Engineers { get; } = new();
}
