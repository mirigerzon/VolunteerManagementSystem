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
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } = null;
    public string Address { get; set; } = null;
    public double? Latitude { get; set; } = null;
    public double? Longitude { get; set; } = null;
    public RoleEnum Role { get; set; }
    public bool IsActive { get; set; } = true;
    public double MaxOfDistance { get; set; } = null;
    public TypeOfDistanceEnum TypeOfDistance { get; set; } = TypeOfDistanceEnum[0];
    public Volunteer() { } //לשלב 3
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