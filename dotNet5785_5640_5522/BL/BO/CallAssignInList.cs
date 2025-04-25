using Helpers;
namespace BO;
public class CallAssignInList
{
    public int? VolunteerId { get; set; }
    public string? VolunteerName { get; set; }
    public DateTime? StartTreatmentTime { get; set; }
    public DateTime? EndTreatmentTime { get; set; }
    public AssignmentEndType? EndType { get; set; }
    public override string ToString() => this.ToStringProperty();
}
