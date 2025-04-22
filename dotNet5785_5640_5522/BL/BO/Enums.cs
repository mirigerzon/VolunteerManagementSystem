namespace BO;
public enum VolunteerRole
{
    Volunteer,
    Manager
}
public enum DistanceType
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
public enum CallStatus
{
    Open,
    InTreatment,
    Closed,
    Expired,
    OpenAtRisk,
    InTreatmentAtRisk
}