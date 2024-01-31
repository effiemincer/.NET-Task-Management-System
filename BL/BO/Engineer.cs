

using static DO.Enums;

namespace BO;

public class Engineer
{
    public int Id { get; init; }
    public string FullName { get; init; }
    public string EmailAddress { get; init; }
    //enum
    //public Enums.EngineerExperience? ExperienceLevel {  get; set; }
    public double CostPerHour {  get; init; }
    public BO.TaskInEngineer? Task {  get; set; }
    public bool Inactive {  get; }
        
}
