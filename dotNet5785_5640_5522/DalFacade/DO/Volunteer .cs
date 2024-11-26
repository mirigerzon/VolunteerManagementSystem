using System.Net;

namespace DO;

/// <summary>
/// Volunteer Entity represents a volunteer with all its properties.
/// </summary>
/// <param name="Id">Unique personal ID of the volunteer.</param>
/// <param name="FirstName">First name of the volunteer.</param>
/// <param name="LastName">Last name of the volunteer.</param>
/// <param name="PhoneNumber">Phone number of the volunteer.</param>
/// <param name="Email">Email address of the volunteer.</param>
/// <param name="Password">Password of the volunteer (default null).</param>
/// <param name="Address">Home address of the volunteer (default null).</param>
/// <param name="Latitude">Latitude of the volunteer’s location (default null).</param>
/// <param name="Longitude">Longitude of the volunteer’s location (default null).</param>
/// <param name="Role">Role of the volunteer (Mentor or Volunteer).</param>
/// <param name="IsActive">Indicates whether the volunteer is currently active (default true).</param>
/// <param name="TypeOfDistance">Type of distance calculation for the volunteer (Aerial, Walking, or Driving distance).</param>

public class Volunteer
(
    int Id,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string Password,
    string Address,
    double? Latitude,
    double? Longitude,
    RoleEnum? Role,
    bool IsActive,
    double? MaxOfDistance,
    TypeOfDistanceEnum? TypeOfDistance
)
{
    public Volunteer(string? Id) : this(0, "", "", "", "", "", "", 0, 0, null, true, null, null) { }
}

public enum RoleEnum
{
    Mentor,
    Volunteer
}

public enum TypeOfDistanceEnum
{
    AerialDistance,
    WalkingDistance,
    DrivingDistance
}