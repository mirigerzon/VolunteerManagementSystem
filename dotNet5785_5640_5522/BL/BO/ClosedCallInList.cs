namespace BO;

/// <summary>
/// Business object representing a closed call in a volunteer's call history list.
/// This is a read-only DTO used for display purposes only.
/// </summary>
/// <param name="Id">Unique identifier of the call.</param>
/// <param name="CallType">The type/category of the call.</param>
/// <param name="Address">Full address where the call was requested.</param>
/// <param name="OpenedAt">Date and time when the call was opened.</param>
/// <param name="AssignedAt">Date and time when the volunteer started handling the call.</param>
/// <param name="ClosedAt">Date and time when the call handling was completed (nullable).</param>
/// <param name="ClosureType">The type of closure result for the call (nullable).</param>
public class ClosedCallInList
{
    public int Id { get; init; }
    public CallType CallType { get; set; } // Enum assumed to be defined elsewhere
    public string Address { get; set; }
    public DateTime OpenedAt { get; init; }
    public DateTime AssignedAt { get; init; }
    public DateTime? ClosedAt { get; init; }
    public ClosureType? ClosureType { get; init; } // Enum assumed to be defined elsewhere
}
