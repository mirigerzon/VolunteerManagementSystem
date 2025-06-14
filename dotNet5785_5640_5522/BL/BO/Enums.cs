namespace BO;
public enum UserRole
{
    Admin,
    Volunteer
}
public enum VolunteerSortField
{
    Id,
    FullName,
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
    None,
    Medical,
    Technical,
    Social,
    Transportation
}
public enum CallStatus
{
    Open,               // 0
    InTreatment,        // 1
    Closed,             // 2
    Expired,            // 3
    OpenAtRisk,         // 4
    InTreatmentAtRisk   // 5
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