
namespace Dal;

static internal class DataSource
{
    internal static class Config
    {
        // Dependency
        internal const int startDependencyId = 9000;
        private static int nextDependencyId = startDependencyId;
        internal static int NextDependencyId { get => nextDependencyId++; }

        // Task
        internal const int startTaskId = 8000;
        private static int nextTaskId = startTaskId;
        internal static int NextITaskId { get => nextTaskId++; }

        // Engineer
        // uncommented until we have a teudat zehut id
        //internal const int startEngineerId = 7000;
        //private static int nextEngineerId = startEngineerId;
        //internal static int NextEngineerId { get => nextEngineerId++; }
    }

    readonly static Random R = new Random();
    internal static List<DO.Dependency> Dependencies { get; } = new();
    internal static List<DO.Task> Tasks { get; } = new();
    internal static List<DO.Engineer> Engineers { get; } = new();

    
}
