

using static DO.Enums;

namespace BO;

public class Engineer
{
    public int Id { get; init; }
    public string? Name { get; init; } 
    public string? EmailAddress { get; init; }
    public Enums.EngineerExperience? ExperienceLevel {  get; set; }
    public double CostPerHour {  get; init; }
    public BO.TaskInEngineer? Task {  get; set; }
        
}
