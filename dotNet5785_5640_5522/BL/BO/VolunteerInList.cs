namespace BO;

/// <summary>
/// Business object representing a volunteer as shown in the volunteer list view.
/// </summary>
/// <param name="Id">Unique identifier of the volunteer.</param>
/// <param name="FullName">Full name of the volunteer (first + last name).</param>
/// <param name="IsActive">Indicates whether the volunteer is currently active.</param>
/// <param name="TotalHandledCalls">Total number of calls the volunteer has handled (status: 'Handled').</param>
/// <param name="TotalCanceledCalls">Total number of calls the volunteer has canceled (status: 'Canceled').</param>
/// <param name="ExpiredCallsCount">Total number of calls the volunteer let expire (status: 'Expired').</param>
/// <param name="CurrentCallId">ID of the call currently being handled by the volunteer, if any.</param>
public class VolunteerInList
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    public int TotalHandledCalls { get; init; }
    public int TotalCanceledCalls { get; init; }
    public int ExpiredCallsCount { get; init; }
    public int? CurrentCallId { get; init; }
}
