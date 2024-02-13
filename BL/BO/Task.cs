

using DO;

namespace BO;

public class Task
{
    public int Id { get; init; }
    public string? Alias { get; init; }
    public DateTime DateCreated { get; init; }
    public string? Description { get; init; }
    public BO.Enums.Status Status { get; init; }  
    public List<BO.TaskInList>? Dependencies { get; set; }
    public TimeSpan? Duration { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? ScheduledStartDate { get; set; }
    public DateTime? ProjectedStartDate {  get; set; }
    public Enums.EngineerExperience? DegreeOfDifficulty {  get; set; }
    public int? AssignedEngineerId {  get; set; }
    public DateTime? ActualEndDate { get; set; }
    public bool IsMilestone {  get; set; }
    public DateTime? ActualStartDate {  get; set; }
    public string? Deliverable {  get; set; }
    public BO.EngineerInTask? Engineer { get; set; }
    public BO.Enums.EngineerExperience? Complexity { get; set; }
    public string? Notes {  get; set; }
    public bool Inactive { get; }
}
