namespace BO;

/// <summary>
/// Business object representing an open call visible to a volunteer. 
/// This is a read-only summary used for listing available calls for assignment.
/// </summary>
/// <param name="Id">Unique identifier of the call.</param>
/// <param name="CallType">Type of the call (Medical, Technical, Social, Transportation).</param>
/// <param name="Description">Optional textual description of the call.</param>
/// <param name="Address">Address of the call location.</param>
/// <param name="OpeningTime">Time when the call was opened.</param>
/// <param name="MaxFinishTime">Latest time by which the call should be completed, if defined.</param>
/// <param name="DistanceFromVolunteer">Distance (in km) from the volunteer to the call location.</param>
public class OpenCallInList
{
    public int Id { get; set; } 
    public CallType CallType { get; set; } 
    public string? Description { get; set; } 
    public string Address { get; set; } = string.Empty;
    public DateTime? OpeningTime { get; set; } 
    public DateTime? MaxFinishTime { get; set; } 
    public double DistanceFromVolunteer { get; set; }
}