using Microsoft.VisualBasic;

namespace BO;

/// <summary>
/// Business object representing a call currently in progress by a volunteer. 
/// This is a read-only view used in various screens to display information about the current call being handled by a volunteer.
/// </summary>
/// <param name="Id">Unique identifier of the assignment (not displayed).</param>
/// <param name="CallId">Identifier of the call being handled.</param>
/// <param name="CallType">Type of the call (Emergency, Regular, etc.).</param>
/// <param name="Description">Optional textual description of the call.</param>
/// <param name="FullAddress">Full address of the call location.</param>
/// <param name="StartTime">Time when the call was opened.</param>
/// <param name="MaxEndTime">Optional maximum time allowed for completion of the call.</param>
/// <param name="EntryTime">Time when the volunteer started handling the call.</param>
/// <param name="DistanceFromVolunteer">Distance (in km) from the volunteer to the call location.</param>
/// <param name="Status">Status of the call - InProgress or InProgressAtRisk.</param>
public class CallInProgress
{
    public int Id { get; init; } // Assignment ID, not displayed
    public int CallId { get; init; }
    public CallType CallType { get; init; }
    public string? Description { get; init; }
    public string FullAddress { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? MaxEndTime { get; init; }
    public DateTime EntryTime { get; init; }
    public double DistanceFromVolunteer { get; init; }
    public CallInProgressStatus Status { get; init; }
}
