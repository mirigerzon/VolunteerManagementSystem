namespace BO;
/// <summary>
/// Business object representing a summarized view of a call for management or monitoring purposes.
/// Used to display call details in overviews or dashboards.
/// </summary>
/// <param name="Id">Unique identifier of the list entry (may differ from CallId in multi-call displays).</param>
/// <param name="CallId">Identifier of the original call.</param>
/// <param name="CallType">Type of the call (Medical, Technical, Social, Transportation).</param>
/// <param name="StartTime">Time when the call was initiated.</param>
/// <param name="TimeLeft">Estimated remaining time until the call deadline.</param>
/// <param name="LastVolunteerName">Name of the last volunteer who was assigned or involved in the call.</param>
/// <param name="TreatmentDuration">Duration of treatment or assistance provided so far.</param>
/// <param name="Status">Current status of the call (e.g., New, InProgress, Resolved).</param>
/// <param name="TotalAssignments">Total number of volunteers assigned to this call.</param>
public class CallInList
{
    public int? Id { get; set; }
    public int CallId { get; set; }
    public CallType CallType { get; set; }
    public DateTime? StartTime { get; set; }
    public TimeSpan? TimeLeft { get; set; }
    public string? LastVolunteerName { get; set; }
    public TimeSpan? TreatmentDuration { get; set; }
    public CallStatus Status { get; set; }
    public int TotalAssignments { get; set; }
    public override string ToString()
    {
        return $"\n ------------------------------- \n " +
            $"Id: {Id}\n " +
            $"CallId: {CallId}\n " +
            $"CallType: {CallType}\n " +
            $"StartTime: {StartTime}\n " +
            $"Status: {Status}\n " +
            $"TotalAssignments: {TotalAssignments}\n " +
            $" ------------------------------- \n";
    }
}