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
    string Description,
    int Duration,
    DateTime Deadline,
    DateTime ProjectedStartDate,
    //enum
    int DegreeOfDifficulty,
    //should be initialized --> is this required?
    int AssignedEngineerId,
    DateTime? ActualEndDate = null,
    DateTime DateCreated = new DateTime(),
    bool IsMilestone = false,
    DateTime? ActualStartDate = null,
    string Deliverable = "",
    string Notes = "",
    bool Inactive = false

)
{
    //empty ctor
    public Task() : this(0, "", "", 0, new DateTime(), new DateTime(), 0, 0) {}
}
