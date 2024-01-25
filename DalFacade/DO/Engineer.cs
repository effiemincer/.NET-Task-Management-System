
namespace DO;

/// <summary> 
/// Engineer: someone who can complete tasks
///</summary>

/// <param name="Id">A unique identifier </param>
/// <param name="FullName"> Full name </param>
/// <param name="EmailAddress"> Email</param>
/// <param name="ExperienceLevel"> experience from enum</param>
/// <param name="CostPerHour"> $$ per hour</param>
/// <param name="Inactive"> (T/F) </param>
public record Engineer
(
    int Id,
    string FullName,
    string EmailAddress,
    //enum
    Enums.EngineerExperience? ExperienceLevel = null,
    double? CostPerHour = null,
    bool Inactive = false
)
{
    public Engineer() : this (0, "", ""){ } //empty ctor for stage 3

}

