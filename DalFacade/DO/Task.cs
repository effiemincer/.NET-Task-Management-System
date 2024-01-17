
namespace DO;

/// <summary>
/// Tasks: a task assigned to an engineer to complete
/// </summary>

/// <param name = "Id" > unique ID(created automatically)</param>
/// <param name = "Nickname"></param>
/// <param name = "Description"></param>
/// <param name = "Is it a milestone?">(T/F)</param>
/// <param name = "Date created"></param>
/// <param name = "Projected start Date"></param>
/// <param name = "Actual start Date"></param>
/// <param name = "Duration"></param>
/// <param name = "Deadline"></param>
///<param name = "Actual end Date"></param>
///<param name = "Deliverable"></param>
///<param name = "Notes"></param>
///<param name = "Assigned engineer ID"></param>
/// <param name = "Degree of difficulty"></param>
/// <param name= "Inactive"> (T/F) </param>
public record Task
(
    int Id,
    string Nickname,
    DateTime DateCreated = new DateTime(),
    string Description = "",
    TimeSpan? Duration = null,                                  
    DateTime? Deadline = null,
    DateTime? ProjectedStartDate = null,
    Enums.EngineerExperience? DegreeOfDifficulty = null,
    int? AssignedEngineerId = null,
    DateTime? ActualEndDate = null,
    bool IsMilestone = false,
    DateTime? ActualStartDate = null,
    string? Deliverable = null,
    string? Notes = null,
    bool Inactive = false

)
{
    //empty ctor
    //public Task() : this(0, "") { }   //for stage 3
}

