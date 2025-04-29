namespace BO;
public enum UserRole
{
    Manager,
    Volunteer
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
public enum OpenCallSortField
{
    Id,
    OpeningTime,
    MaxFinishTime,
    DistanceFromVolunteer
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
    Id,
    Type,
    Description,
    CallerAddress,
    Status,
    StartTime,
    MaxEndTime
}

public enum TimeUnit
{
    Year,
    Month,
    Day,
    Hour,
    Minute
}