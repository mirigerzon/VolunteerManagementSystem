using DO;
using Helpers;
namespace BO;
/// <summary>
/// Business object representing a volunteer's assignment to a specific call, as part of a call's assignment history or details view.
/// </summary>
/// <param name="VolunteerId">ID of the volunteer who was assigned to the call (nullable if unassigned).</param>
/// <param name="VolunteerName">Full name of the volunteer (for display purposes).</param>
/// <param name="StartTreatmentTime">Timestamp when the volunteer began handling the call.</param>
/// <param name="EndTreatmentTime">Timestamp when the volunteer finished handling the call, if completed.</param>
/// <param name="EndType">Indicates how the assignment ended (e.g., Completed, Aborted, Expired).</param>
    public class CallAssignInList
{
    public int? VolunteerId { get; set; }
    public string? VolunteerName { get; set; }
    public DateTime? StartTreatmentTime { get; set; }
    public DateTime? EndTreatmentTime { get; set; }
    public AssignmentEndType? EndType { get; set; }
    public override string ToString()
    {
        return
            $" VolunteerId: {VolunteerId} \n " +
            $"VolunteerName: {VolunteerName}\n " +
            $"StartTime: {StartTreatmentTime}\n " +
            $"EndTime: {EndTreatmentTime}\n " +
            $"EndType: {EndType} \n";
    }
}
