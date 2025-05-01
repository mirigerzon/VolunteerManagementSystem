namespace BO;
/// <summary>
/// Business object representing a full logical "Call" entity, including its core data and related volunteer assignments.
/// Used primarily in detailed views or administrative contexts where all data about a call is needed.
/// </summary>
/// <param name="Id">Unique identifier of the call.</param>
/// <param name="Type">The category or type of the call (Medical, Technical, Social, etc.).</param>
/// <param name="Description">Optional textual description of the call provided by the caller.</param>
/// <param name="CallerAddress">Address or location description where the call is located.</param>
/// <param name="Latitude">Latitude coordinate of the call's location (if available).</param>
/// <param name="Longitude">Longitude coordinate of the call's location (if available).</param>
/// <param name="StartTime">Timestamp indicating when the call was opened.</param>
/// <param name="MaxEndTime">Optional maximum time allowed to finish the call.</param>
/// <param name="Status">The current status of the call (New, InProgress, Resolved, etc.).</param>
/// <param name="Assignments">List of volunteer assignments related to this call (can be empty or null).</param>
public class Call
{
    public int Id { get; set; }
    public CallType Type { get; set; }
    public string? Description { get; set; }
    public string? CallerAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? MaxEndTime { get; set; }
    public CallStatus Status { get; set; }
    public List<CallAssignInList>? Assignments { get; set; }
    public override string ToString()
    {
        return $"\n ------------------------------- \n " +
            $"Id: {Id} \n " +
            $"Description: {Description}\n " +
            $"StartTime: {StartTime}\n " +
            $"Status: {Status}\n " +
            $"Assignment - \n{string.Join("\n", Assignments ?? new())}\n " +
            $" ------------------------------- \n";
    }
}