namespace DO;
public static class Enums
{
    public enum RoleEnum
    {
        Manager,
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