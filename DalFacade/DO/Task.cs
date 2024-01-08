//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;

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
/// <param name="Inactive"> (T/F) </param>
public record Task
(
    int Id,
    string Nickname,
    DateTime DateCreated = new DateTime(),
    string? Description = null,
    int? Duration = null,
    DateTime? Deadline = null,
    DateTime? ProjectedStartDate = null,
    Enums.EngineerExperience? DegreeOfDifficulty = null,
    //should be initialized --> is this required?
    int? AssignedEngineerId = null,
    DateTime? ActualEndDate = null,
    bool IsMilestone = false,
    DateTime? ActualStartDate = null,
    string Deliverable = "",
    string Notes = "",
    bool Inactive = false

)
{
    //empty ctor
    //public Task() : this(0, "") { }
}

