namespace DO;

/// <summary>
/// Dependency: limits the completion of tasks to certain dependiencies
/// </summary>

/// <param name = "Id">A unique identifier(automatic)</param>
/// <param name = "DependentTaskId"></param>
/// <param name = "RequisiteID">(DependsOn) </param>
/// <param name="Inactive"> (T/F) </param>
public record Dependency
(
    int Id,
    int? DependentTaskId = null,
    int? RequisiteID = null,
    bool Inactive = false
    )
{
    //empty ctor
    public Dependency() : this(0) { }     //for stage 3
}
