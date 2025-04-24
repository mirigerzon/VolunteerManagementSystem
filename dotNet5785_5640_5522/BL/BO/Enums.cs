namespace BO;
public enum UserRole
{
    Admin,
    Volunteer,
    Mentor,
    Guest
}
public enum VolunteerSortField
{
    FullName,
    Distance,
    Role,
    IsActive
}
public enum TypeOfDistance
{
    Aerial,
    Walking,
    Driving
}
public enum CallInProgressStatus
{
    InProgress,
    InProgressAtRisk
}
public enum CallType
{
    Medical,
    Technical,
    Social,
    Transportation
}
public enum CallStatus
{
    Open,
    InTreatment,
    Closed,
    Expired,
    OpenAtRisk,
    InTreatmentAtRisk
}
public enum ClosureType
{
    Treated,
    Canceled,
    Expired
}
public enum AssignmentEndType
{
    Finished,
    CanceledByVolunteer,
    Expired,
    ExpiredCanceledAutomatically
}
public enum CallFieldFilter
{
    Status,
    StartTime,
    CallerAddress
}
public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month
}