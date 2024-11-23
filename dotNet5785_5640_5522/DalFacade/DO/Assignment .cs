namespace DO;

/// <summary>
/// Assignment Entity represents an assignment given to a volunteer.
/// </summary>
/// <param name="Id">Unique identifier of the assignment.</param>
/// <param name="CallId">Unique identifier of the call related to the assignment.</param>
/// <param name="VolunteerId">Unique identifier of the volunteer assigned to the call.</param>
/// <param name="RequestedTime">The time the assignment was requested.</param>
/// <param name="ArrivalTime">The time the volunteer arrived at the location (may be null if not arrived yet).</param>
/// <param name="EndTime">The time the assignment was completed (default is null).</param>
/// <param name="Status">Current status of the assignment (Waiting, Active, or Closed).</param>

public record Assignment
{
    public int Id { get; set; }
    public int CallId { get; set; }
    public int VolunteerId { get; set; }
    public DateTime? ArrivalTime { get; set; }
    public DateTime? EndTime { get; set; } = null;
    public TerminationTypeEnum? Status { get; set; } = null;
    public Assignment() { } //לשלב 3
}

public enum TerminationTypeEnum
{

}
