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
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime? ArrivalTime,
    DateTime? EndTime,
    TerminationTypeEnum? EndStatus
)
{
    public Assignment(string? Id) : this(0, 0, 0, DateTime.Now, DateTime.Now, null) { }
}

public enum TerminationTypeEnum
{
    Treated,
    SelfCancelled,
    ManagerCancelled,
    Expired
}