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
        Open,               // 0
        InTreatment,        // 1
        Closed,             // 2
        Expired             // 3
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