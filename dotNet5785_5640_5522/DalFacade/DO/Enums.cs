namespace DO;
public static class Enums
{
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
    public enum CallStatusEnum
    {
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
        Canceled,
        Expired
    }
}