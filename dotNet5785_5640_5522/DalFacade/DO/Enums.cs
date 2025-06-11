namespace DO;
public static class Enums
{
    public enum RoleEnum
    {
        Admin,
        Volunteer
    }
    public enum TypeOfDistanceEnum
    {
        Aerial,
        Walking,
        Driving
    }
    public enum CallStatusEnum
    {
        New,
        Open,
        InProgress,
        Resolved, 
        Closed,
        Expired,
        Aborted
    }
    public enum CallTypeEnum
    {
        None,
        Medical,
        Technical,
        Social,
        Transportation
    }
    public enum TerminationTypeEnum
    {
        Treated,
        SelfCancelled,
        ManagerCancelled,
        Expired
    }
}