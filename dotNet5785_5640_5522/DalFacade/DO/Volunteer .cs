using System.Net;

namespace DO;

/// <summary>
/// Volunteer Entity represents a volunteer with all its properties.
/// </summary>
/// <param name="Id">Unique personal ID of the volunteer.</param>
/// <param name="FullName">Full name of the volunteer.</param>
/// <param name="PhoneNumber">Phone number of the volunteer.</param>
/// <param name="Email">Email address of the volunteer.</param>
/// <param name="Password">Password of the volunteer (default null).</param>
/// <param name="Address">Home address of the volunteer (default null).</param>
/// <param name="Latitude">Latitude of the volunteer’s location (default null).</param>
/// <param name="Longitude">Longitude of the volunteer’s location (default null).</param>
/// <param name="Role">Role of the volunteer (Mentor or Volunteer).</param>
/// <param name="IsActive">Indicates whether the volunteer is currently active (default true).</param>
/// <param name="TypeOfDistance">Type of distance calculation for the volunteer (Aerial, Walking, or Driving distance).</param>
public record Volunteer
(
    int Id,
    string FullName,
    string PhoneNumber,
    string Email,
    string Password,
    string Address,
    double? Latitude,
    double? Longitude,
    Enums.RoleEnum? Role,
    bool IsActive,
    double? MaxOfDistance,
    Enums.TypeOfDistanceEnum? TypeOfDistance

)
{

    public Volunteer(int id, string fullName, string phoneNumber, string email, string password, string address, double latitude, double? longitude, double maxOfDistance)
        : this(id, fullName, phoneNumber, email, password, address, latitude, longitude, Enums.RoleEnum.Volunteer, true, maxOfDistance, Enums.TypeOfDistanceEnum.Driving)
    { }
    public Volunteer() : this(0, "", "", "", "", "", 0, 0, Enums.RoleEnum.Volunteer, true,0, Enums.TypeOfDistanceEnum.Driving) { }
}