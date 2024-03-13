


using DO;

namespace BO;

/// <summary>
/// Class representing a Task
/// </summary>
public class Task
{
    public int Id { get; init; }
    public string? Alias { get; init; }
    public DateTime DateCreated { get; init; }
    public string? Description { get; init; }
    public BO.Enums.Status? Status { get; set; }  
    public List<BO.TaskInList>? Dependencies { get; set; }
    public BO.MilestoneInTask? Milestone { get; set; }
    public TimeSpan? RequiredEffortTime { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ProjectedStartDate { get; set; }
    public DateTime? Deadline { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public bool Deliverable { get; set; }
    public string? Remarks { get; set; }
    public BO.EngineerInTask? Engineer { get; set; }
    public Enums.EngineerExperience? Complexity { get; set; }
    public override string ToString() => this.ToStringProperty();
}
