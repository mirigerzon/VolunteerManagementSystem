using DO;
namespace BO;

/// <summary>
/// Volunteer business object representing a volunteer in the system with all relevant business data.
/// </summary>
/// <param name="Id">Unique identifier of the volunteer.</param>
/// <param name="FullName">Full name of the volunteer (first + last name).</param>
/// <param name="PhoneNumber">Phone number of the volunteer.</param>
/// <param name="Email">Email address of the volunteer.</param>
/// <param name="Password">Password used by the volunteer (nullable).</param>
/// <param name="Address">Home address of the volunteer (nullable).</param>
/// <param name="Latitude">Latitude coordinate of the volunteer's address (nullable).</param>
/// <param name="Longitude">Longitude coordinate of the volunteer's address (nullable).</param>
/// <param name="Role">Role assigned to the volunteer (Volunteer or Manager).</param>
/// <param name="IsActive">Indicates whether the volunteer is currently active.</param>
/// <param name="MaxDistance">Maximum distance the volunteer is willing to travel (nullable).</param>
/// <param name="TypeOfDistance">Type of distance calculation preferred by the volunteer.</param>
/// <param name="TotalHandledCalls">Total number of calls the volunteer has handled.</param>
/// <param name="TotalCanceledCalls">Total number of calls the volunteer has canceled.</param>
/// <param name="ExpiredCallsCount">Total calls that he chose to handle and expired </param>
/// <param name="CurrentCall">The call currently being handled by the volunteer (nullable).</param>
public class Volunteer
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DO.Enums.RoleEnum Role { get; set; }
    public bool IsActive { get; set; }
    public double? MaxDistance { get; set; }
    public TypeOfDistance TypeOfDistance { get; set; }
    public int TotalHandledCalls { get; init; }
    public int TotalCanceledCalls { get; init; }
    public int ExpiredCallsCount { get; init; }
    public CallInProgress? CurrentCall { get; init; }
}